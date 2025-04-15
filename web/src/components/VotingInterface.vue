<template>
  <v-card class="mb-6 rounded-lg shadow-md">
    <v-card-title class="text-h5 font-medium">
      Select Your Candidate
      <v-btn
        icon
        class="ml-auto"
        @click="$emit('update:activeTab', 'elections')"
      >
        <v-icon>mdi-close</v-icon>
      </v-btn>
    </v-card-title>

    <v-card-text v-if="candidates.length > 0">
      <div class="text-gray-600 dark:text-gray-300 mb-4">
        Please select a candidate to cast your vote.
      </div>

      <v-radio-group v-model="selectedCandidateId">
        <v-card
          v-for="candidate in candidates"
          :key="candidate.id"
          class="mb-3 p-4 hover:bg-gray-50 dark:hover:bg-gray-800 transition-colors cursor-pointer"
          :class="{ 'border-2 border-primary': selectedCandidateId === candidate.id }"
          @click="selectedCandidateId = candidate.id"
        >
          <div class="flex items-center">
            <v-radio :value="candidate.id" class="mr-2"></v-radio>
            <v-avatar size="48" class="mr-4">
              <v-img :src="candidate.avatar || 'default-avatar.png'" alt="Candidate avatar"></v-img>
            </v-avatar>
            <div class="flex-grow">
              <div class="text-h6">{{ candidate.name }}</div>
              <div class="flex items-center">
                <v-chip small :color="candidate.partyColor" class="mr-2">
                  {{ candidate.party }}
                </v-chip>
                <span class="text-sm text-gray-500 dark:text-gray-400">
                  {{ candidate.position }}
                </span>
              </div>
            </div>
          </div>
        </v-card>
      </v-radio-group>

      <div class="flex justify-end mt-4">
        <v-btn color="primary" :disabled="!selectedCandidateId" @click="castVote">
          Cast Vote
        </v-btn>
      </div>
    </v-card-text>

    <v-card-text v-else>
      <p class="text-gray-500 dark:text-gray-400">No candidates found.</p>
    </v-card-text>
  </v-card>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue';
import candidateService from '../service/candidateService';

interface Candidate {
  id: number;
  name: string;
  party: string;
  partyColor: string;
  position: string;
  avatar: string | null;
}

const emit = defineEmits(['update:activeTab', 'vote-cast']);

const selectedCandidateId = ref<number | null>(null);
const candidates = ref<Candidate[]>([]);
const service = new candidateService();

// Fetch candidates based on the voter ID
const fetchCandidates = async () => {
  const voterId = localStorage.getItem('voterid');
  if (!voterId) {
    alert("Voter ID not found.");
    return;
  }

  try {
    const response = await service.getCandidatesByVoterId(voterId);
    const allCandidates = response.data;
    console.log('API Response:', allCandidates);

    candidates.value = allCandidates.map((candidate: any) => ({
      id: candidate.id,
      name: candidate.fullName,
      party: candidate.partyId,
      partyColor: 'indigo', // Default color, adjust as needed
    }));
  } catch (error) {
    console.error("Error fetching candidates:", error);
    alert("Failed to load candidates.");
  }
};

// Cast the vote
const castVote = () => {
  if (!selectedCandidateId.value) return;

  const candidate = candidates.value.find(c => c.id === selectedCandidateId.value);
  if (candidate) {
    console.log('Casting vote for:', candidate);
    emit('vote-cast', {
      candidateId: candidate.id,
      candidateName: candidate.name,
      candidateParty: candidate.party,
      candidatePartyColor: candidate.partyColor,
      candidateAvatar: candidate.avatar
    });
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
</style>
