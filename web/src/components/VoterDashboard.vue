<template>
    <v-app>
      <v-app-bar style="background-color: #003893;" >
        <v-btn text @click="activeTab = 'elections'" style="color: white;">Elections</v-btn>
<v-btn text @click="activeTab = 'voting'" style="color: white;">Vote</v-btn>
<v-btn text @click="activeTab = 'history'" style="color: white;">History</v-btn>

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
                :items="elections"
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
                    :disabled="item.status !== 'Open' || item.enrolled"
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
                <ArrowLeft size="24" />
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
                <FileCheck size="64" class="text-gray-400" />
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
                <History size="64" class="text-gray-400" />
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
                  <CheckCircle size="64" class="text-green-500" />
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
                  text
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
  
  <script setup>
  import { ref, computed } from 'vue';
  import { Sun, Moon, ArrowLeft, History, CheckCircle, FileCheck } from 'lucide-vue-next';
  
  // Theme state
  const isDark = ref(false);
  const toggleTheme = () => {
    isDark.value = !isDark.value;
    document.documentElement.classList.toggle('gray', isDark.value);
  };
  
  // Navigation state
  const activeTab = ref('elections');
  const selectedElection = ref(null);
  const selectedCandidate = ref(null);
  const showSuccessDialog = ref(false);
  
  // Elections data
  const elections = ref([
    {
      id: 1,
      name: 'Presidential Election 2024',
      date: '2024-11-05',
      status: 'Open',
      enrolled: false,
      voted: false,
      type: 'National'
    },
    {
      id: 2,
      name: 'State Governor Election',
      date: '2024-10-15',
      status: 'Open',
      enrolled: false,
      voted: false,
      type: 'State'
    },
    {
      id: 3,
      name: 'City Council Election',
      date: '2024-09-20',
      status: 'Open',
      enrolled: false,
      voted: false,
      type: 'Local'
    },
    {
      id: 4,
      name: 'School Board Election',
      date: '2024-08-10',
      status: 'Upcoming',
      enrolled: false,
      voted: false,
      type: 'Local'
    },
    {
      id: 5,
      name: 'Midterm Elections 2022',
      date: '2022-11-08',
      status: 'Closed',
      enrolled: true,
      voted: true,
      type: 'National'
    }
  ]);
  
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
    { text: 'Election Name', value: 'name' },
    { text: 'Date', value: 'date' },
    { text: 'Type', value: 'type' },
    { text: 'Status', value: 'status' },
    { text: 'Actions', value: 'actions', sortable: false }
  ]);
  
  // Helper functions
  const getStatusColor = (status) => {
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
  
  const enrollInElection = (election) => {
    const index = elections.value.findIndex(e => e.id === election.id);
    if (index !== -1) {
      elections.value[index].enrolled = true;
    }
  };
  
  const castVote = () => {
    if (selectedElection.value && selectedCandidate.value) {
      // Update election status
      const index = elections.value.findIndex(e => e.id === selectedElection.value.id);
      if (index !== -1) {
        elections.value[index].voted = true;
      }
  
      // Add to voting history
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
      
      // Reset selection
      selectedCandidate.value = null;
    }
  };
  </script>
  
  <style>
  /* Tailwind dark mode support */
  .dark {
    --v-theme-surface: #1e1e1e;
    --v-theme-background: #121212;
    color-scheme: dark;
  }
  
  /* Custom styles */
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