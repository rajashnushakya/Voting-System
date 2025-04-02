<template>
    <v-card class="mb-6 rounded-lg shadow-md">
      <v-card-title class="text-h5 font-medium">
        Vote: {{ selectedElection ? selectedElection.name : 'Select an Election' }}
        <v-btn
          icon
          class="ml-auto"
          @click="$emit('update:activeTab', 'elections')"
        >
          <v-icon>mdi-close</v-icon>
        </v-btn>
      </v-card-title>
      <v-card-text v-if="selectedElection && candidates.length > 0">
        <div class="text-gray-600 dark:text-gray-300 mb-4">
          Please select a candidate to cast your vote.
        </div>
        
        <v-radio-group v-model="selectedCandidate">
          <v-card
            v-for="candidate in candidates"
            :key="candidate.id"
            class="mb-3 p-4 hover:bg-gray-50 dark:hover:bg-gray-800 transition-colors cursor-pointer"
            :class="{ 'border-2 border-primary': selectedCandidate === candidate.id }"
            @click="selectedCandidate = candidate.id"
          >
            <div class="flex items-center">
              <v-radio :value="candidate.id" class="mr-2"></v-radio>
              <v-avatar size="48" class="mr-4">
                <v-img :src="candidate.avatar" alt="Candidate avatar"></v-img>
              </v-avatar>
              <div class="flex-grow">
                <div class="text-h6">{{ candidate.name }}</div>
                <div class="flex items-center">
                  <v-chip small :color="candidate.partyColor" class="mr-2">
                    {{ candidate.party }}
                  </v-chip>
                  <span class="text-sm text-gray-500 dark:text-gray-400">{{ candidate.position }}</span>
                </div>
              </div>
            </div>
          </v-card>
        </v-radio-group>
        
        <div class="flex justify-end mt-4">
          <v-btn color="primary" :disabled="!selectedCandidate" @click="castVote">
            Cast Vote
          </v-btn>
        </div>
      </v-card-text>
    </v-card>
</template>

<script setup>
import { ref, watch } from 'vue';

const props = defineProps({
  selectedElection: {
    type: Object,
    default: null
  }
});

const emit = defineEmits(['update:activeTab', 'vote-cast']);

const selectedCandidate = ref(null);
const candidates = ref([]);

// Function to return demo candidate data
const getCandidatesForElection = (electionId) => {
  return [
    {
      id: 1,
      name: "John Doe",
      avatar: "https://randomuser.me/api/portraits/men/1.jpg",
      party: "Democratic Party",
      partyColor: "blue",
      position: "Mayor",
      bio: "Experienced leader with a passion for community development.",
      keyIssues: ["Healthcare", "Education", "Infrastructure"],
      experience: "10 years in public service",
      supportedBy: "Community Leaders"
    },
    {
      id: 2,
      name: "Jane Smith",
      avatar: "https://randomuser.me/api/portraits/women/1.jpg",
      party: "Republican Party",
      partyColor: "red",
      position: "Governor",
      bio: "Committed to economic growth and security.",
      keyIssues: ["Job Creation", "Lower Taxes", "Public Safety"],
      experience: "15 years in government",
      supportedBy: "Business Associations"
    }
  ];
};

// Watch for changes in the selected election
watch(() => props.selectedElection, (newElection) => {
  if (newElection) {
    selectedCandidate.value = null;
    candidates.value = getCandidatesForElection(newElection.id);
  }
}, { immediate: true });

const castVote = () => {
  if (!selectedCandidate.value) return;
  
  const candidate = candidates.value.find(c => c.id === selectedCandidate.value);
  if (candidate) {
    emit('vote-cast', {
      electionId: props.selectedElection.id,
      candidateId: candidate.id,
      candidateName: candidate.name,
      candidateParty: candidate.party,
      candidatePartyColor: candidate.partyColor,
      candidateAvatar: candidate.avatar
    });
  }
};
</script>

<style scoped>
.v-card {
  transition: all 0.3s ease;
}
.v-card:hover {
  transform: translateY(-2px);
}
</style>