<script setup lang="ts">
import { ref, onMounted } from 'vue'
import MenuComponent from './childcomponents/MenuComponent.vue'
import { VoteIcon, UsersIcon, CheckSquareIcon, FileTextIcon, UserIcon } from 'lucide-vue-next'
import ElectionService from '../service/electionService'
import voterService from '../service/voterService'


interface QuickStat {
  title: string;
  value: number;
  icon: any;
}

interface RecentActivity {
  id: number;
  description: string;
  timestamp: string;
  icon: any;
}
interface Election {
  id: number;
  name: string;
  startDate: string; 
  endDate: string;    
  totalVotes: number;
}

interface ElectionResult {
  id: number;
  name: string;
}

const quickStats = ref<QuickStat[]>([
  { title: 'Number of Elections', value:100 , icon: VoteIcon },
  { title: 'Number of Voters', value: 1250, icon: UsersIcon },
  { title: 'Votes Cast', value: 875, icon: CheckSquareIcon },
])

const recentActivities = ref<RecentActivity[]>([
  { id: 1, description: 'Election XYZ created', timestamp: '2 hours ago', icon: FileTextIcon },
  { id: 2, description: '50 votes cast in Election ABC', timestamp: '4 hours ago', icon: CheckSquareIcon },
  { id: 3, description: 'New voter registered', timestamp: '1 day ago', icon: UserIcon },
])


const ongoingElections = ref<Election[]>([]);

const electionService = new ElectionService();
const VoterService = new voterService();
const fetchActiveElections = async () => {
  try {
    const response = await electionService.getActiveElection();

    ongoingElections.value = response.map((election: any) => {
      const formatDateTime = (dateString: string) => {
        const date = new Date(dateString);
        return date.toISOString().slice(0, 16); // Returns "yyyy-mm-ddThh:mm"
      };

      return {
        id: election.id,
        name: election.name,
        startDate: formatDateTime(election.start_date),  // Formatted as "yyyy-mm-ddThh:mm"
        endDate: formatDateTime(election.end_date),    // Formatted as "yyyy-mm-ddThh:mm"
        totalVotes: election.total_votes
      };
    });
  } catch (error) {
    console.error('Error fetching ongoing elections:', error);
  }
};


const endElection = async (id: number) => {
  try {
    const response = await electionService.endElection(id);

    if (response) {
      console.log('End election response:', response, id);

    } else {
      console.error('Election ending failed:', response);
    }

    return response; 
  } catch (error) {
    console.error('Error ending election:', error);
    alert('Failed to end the election. Please try again.');
  }
};


const fetchElectionCount = async () => {
  try {
    const response = await electionService.getElectionCount();
    quickStats.value[0].value = response;
  } catch (error) {
    console.error('Error fetching election count:', error);
  }
}

const fetchVoterCount = async () => {
  try {
    const response = await VoterService.getVoterCount();
    quickStats.value[1].value = response;
  } catch (error) {
    console.error('Error fetching voter count:', error);
  }
}

onMounted(() => {
  fetchActiveElections();
  fetchElectionCount();
  fetchVoterCount();
});

const electionResults = ref<ElectionResult[]>([
  { id: 1, name: 'School Board Election' },
  { id: 2, name: 'Local Community Council' },
])


const viewDetailedResults = (resultId: number) => {
  // Implement view detailed results logic
  console.log('View detailed results clicked for', resultId)
}



</script>
<template>
  <div class="min-h-screen bg-gray-100">
    <MenuComponent></MenuComponent>
    <div class="p-8">
      <h1 class="text-3xl font-bold text-gray-800 mb-8">Admin Dashboard</h1>

      <!-- Quick Stats Section -->
      <section class="grid grid-cols-1 md:grid-cols-3 gap-6 mb-8">
        <div v-for="stat in quickStats" :key="stat.title" class="bg-white rounded-lg shadow p-6 flex items-center">
          <component :is="stat.icon" class="w-12 h-12 text-blue-500 mr-4" />
          <div>
            <h2 class="text-xl font-semibold text-gray-700">{{ stat.title }}</h2>
            <p class="text-3xl font-bold text-gray-900">{{ stat.value }}</p>
          </div>
        </div>
      </section>



      <!-- Recent Activities Section -->
      <section class="bg-white rounded-lg shadow p-6 mb-8">
        <h2 class="text-2xl font-semibold text-gray-800 mb-4">Recent Activities</h2>
        <ul class="space-y-4">
          <li v-for="activity in recentActivities" :key="activity.id" class="flex items-center">
            <component :is="activity.icon" class="w-5 h-5 text-gray-500 mr-3" />
            <span class="text-gray-700">{{ activity.description }}</span>
            <span class="ml-auto text-sm text-gray-500">{{ activity.timestamp }}</span>
          </li>
        </ul>
      </section>

      <!-- Election Summary Section -->
      <section class="bg-white rounded-lg shadow p-6 mb-8">
        <h2 class="text-2xl font-semibold text-gray-800 mb-4">Ongoing Elections</h2>
        <div class="overflow-x-auto">
          <table class="min-w-full">
            <thead>
              <tr class="bg-gray-50">
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Election Name</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Start Date</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">End Date</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Actions</th>
              </tr>
            </thead>
            <tbody class="bg-white divide-y divide-gray-200">
              <tr v-for="election in ongoingElections" :key="election.id">
                <td class="px-6 py-4 whitespace-nowrap">{{ election.name }}</td>
                <td class="px-6 py-4 whitespace-nowrap">{{ election.startDate }}</td>
                <td class="px-6 py-4 whitespace-nowrap">{{ election.endDate }}</td>
                <td class="px-6 py-4 whitespace-nowrap space-x-2">
                  <button @click="endElection(election.id)" class="bg-red-500 hover:bg-red-600 text-white font-bold py-2 px-4 rounded text-sm">
                    End Election
                  </button>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </section>

      <!-- Results Section -->
      <section v-if="electionResults.length > 0" class="bg-white rounded-lg shadow p-6">
        <h2 class="text-2xl font-semibold text-gray-800 mb-4">Election Results</h2>
        <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
          <div v-for="result in electionResults" :key="result.id" class="bg-gray-50 p-4 rounded-lg">
            <h3 class="text-lg font-semibold mb-2">{{ result.name }}</h3>
            <div class="h-64">
              <!-- Placeholder for chart -->
              <div class="bg-gray-200 h-full w-full flex items-center justify-center text-gray-500">
                Chart Placeholder
              </div>
            </div>
            <button @click="viewDetailedResults(result.id)" class="mt-4 bg-blue-500 hover:bg-blue-600 text-white font-bold py-2 px-4 rounded text-sm">
              View Detailed Results
            </button>
          </div>
        </div>
      </section>
    </div>
  </div>
</template>
