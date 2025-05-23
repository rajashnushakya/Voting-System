<template>
  <v-card class="mb-6 rounded-lg shadow-md">
    <v-card-title class="text-h5 font-medium">
      Select Your Candidate
    </v-card-title>

    <v-card-text v-if="candidates.length > 0">
      <div class="text-gray-600 dark:text-gray-300 mb-4">
        Please select a candidate to cast your vote.
      </div>

      <v-radio-group v-model="selectedCandidateId">
        <v-radio
          v-for="candidate in candidates"
          :key="candidate.id"
          :label="candidate.name + ' - ' + candidate.party"
          :value="candidate.id"
          class="mb-2"
        >
          <template #label>
            <div class="flex items-center">
              <v-avatar size="36" class="mr-3">
                <v-img :src="candidate.avatar || 'default-avatar.png'" />
              </v-avatar>
              <div>
                <div class="text-base font-medium">{{ candidate.name }}</div>
                <div class="text-sm text-gray-500">{{ candidate.party }}</div>
              </div>
            </div>
          </template>
        </v-radio>
      </v-radio-group>

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
import { useRoute } from 'vue-router';
import candidateService from '../service/candidateService';
import voteService from '../service/voteService';
import type { VotePayload } from '../service/voteService';
import { useRouter } from 'vue-router';

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

  router.push({ name: 'voter-dashboard'});
};
const selectedCandidateId = ref<number | null>(null);
const candidates = ref<Candidate[]>([]);

const cservice = new candidateService();
const vservice = new voteService();

// Fetch candidates based on election center
const fetchCandidates = async () => {
  const electionCentreId = localStorage.getItem('electionCenterId');
  if (!electionCentreId) {
    alert("Election Center ID not found.");
    return;
  }

  try {
    const response = await cservice.getCandidateByCentreId(electionCentreId);
    const allCandidates = response.data;

    // Adjusting the candidate structure to match the response format
    candidates.value = allCandidates.map((candidate: any) => ({
      id: candidate.candidateId, // Use candidateId instead of id
      name: candidate.fullName,   // Use fullName as name
      party: candidate.partyName, // Use partyName for party
      partyColor: '',             // You might not have partyColor in the response, so leave empty
      position: '',               // If the position is not in the response, leave empty
      avatar: null                // Assuming you don't have avatar in the response
    }));

    console.log("Candidates loaded:", candidates.value); // Debugging output
  } catch (error) {
    console.error("Error fetching candidates:", error);
    alert("Failed to load candidates.");
  }
};
const castVote = async () => {
  // Retrieve values from the URL params and local storage
  const electionId = route.params.electionId;
  const voterId = localStorage.getItem('voterid');
  const electionCentreId = localStorage.getItem('electionCenterId');

  // Ensure candidate selection
  const candidate = candidates.value.find(c => c.id === selectedCandidateId.value);
  if (!candidate) {
    console.error("No candidate selected!");
    alert("Please select a candidate.");
    return;
  }

  console.log("Selected candidate ID:", selectedCandidateId.value);
  console.log("Election ID:", electionId);
  console.log("Voter ID:", voterId);
  console.log("Election Centre ID:", electionCentreId);

  // Ensure all necessary values are available
  if (!electionId || !voterId || !electionCentreId || !selectedCandidateId.value) {
    console.error("Missing required values!");
    alert("Missing required information to cast the vote.");
    return;
  }

  // Create the payload for the vote
  const payload: VotePayload = {
    voterId: parseInt(voterId),
    candidateId: candidate.id,
    electionId: parseInt(electionId as string),
    electionCentreId: parseInt(electionCentreId)
  };

  console.log("Casting vote with payload:", payload);

  try {
    // Call the vote service to cast the vote
    const result = await vservice.castVote(payload);

    // Since the response is just a string "Vote successfully recorded."
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
      console.error("Failed to cast vote:", result);
      alert("Cannot vote twice." +result);
    }
  } catch (error: any) {
    // Log error if the request fails
    console.error("Vote cast failed:", error);
    alert("Failed to cast vote: " + (error.message || "Unknown error"));
  }
};


// Fetch candidates on mount
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
</style>
