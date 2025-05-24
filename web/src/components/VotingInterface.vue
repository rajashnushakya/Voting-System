<template>
  <v-card class="mb-6 rounded-lg shadow-md">
    <v-card-title class="text-h5 font-medium">
      Select Your Candidate
    </v-card-title>

    <v-card-text v-if="candidates.length > 0">
      <div class="text-gray-600 dark:text-gray-300 mb-4">
        Please select a candidate to cast your vote.
      </div>

      <div>
        <div
          v-for="candidate in candidates"
          :key="candidate.id"
          @click="selectedCandidateId = candidate.id"
          :class="['candidate-item', { 'candidate-selected': selectedCandidateId === candidate.id }]"
          class="mb-2 cursor-pointer p-3 rounded-lg flex items-center hover:bg-gray-100 dark:hover:bg-gray-700"
        >
          <v-avatar size="36" class="mr-3">
            <v-img :src="candidate.avatar || 'default-avatar.png'" />
          </v-avatar>
          <div>
            <div class="text-base font-medium">{{ candidate.name }}</div>
            <div class="text-sm text-gray-500">{{ candidate.party }}</div>
          </div>
        </div>
      </div>

      <div class="flex justify-end mt-4">
        <v-btn v-if="selectedCandidateId !== null" color="primary" @click="castVote">
          Cast Vote
        </v-btn>
      </div>
    </v-card-text>

    <v-card-text v-else>
      <p class="text-gray-500 dark:text-gray-400">No candidates found.</p>
    </v-card-text>
  </v-card>

  <v-btn variant="outlined" @click="navigateToEnrollment">Back to Elections</v-btn>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import candidateService from '../service/candidateService';
import voteService from '../service/voteService';
import type { VotePayload } from '../service/voteService';

const router = useRouter();
const emit = defineEmits(['update:activeTab', 'vote-cast']);
const route = useRoute();

interface Candidate {
  id: number;
  name: string;
  party: string;
  partyColor: string;
  position: string;
  avatar: string | null;
}

const navigateToEnrollment = () => {
  router.push({ name: 'voter-dashboard' });
};

const selectedCandidateId = ref<number | null>(null);
const candidates = ref<Candidate[]>([]);

const cservice = new candidateService();
const vservice = new voteService();

const fetchCandidates = async () => {
  const electionCentreId = localStorage.getItem('electionCenterId');
  if (!electionCentreId) {
    alert("Election Center ID not found.");
    return;
  }

  try {
    const response = await cservice.getCandidateByCentreId(electionCentreId);
    const allCandidates = response.data;

    candidates.value = allCandidates.map((candidate: any) => ({
      id: candidate.candidateId,
      name: candidate.fullName,
      party: candidate.partyName,
      partyColor: '',
      position: '',
      avatar: null
    }));

    console.log("Candidates loaded:", candidates.value);
  } catch (error) {
    console.error("Error fetching candidates:", error);
    alert("Failed to load candidates.");
  }
};

const castVote = async () => {
  const electionId = route.params.electionId;
  const voterId = localStorage.getItem('voterid');
  const electionCentreId = localStorage.getItem('electionCenterId');

  const candidate = candidates.value.find(c => c.id === selectedCandidateId.value);
  if (!candidate) {
    alert("Please select a candidate.");
    return;
  }

  if (!electionId || !voterId || !electionCentreId || !selectedCandidateId.value) {
    alert("Missing required information to cast the vote.");
    return;
  }

  const payload: VotePayload = {
    voterId: parseInt(voterId),
    candidateId: candidate.id,
    electionId: parseInt(electionId as string),
    electionCentreId: parseInt(electionCentreId)
  };

  try {
    const result = await vservice.castVote(payload);
    if (result === "Vote successfully recorded.") {
      alert("Vote cast successfully!");
      emit('vote-cast', {
        candidateId: candidate.id,
        candidateName: candidate.name,
        candidateParty: candidate.party,
        candidatePartyColor: candidate.partyColor,
        candidateAvatar: candidate.avatar
      });
    } else {
      alert("Cannot vote twice." + result);
    }
  } catch (error: any) {
    alert("Failed to cast vote: " + (error.message || "Unknown error"));
  }
};

onMounted(() => {
  fetchCandidates();
});
</script>

<style scoped>
.v-card {
  transition: all 0.3s ease;
}
.v-card:hover {
  transform: translateY(-2px);
}

/* Candidate item style */
.candidate-item {
  transition: background-color 0.3s ease, border 0.3s ease;
}

.candidate-selected {
  background-color: #e0f7fa; /* Light blue background */
  border: 2px solid #00acc1; /* Teal border */
}
</style>
