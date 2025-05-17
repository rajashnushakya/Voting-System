<script setup lang="ts">
import { ref, onMounted } from 'vue'
import MenuComponent from './childcomponents/MenuComponent.vue'
import { VoteIcon, UsersIcon, CheckSquareIcon, FileTextIcon, UserIcon } from 'lucide-vue-next'
import ElectionService from '../service/electionService'
import voterService from '../service/voterService'
import { Bar } from 'vue-chartjs'
import { Chart as ChartJS, CategoryScale, LinearScale, BarElement, Title, Tooltip, Legend } from 'chart.js'
import candidateService from '../service/candidateService'

// Register Chart.js components
ChartJS.register(CategoryScale, LinearScale, BarElement, Title, Tooltip, Legend)

const service = new candidateService();
const electionService = new ElectionService();
const VoterService = new voterService();

// Defining a consistent color palette for all charts
const chartColors = {
  backgroundColor: [
    '#4F46E5', // Indigo
    '#7C3AED', // Purple
    '#EC4899', // Pink
    '#F59E0B', // Amber
    '#10B981', // Emerald
    '#3B82F6', // Blue
    '#F43F5E', // Rose
    '#8B5CF6'  // Violet
  ],
  borderColor: [
    '#4338CA', // Indigo (darker)
    '#6D28D9', // Purple (darker)
    '#DB2777', // Pink (darker)
    '#D97706', // Amber (darker)
    '#059669', // Emerald (darker)
    '#2563EB', // Blue (darker)
    '#E11D48', // Rose (darker)
    '#7C3AED'  // Violet (darker)
  ]
};

// Function to get colors based on data length
const getChartColors = (dataLength: number) => {
  const backgroundColors = [];
  const borderColors = [];
  
  for (let i = 0; i < dataLength; i++) {
    // Use modulo to cycle through colors if there are more data points than colors
    const colorIndex = i % chartColors.backgroundColor.length;
    backgroundColors.push(chartColors.backgroundColor[colorIndex]);
    borderColors.push(chartColors.borderColor[colorIndex]);
  }
  
  return {
    backgroundColor: backgroundColors,
    borderColor: borderColors
  };
};

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

interface CandidateData {
  candidateId: number;
  fullName: string;
  voteCount: number;
  electionName: string;
}

// Define chart data interface to fix the 'never' type issues
interface ChartData {
  labels: string[];
  datasets: {
    label: string;
    data: number[];
    backgroundColor: string[];
    borderColor: string[];
    borderWidth: number;
  }[];
}

const quickStats = ref<QuickStat[]>([
  { title: 'Number of Elections', value:100 , icon: VoteIcon },
  { title: 'Number of Voters', value: 1250, icon: UsersIcon },
  { title: 'Votes Cast', value: 875, icon: CheckSquareIcon },
])

// const recentActivities = ref<RecentActivity[]>([
//   { id: 1, description: 'Election XYZ created', timestamp: '2 hours ago', icon: FileTextIcon },
//   { id: 2, description: '50 votes cast in Election ABC', timestamp: '4 hours ago', icon: CheckSquareIcon },
//   { id: 3, description: 'New voter registered', timestamp: '1 day ago', icon: UserIcon },
// ])

// Track which election's menu is open by ID
const openMenuId = ref<number | null>(null);
// Store menu position
const menuPosition = ref({ top: 0, left: 0 });

// Initialize empty chart data with proper typing
const chartData = ref<ChartData>({
  labels: [],
  datasets: [
    {
      label: 'Votes',
      data: [],
      backgroundColor: [],
      borderColor: [],
      borderWidth: 1
    }
  ]
});

const chartOptions = ref({
  responsive: true,
  maintainAspectRatio: false,
  plugins: {
    legend: {
      display: false
    },
    title: {
      display: true,
      text: 'Top Candidates by Votes',
      font: {
        size: 16,
        weight: 'bold' as const 
      }
    },
    tooltip: {
      callbacks: {
        label: function(context: any) {
          const label = context.dataset.label || '';
          const value = context.parsed.y || 0;
          const total = context.dataset.data.reduce((a: number, b: number) => a + b, 0);
          const percentage = total > 0 ? ((value / total) * 100).toFixed(2) : '0.00';
          return `${label}: ${value} (${percentage}%)`;
        }
      }
    }
  },
  scales: {
    y: {
      beginAtZero: true,
      title: {
        display: true,
        text: 'Number of Votes'
      }
    },
    x: {
      title: {
        display: true,
        text: 'Candidates'
      }
    }
  }
});

// Store candidate data
const candidateData = ref<CandidateData[]>([]);

// Fetch candidate data from API
const fetchCandidateCount = async () => {
  try {
    const response = await service.getTopCandidate();
    console.log('Top candidates:', response);
    candidateData.value = response;
    updateChartFromAPI(response);
  } catch (error) {
    console.error('Error fetching candidate count:', error);
  }
};

// Function to create chart data from API response
const updateChartFromAPI = (apiData: CandidateData[]) => {
  if (!apiData || apiData.length === 0) {
    // Handle empty data
    chartData.value = {
      labels: ['No data available'],
      datasets: [
        {
          label: 'Votes',
          data: [0],
          backgroundColor: ['#e5e7eb'],
          borderColor: ['#d1d5db'],
          borderWidth: 1
        }
      ]
    };
    return;
  }
  
  const labels = apiData.map((candidate: CandidateData) => candidate.fullName);
  const data = apiData.map((candidate: CandidateData) => candidate.voteCount);
  const colors = getChartColors(labels.length);
  
  chartData.value = {
    labels: labels,
    datasets: [
      {
        label: 'Votes',
        data: data,
        backgroundColor: colors.backgroundColor,
        borderColor: colors.borderColor,
        borderWidth: 1
      }
    ]
  };
};

const toggleMenu = (electionId: number, event: MouseEvent) => {
  // Get the button position
  const button = event.currentTarget as HTMLElement;
  const rect = button.getBoundingClientRect();
  
  // Set the menu position
  menuPosition.value = {
    top: rect.bottom + window.scrollY,
    left: rect.left + window.scrollX
  };
  
  if (openMenuId.value === electionId) {
    openMenuId.value = null; // Close if already open
  } else {
    openMenuId.value = electionId; // Open this election's menu
  }
};

const startElection = async (electionId: number) => {
  try {
    await electionService.startElection(electionId);
    alert(`Election started successfully !!`);
    fetchActiveElections();
  } catch (error) {
    console.error("Error starting election:", error);
    alert(`Failed to start election `);
  }
};

const ongoingElections = ref<Election[]>([]);

const fetchActiveElections = async () => {
  try {
    const response = await electionService.getActiveElection();

    ongoingElections.value = response.map((election: any) => {
      const formatDateTime = (dateString: string) => {
        const date = new Date(dateString);
        const year = date.getFullYear();
        const month = String(date.getMonth() + 1).padStart(2, "0");
        const day = String(date.getDate()).padStart(2, "0");
        const hours = String(date.getHours()).padStart(2, "0");
        const minutes = String(date.getMinutes()).padStart(2, "0");
        return `${year}-${month}-${day} ${hours}:${minutes}`; 
      };

      return {
        id: election.id,
        name: election.name,
        startDate: formatDateTime(election.start_date),
        endDate: formatDateTime(election.end_date),
        totalVotes: election.total_votes
      };
    });
  } catch (error) {
    console.error("Error fetching ongoing elections:", error);
  }
};

const endElection = async (id: number) => {
  try {
    openMenuId.value = null;
    const response = await electionService.endElection(id);

    if (response) {
      fetchActiveElections();
      // Refresh candidate data after ending an election
      fetchCandidateCount();
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

// Close menu when clicking outside
const closeMenu = () => {
  openMenuId.value = null;
};

onMounted(() => {
  fetchActiveElections();
  fetchElectionCount();
  fetchVoterCount();
  fetchCandidateCount();
  
  // Add event listener to close menu when clicking outside
  document.addEventListener('click', (event) => {
    const target = event.target as HTMLElement;
    if (!target.closest('.menu-button') && !target.closest('.menu-dropdown')) {
      closeMenu();
    }
  });
});
</script>

<template>
  <div class="min-h-screen bg-gray-100">
    <MenuComponent></MenuComponent>
    <div class="p-8">
      <h1 class="text-4xl font-bold text-gray-800 mb-8">Admin Dashboard</h1>

      <!-- Quick Stats Section -->
      <section class="grid grid-cols-1 md:grid-cols-3 gap-6 mb-8">
        <div v-for="stat in quickStats" :key="stat.title" class="bg-white rounded-lg shadow p-6 flex items-center">
          <component :is="stat.icon" class="w-12 h-12 text-blue-500 mr-4" />
          <div>
            <h2 class="text-3xl font-semibold text-gray-800">{{ stat.title }}</h2>
            <p class="text-3xl font-bold text-gray-900">{{ stat.value }}</p>
          </div>
        </div>
      </section>

      <!-- Recent Activities Section
      <section class="bg-white rounded-lg shadow p-6 mb-8">
        <h2 class="text-2xl font-semibold text-gray-800 mb-4">Recent Activities</h2>
        <ul class="space-y-4">
          <li v-for="activity in recentActivities" :key="activity.id" class="flex items-center">
            <component :is="activity.icon" class="w-5 h-5 text-gray-500 mr-3" />
            <span class="text-gray-700">{{ activity.description }}</span>
            <span class="ml-auto text-sm text-gray-500">{{ activity.timestamp }}</span>
          </li>
        </ul>
      </section> -->

      <!-- Election Summary Section -->
      <section class="bg-white rounded-lg shadow p-6 mb-8">
        <h2 class="text-3xl font-semibold text-gray-800 mb-4">Ongoing Elections</h2>
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
                  <button 
                    @click="toggleMenu(election.id, $event)" 
                    class="p-3 text-gray-600 hover:text-black menu-button"
                  >
                    ⋮
                  </button>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </section>

      <!-- Results Section with Single Chart -->
      <section class="bg-white rounded-lg shadow p-6">
        <div class="flex flex-col md:flex-row justify-between items-start md:items-center mb-4">
          <h2 class="text-3xl font-semibold text-gray-800">Top Candidates by Votes</h2>
        </div>
        
        <div v-if="candidateData.length === 0" class="text-center py-10 text-gray-500">
          No candidate data available. Please check if there are any active elections with votes.
        </div>
        
        <div v-else class="h-80">
          <Bar 
            :data="chartData" 
            :options="chartOptions" 
          />
        </div>
        
        <div v-if="candidateData.length > 0" class="mt-4 text-sm text-gray-500 text-right">
          Data from: {{ candidateData[0]?.electionName || 'Unknown Election' }}
        </div>
      </section>
    </div>
    
    <!-- Dropdown menu positioned absolutely in the document -->
    <div 
      v-if="openMenuId !== null"
      class="fixed w-40 rounded-md shadow-lg bg-white ring-1 ring-black ring-opacity-5 z-50 menu-dropdown"
      :style="{
        top: menuPosition.top + 'px',
        left: menuPosition.left + 'px'
      }"
    >
      <div class="py-1">
        <button 
          @click="startElection(openMenuId!)" 
          class="block px-4 py-2 text-sm text-green-700 hover:bg-green-100 w-full text-left"
        >
          Start Election
        </button>
        <button 
          @click="endElection(openMenuId!)" 
          class="block px-4 py-2 text-sm text-red-600 hover:bg-red-100 w-full text-left"
        >
          End Election
        </button>
      </div>
    </div>
  </div>
</template>