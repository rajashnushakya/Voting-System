<template>
  <v-card class="mb-6 rounded-lg shadow-md">
    <v-card-title class="text-h5 font-medium d-flex align-center">
      Your Voting History
      <v-spacer></v-spacer>
      <v-text-field
        v-model="searchQuery"
        append-inner-icon="mdi-magnify"
        label="Search history"
        density="compact"
        hide-details
        variant="outlined"
        class="max-w-xs"
      ></v-text-field>
    </v-card-title>

    <v-card-text>
      <div class="mb-4">
        <v-chip-group v-model="selectedFilter" filter>
          <v-chip value="all">All</v-chip>
          <v-chip value="presidential">Presidential</v-chip>
          <v-chip value="local">Local</v-chip>
          <v-chip value="referendum">Referendum</v-chip>
        </v-chip-group>
      </div>

      <v-timeline v-if="filteredHistory.length > 0" align-top dense>
        <v-timeline-item
          v-for="(history, index) in filteredHistory"
          :key="index"
          :dot-color="history.partyColor"
          size="small"
        >
          <template v-slot:opposite>
            <div class="text-caption text-grey">{{ history.date }}</div>
          </template>

          <v-card class="elevation-1 mb-3">
            <v-card-text>
              <div class="d-flex align-start">
                <v-avatar size="48" class="mr-4">
                  <v-img :src="history.candidateAvatar" :alt="history.candidate"></v-img>
                </v-avatar>

                <div class="flex-grow-1">
                  <div class="d-flex align-center">
                    <div class="text-h6">{{ history.election }}</div>
                    <v-chip size="x-small" :color="getElectionTypeColor(history.electionType)" class="ml-2">
                      {{ history.electionType }}
                    </v-chip>
                  </div>

                  <div class="d-flex align-center mt-1">
                    <div class="font-weight-medium">{{ history.candidate }}</div>
                    <v-chip size="x-small" :color="history.partyColor" class="ml-2">
                      {{ history.party }}
                    </v-chip>
                  </div>

                  <div class="mt-2 text-body-2 text-grey-darken-1">
                    <div class="d-flex align-center">
                      <v-icon size="small" class="mr-1">mdi-map-marker</v-icon>
                      <span>Voted at: {{ history.votingLocation }}</span>
                    </div>

                    <div class="d-flex align-center mt-1">
                      <v-icon size="small" class="mr-1">mdi-check-circle</v-icon>
                      <span>Vote verified: {{ history.verificationTime }}</span>
                    </div>
                  </div>
                </div>

                <v-btn
                  icon
                  variant="text"
                  size="small"
                  @click="expandedItem = expandedItem === index ? null : index"
                >
                  <v-icon>{{ expandedItem === index ? 'mdi-chevron-up' : 'mdi-chevron-down' }}</v-icon>
                </v-btn>
              </div>

              <v-expand-transition>
                <div v-if="expandedItem === index" class="mt-3 pt-3 border-t border-gray-200">
                  <div class="text-body-2">
                    <div class="font-weight-medium mb-1">Election Details:</div>
                    <div class="ml-2 mb-2">
                      <div>Type: {{ history.electionType }}</div>
                      <div>Ballot ID: {{ history.ballotId }}</div>
                      <div>Voter ID: {{ history.voterId }}</div>
                    </div>

                    <div class="font-weight-medium mb-1">Candidate Platform:</div>
                    <div class="ml-2 mb-2">
                      <p>{{ history.candidatePlatform }}</p>
                    </div>

                    <div v-if="history.keyIssues && history.keyIssues.length">
                      <div class="font-weight-medium mb-1">Key Issues:</div>
                      <div class="d-flex flex-wrap gap-1 ml-2">
                        <v-chip
                          v-for="(issue, i) in history.keyIssues"
                          :key="i"
                          size="x-small"
                          :color="history.partyColor"
                          variant="outlined"
                        >
                          {{ issue }}
                        </v-chip>
                      </div>
                    </div>
                  </div>

                  <div class="d-flex justify-end mt-3">
                    <v-btn
                      size="small"
                      variant="outlined"
                      prepend-icon="mdi-file-document-outline"
                      @click="viewBallot(history)"
                    >
                      View Ballot
                    </v-btn>
                  </div>
                </div>
              </v-expand-transition>
            </v-card-text>
          </v-card>
        </v-timeline-item>
      </v-timeline>

      <div v-else class="text-center py-8">
        <v-icon size="large" class="text-gray-400">mdi-history</v-icon>
        <div class="text-h6 mt-4">No Voting History Found</div>
        <div class="text-body-1 text-gray-600 dark:text-gray-400 mt-2">
          {{ searchQuery ? 'No results match your search criteria.' : "You haven't voted in any elections yet." }}
        </div>
        <v-btn color="primary" class="mt-4" @click="$emit('update:activeTab', 'elections')">
          View Elections
        </v-btn>
      </div>
    </v-card-text>

    <v-dialog v-model="showBallotDialog" max-width="600">
      <v-card>
        <v-card-title class="text-h5">
          Ballot Details
          <v-spacer></v-spacer>
          <v-btn icon @click="showBallotDialog = false">
            <v-icon>mdi-close</v-icon>
          </v-btn>
        </v-card-title>
        <v-card-text v-if="selectedBallot">
          <v-img
            src="https://via.placeholder.com/600x400?text=Official+Ballot"
            class="mb-4 rounded-lg"
            alt="Official Ballot"
          ></v-img>

          <div class="d-flex flex-column gap-2">
            <div class="d-flex" v-for="(val, label) in {
              'Election': selectedBallot.election,
              'Date': selectedBallot.date,
              'Ballot ID': selectedBallot.ballotId,
              'Voter ID': selectedBallot.voterId,
              'Voting Location': selectedBallot.votingLocation,
              'Selected Candidate': `${selectedBallot.candidate} (${selectedBallot.party})`,
              'Verification Time': selectedBallot.verificationTime,
              'Verification Hash': selectedBallot.verificationHash
            }" :key="label">
              <div class="font-weight-medium min-w-[150px]">{{ label }}:</div>
              <div>{{ val }}</div>
            </div>
          </div>
        </v-card-text>
        <v-card-actions>
          <v-spacer></v-spacer>
          <v-btn color="primary" variant="outlined" prepend-icon="mdi-printer">Print Ballot</v-btn>
          <v-btn color="primary" prepend-icon="mdi-download">Download PDF</v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
  </v-card>
</template>
<script setup lang="ts">
import { ref, computed, onMounted } from 'vue';
import voteService from '../service/voteService';

const vservice = new voteService();

interface VotingHistoryItem {
  date: string;
  election: string;
  candidate: string;
  party: string;
  candidateAvatar: string;
  partyColor: string;
  electionType?: string;
  votingLocation?: string;
  verificationTime?: string;
  ballotId?: string;
  voterId?: string;
  verificationHash?: string;
  candidatePlatform?: string;
  keyIssues?: string[];
}

const props = defineProps({
  votingHistory: {
    type: Array as () => VotingHistoryItem[],
    default: () => []
  }
});

const emit = defineEmits(['update:activeTab']);

const searchQuery = ref('');
const selectedFilter = ref('all');
const expandedItem = ref<number | null>(null);
const showBallotDialog = ref(false);
const selectedBallot = ref<any>(null);

const realVotingHistory = ref<VotingHistoryItem[]>([]);

const enhancedHistory = computed(() =>
  realVotingHistory.value.map(history => ({
    ...history,
    electionType: getElectionType(history.election),
    votingLocation: 'Central High School, LA',  
    verificationTime: '2025-04-01 10:30 AM',
    ballotId: `BAL-${Math.random().toString(36).substr(2, 9)}`,
    voterId: `VOT-${Math.random().toString(36).substr(2, 9)}`,
    verificationHash: Math.random().toString(36).substring(2, 15),
    candidatePlatform: 'Transparency, Education, and Infrastructure.',
    keyIssues: ['Education', 'Healthcare', 'Job Growth']
  }))
);

const filteredHistory = computed(() => {
  let result = enhancedHistory.value;
  const q = searchQuery.value.toLowerCase();

  if (q) {
    result = result.filter(item =>
      item.election.toLowerCase().includes(q) ||
      item.candidate.toLowerCase().includes(q) ||
      item.party.toLowerCase().includes(q) ||
      item.electionType.toLowerCase().includes(q)
    );
  }

  if (selectedFilter.value !== 'all') {
    result = result.filter(item => item.electionType.toLowerCase() === selectedFilter.value);
  }

  return result;
});

const getElectionType = (name: string) => {
  if (name.toLowerCase().includes('presidential')) return 'Presidential';
  if (name.toLowerCase().includes('local')) return 'Local';
  if (name.toLowerCase().includes('referendum')) return 'Referendum';
  return 'General';
};

const getElectionTypeColor = (type: string) => {
  switch (type.toLowerCase()) {
    case 'presidential': return 'blue';
    case 'local': return 'green';
    case 'referendum': return 'purple';
    default: return 'grey';
  }
};

const viewBallot = (history: any) => {
  selectedBallot.value = history;
  showBallotDialog.value = true;
};

const voteHistory = async () => {
  const voterId = localStorage.getItem('voterid');
  if (!voterId) {
    console.error('No voter ID found in local storage.');
    return;
  }

  try {
    const data = await vservice.getVotesByVoter(voterId);
    realVotingHistory.value = data.map((vote: any) => ({
      date: new Date().toLocaleDateString(), 
      election: vote.electionName,
      candidate: vote.candidateName,
      party: vote.partyName,
      candidateAvatar: `https://via.placeholder.com/48?text=${vote.candidateName.charAt(0)}`,
      partyColor: 'blue', 
    }));
    console.log('Voting history:', realVotingHistory.value);
  } catch (err) {
    console.error('Failed to load voting history:', err);
  }
};

onMounted(() => {
  console.log('Component mounted');
  voteHistory();
});

</script>


