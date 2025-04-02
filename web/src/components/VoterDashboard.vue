<template>
  <v-app>
    <v-app-bar style="background-color: #003893;">
      <v-btn @click="activeTab = 'elections'" style="color: white;">Elections</v-btn>
      <v-btn @click="activeTab = 'enrollment'" style="color: white;">Enrollment</v-btn>
      <v-btn @click="activeTab = 'voting'" style="color: white;">Vote</v-btn>
      <v-btn @click="activeTab = 'history'" style="color: white;">History</v-btn>
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
                  @click="activeTab = 'enrollment'; selectedElection = item"
                  :disabled="item.enrolled"
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

        <!-- Enrollment Interface -->
        <ElectionCenterEnrollment 
          v-if="activeTab === 'enrollment'"
          :election-id="selectedElection ? selectedElection.id : ''"
          @update:active-tab="activeTab = $event"
          @enrollment-complete="handleEnrollmentComplete"
        />

        <!-- Voting Interface -->
        <VotingInterface
          v-if="activeTab === 'voting'"
          :selected-election="selectedElection"
          @update:active-tab="activeTab = $event"
          @vote-cast="handleVoteCast"
        />

        <!-- Voting History -->
        <VotingHistory
          v-if="activeTab === 'history'"
          :voting-history="votingHistory"
          @update:active-tab="activeTab = $event"
        />

        <!-- Success Dialog -->
        <v-dialog v-model="showSuccessDialog" max-width="400">
          <v-card>
            <v-card-title class="text-h5 bg-success text-white">
              Vote Cast Successfully
            </v-card-title>
            <v-card-text class="pt-4">
              <div class="text-center">
                <v-icon size="x-large" color="success">mdi-check-circle</v-icon>
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
import { ref, onMounted, computed } from 'vue';
import { useRoute } from 'vue-router';
import ElectionService from '../service/electionService';
import ElectionCenterEnrollment from './ElectionCenterEnrollment.vue';
import VotingInterface from './VotingInterface.vue';
import VotingHistory from './VotingHistory.vue';

const route = useRoute();

interface Election {
  id: number;
  name: string;
  startDate: string;
  endDate: string;
  totalVotes: number;
  status: string;  
  enrolled?: boolean; 
  voted?: boolean;
  electionCenter?: string;
}

const electionService = new ElectionService();
const ongoingElections = ref<Election[]>([]);

// Navigation state
const activeTab = ref('elections');
const selectedElection = ref<Election | null>(null);
const showSuccessDialog = ref(false);

// Voting history with enhanced data
const votingHistory = ref([
  {
    election: 'Presidential Election 2020',
    date: 'November 3, 2020',
    candidate: 'Michael Williams',
    party: 'Liberty Party',
    partyColor: 'amber',
    candidateAvatar: 'https://randomuser.me/api/portraits/men/42.jpg',
    color: 'amber'
  },
  {
    election: 'Local City Council Election 2022',
    date: 'March 15, 2022',
    candidate: 'Jasmine Williams',
    party: 'Community First',
    partyColor: 'purple',
    candidateAvatar: 'https://randomuser.me/api/portraits/women/22.jpg',
    color: 'purple'
  },
  {
    election: 'State Referendum 2022',
    date: 'June 15, 2022',
    candidate: 'Proposition A: Infrastructure Bond',
    party: 'N/A',
    partyColor: 'grey',
    candidateAvatar: 'https://via.placeholder.com/150?text=Prop+A',
    color: 'grey'
  },
  {
    election: 'Midterm Elections 2022',
    date: 'November 8, 2022',
    candidate: 'Sarah Johnson',
    party: 'Progressive Party',
    partyColor: 'blue',
    candidateAvatar: 'https://randomuser.me/api/portraits/women/23.jpg',
    color: 'blue'
  },
  {
    election: 'School Board Election 2023',
    date: 'April 4, 2023',
    candidate: 'David Chen',
    party: 'Education First',
    partyColor: 'teal',
    candidateAvatar: 'https://randomuser.me/api/portraits/men/67.jpg',
    color: 'teal'
  },
  {
    election: 'Special Election: Transit Funding 2023',
    date: 'September 12, 2023',
    candidate: 'Proposition B: Transit Expansion',
    party: 'N/A',
    partyColor: 'grey',
    candidateAvatar: 'https://via.placeholder.com/150?text=Prop+B',
    color: 'orange'
  }
]);

// Table headers
const electionHeaders = ref([
  { title: 'Election Name', value: 'name' },
  { title: 'Start Date', value: 'startDate' },
  { title: 'End Date', value: 'endDate' },
  { title: 'Status', value: 'status' },
  { title: 'Actions', value: 'actions', sortable: false }
]);

// Helper functions
const getStatusColor = (status: string) => {
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
        endDate: formatDate(election.end_date),
        status: 'Open', // You might want to calculate this based on dates
        enrolled: false,
        voted: false
      };
    });
  } catch (error) {
    console.error('Error fetching ongoing elections:', error);
    
    // Fallback mock data if API fails
    ongoingElections.value = [
      {
        id: 1,
        name: "2024 Presidential Election",
        startDate: "2024-11-01",
        endDate: "2024-11-08",
        totalVotes: 0,
        status: "Upcoming",
        enrolled: false,
        voted: false
      },
      {
        id: 2,
        name: "Local City Council Election",
        startDate: "2024-06-01",
        endDate: "2024-06-15",
        totalVotes: 0,
        status: "Open",
        enrolled: false,
        voted: false
      },
      {
        id: 3,
        name: "State Referendum",
        startDate: "2024-07-10",
        endDate: "2024-07-24",
        totalVotes: 0,
        status: "Upcoming",
        enrolled: false,
        voted: false
      }
    ];
  }
};

const handleEnrollmentComplete = (data: { electionId: number, centerId: string }) => {
  // Update the election status to enrolled
  const index = ongoingElections.value.findIndex(e => e.id === data.electionId);
  if (index !== -1) {
    ongoingElections.value[index].enrolled = true;
    ongoingElections.value[index].electionCenter = data.centerId;
  }
  
  // Switch back to elections tab
  activeTab.value = 'elections';
};

const handleVoteCast = (voteData: any) => {
  // Update election status
  const electionIndex = ongoingElections.value.findIndex(e => e.id === voteData.electionId);
  if (electionIndex !== -1) {
    ongoingElections.value[electionIndex].voted = true;
  }

  // Add to voting history
  const election = ongoingElections.value.find(e => e.id === voteData.electionId);
  if (election) {
    const today = new Date();
    const formattedDate = today.toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'long',
      day: 'numeric'
    });

    votingHistory.value.unshift({
      election: election.name,
      date: formattedDate,
      candidate: voteData.candidateName,
      party: voteData.candidateParty,
      partyColor: voteData.candidatePartyColor,
      candidateAvatar: voteData.candidateAvatar,
      color: voteData.candidatePartyColor
    });
  }

  // Show success dialog
  showSuccessDialog.value = true;
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

