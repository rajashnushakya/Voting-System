<template>
    <v-app>
      <v-app-bar style="background-color: #003893;" >
        <v-btn  @click="activeTab = 'elections'" style="color: white;">Elections</v-btn>
<v-btn  @click="activeTab = 'voting'" style="color: white;">Vote</v-btn>
<v-btn  @click="activeTab = 'history'" style="color: white;">History</v-btn>

      </v-app-bar>
  
      <v-main class="bg-gray-100 dark:bg-gray-900 min-h-screen">
        <v-container fluid>
          <!-- Elections List -->
          <v-card v-if="activeTab === 'elections'" class="mb-6 rounded-lg shadow-md">
            <v-card-title class="text-h5 font-medium">
              Available Elections
            </v-card-title>
            <v-card-text>
              <v-data-table
                :headers="electionHeaders"
                :items="ongoingElections"
                :items-per-page="5"
                class="elevation-1 rounded-lg"
              >
                <template v-slot:item.status="{ item }">
                  <v-chip
                    :color="getStatusColor(item.status)"
                    text-color="white"
                    small
                  >
                    {{ item.status }}
                  </v-chip>
                </template>
                <template v-slot:item.actions="{ item }">
                  <v-btn
                    small
                    color="primary"
                    @click="enrollInElection(item)"
                  >
                    {{ item.enrolled ? 'Enrolled' : 'Enroll' }}
                  </v-btn>
                  <v-btn
                    small
                    color="success"
                    class="ml-2"
                    :disabled="!item.enrolled || item.status !== 'Open' || item.voted"
                    @click="activeTab = 'voting'; selectedElection = item"
                  >
                    Vote
                  </v-btn>
                </template>
              </v-data-table>
            </v-card-text>
          </v-card>
  
          <!-- Voting Interface -->
          <v-card v-if="activeTab === 'voting'" class="mb-6 rounded-lg shadow-md">
            <v-card-title class="text-h5 font-medium">
              Vote: {{ selectedElection ? selectedElection.name : 'Select an Election' }}
              <v-btn
                icon
                class="ml-auto"
                @click="activeTab = 'elections'"
              >
              </v-btn>
            </v-card-title>
            <v-card-text v-if="selectedElection">
              <div class="text-gray-600 dark:text-gray-300 mb-4">
                Please select a candidate to cast your vote.
              </div>
              <v-radio-group v-model="selectedCandidate">
                <v-card
                  v-for="candidate in candidates"
                  :key="candidate.id"
                  class="mb-3 p-4 hover:bg-gray-50 :hover:bg-gray-800 transition-colors cursor-pointer"
                  :class="{ 'border-2 border-primary': selectedCandidate === candidate.id }"
                  @click="selectedCandidate = candidate.id"
                >
                  <div class="flex items-center">
                    <v-avatar size="48" class="mr-4">
                      <v-img :src="candidate.avatar"></v-img>
                    </v-avatar>
                    <div>
                      <div class="text-h6">{{ candidate.name }}</div>
                      <div class="flex items-center">
                        <v-chip
                          small
                          :color="candidate.partyColor"
                          class="mr-2"
                        >
                          {{ candidate.party }}
                        </v-chip>
                        <span class="text-sm text-gray-500 dark:text-gray-400">{{ candidate.position }}</span>
                      </div>
                    </div>
                    <v-radio
                      :value="candidate.id"
                      class="ml-auto"
                    ></v-radio>
                  </div>
                </v-card>
              </v-radio-group>
              <div class="flex justify-end mt-4">
                <v-btn
                  color="primary"
                  :disabled="!selectedCandidate"
                  @click="castVote"
                >
                  Cast Vote
                </v-btn>
              </div>
            </v-card-text>
            <v-card-text v-else>
              <div class="text-center py-8">
                <FileCheck class="text-gray-400" />
                <div class="text-h6 mt-4">No Election Selected</div>
                <div class="text-body-1 text-gray-600 dark:text-gray-400 mt-2">
                  Please go back to the elections list and select an election to vote in.
                </div>
                <v-btn
                  color="primary"
                  class="mt-4"
                  @click="activeTab = 'elections'"
                >
                  View Elections
                </v-btn>
              </div>
            </v-card-text>
          </v-card>
  
          <!-- Voting History -->
          <v-card v-if="activeTab === 'history'" class="mb-6 rounded-lg shadow-md">
            <v-card-title class="text-h5 font-medium">
              Your Voting History
            </v-card-title>
            <v-card-text>
              <v-timeline v-if="votingHistory.length > 0" align-top dense>
                <v-timeline-item
                  v-for="(history, index) in votingHistory"
                  :key="index"
                  :color="history.color"
                  small
                >
                  <div class="font-medium">{{ history.election }}</div>
                  <div class="text-sm text-gray-600 dark:text-gray-400">
                    {{ history.date }}
                  </div>
                  <div class="mt-2 flex items-center">
                    <v-avatar size="24" class="mr-2">
                      <v-img :src="history.candidateAvatar"></v-img>
                    </v-avatar>
                    <span>{{ history.candidate }}</span>
                    <v-chip
                      x-small
                      :color="history.partyColor"
                      class="ml-2"
                    >
                      {{ history.party }}
                    </v-chip>
                  </div>
                </v-timeline-item>
              </v-timeline>
              <div v-else class="text-center py-8">
                <History class="text-gray-400" />
                <div class="text-h6 mt-4">No Voting History</div>
                <div class="text-body-1 text-gray-600 dark:text-gray-400 mt-2">
                  You haven't voted in any elections yet.
                </div>
                <v-btn
                  color="primary"
                  class="mt-4"
                  @click="activeTab = 'elections'"
                >
                  View Elections
                </v-btn>
              </div>
            </v-card-text>
          </v-card>
  
          <!-- Success Dialog -->
          <v-dialog v-model="showSuccessDialog" max-width="400">
            <v-card>
              <v-card-title class="text-h5 bg-success text-white">
                Vote Cast Successfully
              </v-card-title>
              <v-card-text class="pt-4">
                <div class="text-center">
                  <CheckCircle  class="text-green-500" />
                  <div class="text-h6 mt-2">Thank you for voting!</div>
                  <div class="text-body-1 mt-2">
                    Your vote has been recorded successfully.
                  </div>
                </div>
              </v-card-text>
              <v-card-actions>
                <v-spacer></v-spacer>
                <v-btn
                  color="primary"
                  
                  @click="showSuccessDialog = false; activeTab = 'elections'"
                >
                  Back to Elections
                </v-btn>
                <v-btn
                  color="primary"
                  @click="showSuccessDialog = false; activeTab = 'history'"
                >
                  View History
                </v-btn>
              </v-card-actions>
            </v-card>
          </v-dialog>
        </v-container>
      </v-main>
    </v-app>
  </template>
  
  <script setup lang="ts">
  import { ref, onMounted} from 'vue';
  import { History, CheckCircle, FileCheck } from 'lucide-vue-next';
  import ElectionService from '../service/electionService';

  interface Election {
  id: number;
  name: string;
  startDate: string;
  endDate: string;
  totalVotes: number;
  status: string;  
  enrolled?: boolean; 
  voted?: boolean; 
}
const electionService = new ElectionService();
const ongoingElections = ref<Election[]>([]);

  
  // Navigation state
  const activeTab = ref('elections');
  const selectedElection = ref();
  const selectedCandidate = ref();
  const showSuccessDialog = ref(false);
  
  const elections = ref<Election[]>([]);

  
  // Candidates data
  const candidates = ref([
    {
      id: 1,
      name: 'Jane Smith',
      party: 'Progressive Party',
      partyColor: 'blue',
      position: 'Current Senator',
      avatar: 'https://randomuser.me/api/portraits/women/44.jpg'
    },
    {
      id: 2,
      name: 'John Davis',
      party: 'Conservative Party',
      partyColor: 'red',
      position: 'Former Governor',
      avatar: 'https://randomuser.me/api/portraits/men/32.jpg'
    },
    {
      id: 3,
      name: 'Maria Rodriguez',
      party: 'Liberty Party',
      partyColor: 'amber',
      position: 'State Representative',
      avatar: 'https://randomuser.me/api/portraits/women/68.jpg'
    },
    {
      id: 4,
      name: 'Robert Chen',
      party: 'Green Party',
      partyColor: 'green',
      position: 'Environmental Activist',
      avatar: 'https://randomuser.me/api/portraits/men/75.jpg'
    }
  ]);
  
  const fetchActiveElections = async () => {
  try {
    const response = await electionService.getActiveElection();

    ongoingElections.value = response.map((election: any) => {
      const formatDate = (dateString: string) => {
        const date = new Date(dateString);
        return date.toISOString().split('T')[0];
      };

      return {
        id: election.id,
        name: election.name,
        startDate: formatDate(election.start_date),
        endDate: formatDate(election.end_date)
      };
    });
  } catch (error) {
    console.error('Error fetching ongoing elections:', error);
  }
};
  // Voting history
  const votingHistory = ref([
    {
      election: 'Midterm Elections 2022',
      date: 'November 8, 2022',
      candidate: 'Sarah Johnson',
      party: 'Progressive Party',
      partyColor: 'blue',
      candidateAvatar: 'https://randomuser.me/api/portraits/women/22.jpg',
      color: 'blue'
    },
    {
      election: 'Local Referendum',
      date: 'June 15, 2022',
      candidate: 'Proposition 42',
      party: 'N/A',
      partyColor: 'grey',
      candidateAvatar: 'https://via.placeholder.com/150?text=P42',
      color: 'grey'
    },
    {
      election: 'Presidential Election 2020',
      date: 'November 3, 2020',
      candidate: 'Michael Williams',
      party: 'Liberty Party',
      partyColor: 'amber',
      candidateAvatar: 'https://randomuser.me/api/portraits/men/42.jpg',
      color: 'amber'
    }
  ]);

  // Table headers
  const electionHeaders = ref([
  { title: 'Election Name', value: 'name' },
  { title: 'Start Date', value: 'startDate' },
  { title: 'End Date', value: 'endDate' },
  { title: 'Actions', value: 'actions', sortable: false }
]);

  
  // Helper functions
  const getStatusColor = (status:string) => {
    switch (status) {
      case 'Open':
        return 'green';
      case 'Upcoming':
        return 'blue';
      case 'Closed':
        return 'grey';
      default:
        return 'grey';
    }
  };
  
  const enrollInElection = (election:Election) => {
  const index = elections.value.findIndex((e: Election) => e.id === election.id);
  if (index !== -1) {
    elections.value[index].enrolled = true;
  }
};

const castVote = () => {
  if (selectedElection.value && selectedCandidate.value) {
    // Update election status
    const electionIndex = ongoingElections.value.findIndex((e: Election) => e.id === selectedElection.value.id);
    if (electionIndex !== -1) {
      ongoingElections.value[electionIndex].voted = true;
    }

    const candidate = candidates.value.find(c => c.id === selectedCandidate.value);
    if (candidate) {
      const today = new Date();
      const formattedDate = today.toLocaleDateString('en-US', {
        year: 'numeric',
        month: 'long',
        day: 'numeric'
      });

      votingHistory.value.unshift({
        election: selectedElection.value.name,
        date: formattedDate,
        candidate: candidate.name,
        party: candidate.party,
        partyColor: candidate.partyColor,
        candidateAvatar: candidate.avatar,
        color: candidate.partyColor
      });
    }

    // Show success dialog
    showSuccessDialog.value = true;
  }
};


  onMounted(() => {
    fetchActiveElections();
  });
  </script>
  
  <style>
  .v-timeline-item__body {
    margin-bottom: 12px;
  }
  
  .v-card {
    transition: all 0.3s ease;
  }
  
  .v-card:hover {
    transform: translateY(-2px);
  }
  
  .v-timeline-item__dot--small {
    margin-inline-end: 8px;
  }
  </style>