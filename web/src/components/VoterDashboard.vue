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
                    :disabled="item.enrolled"
                    @click="navigateToEnrollment(item.id)"
                  >
                    {{ item.enrolled ? 'Enrolled' : 'Enroll' }}
                  </v-btn>

                  <v-btn
  small
  color="success"
  class="ml-2"
  :disabled="!item.enrolled || item.status !== 'Open' || item.voted"
  @click="goToVoting(item)"
>
  Vote
</v-btn>


              </template>
            </v-data-table>
          </v-card-text>
        </v-card>



      </v-container>
    </v-main>
  </v-app>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import ElectionService from '../service/electionService';


const router = useRouter();

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
  // Add other history entries as needed
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
const goToVoting = (election: any) => {
  selectedElection.value = election;
  activeTab.value = 'voting'; // Optional if you're using tabs

  // Navigate to a route
  router.push({
    name: 'vote', // 👈 Your route name from router/index.js
  });
};

const fetchActiveElections = async () => {
  try {
    const response = await electionService.getActiveElection();

    const enrolledIds = JSON.parse(localStorage.getItem('enrolledElections') || '[]');

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
        status: 'Open', // or compute based on date
        enrolled: enrolledIds.includes(election.id),
        voted: false // you could persist this too if needed
      };
    });

  } catch (error) {
    console.error('Error fetching ongoing elections:', error);
  }
};


const navigateToEnrollment = async (electionId: number) => {
  try {
    const voterId = localStorage.getItem('voterid');

    if (!voterId) {
      alert("Voter ID not found. Please log in again.");
      return;
    }

    await electionService.enrollVoter(voterId, electionId);

    const election = ongoingElections.value.find(e => e.id === electionId);
    if (election) {
      election.enrolled = true;

      // 👇 Save to localStorage
      let enrolled = JSON.parse(localStorage.getItem('enrolledElections') || '[]');
      if (!enrolled.includes(electionId)) {
        enrolled.push(electionId);
        localStorage.setItem('enrolledElections', JSON.stringify(enrolled));
      }
    }

    alert('Enrollment successful!');

    // Go to the enrollment center page
    router.push({ name: 'voter-enroll', params: { electionId } });

  } catch (error) {
    console.error('Enrollment error:', error);
    alert('Failed to enroll. Please try again.');
  }
};





onMounted(() => {
  fetchActiveElections();
});
</script>

<style scoped>
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
