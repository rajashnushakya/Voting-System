<template>
  <div class="min-h-screen bg-gray-100">
    <MenuComponent></MenuComponent>
    <div class="p-8">
      <h1 class="text-3xl font-bold text-gray-800 mb-8">Election Results and Analysis</h1>

      <!-- Filters Section -->
      <section class="bg-white rounded-lg shadow p-6 mb-8">
        <h2 class="text-2xl font-semibold text-gray-800 mb-4">Filters</h2>
        <div class="grid grid-cols-1 md:grid-cols-3 gap-6">
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-2">Election</label>
            <select
              v-model="filters.election"
              class="w-full bg-gray-50 border border-gray-300 rounded-lg p-2 focus:ring-blue-500 focus:border-blue-500"
            >
              <option v-for="election in elections" :key="election" :value="election">
                {{ election }}
              </option>
            </select>
          </div>
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-2">Centres</label>
            <select
              v-model="filters.centre"
              :disabled="centres.length === 0"
              class="w-full bg-gray-50 border border-gray-300 rounded-lg p-2 focus:ring-blue-500 focus:border-blue-500 disabled:opacity-50"
            >
              <option v-for="centre in centres" :key="centre.id" :value="centre.name">
                {{ centre.name }}
              </option>
            </select>
          </div>
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-2">Search</label>
            <button
              @click="fetchResults"
              class="bg-blue-500 hover:bg-blue-600 text-white font-semibold py-2 px-4 rounded-lg"
            >
              Search
            </button>
          </div>
        </div>
      </section>

      <!-- Overall Results Section -->
      <section class="bg-white rounded-lg shadow p-6 mb-8">
        <h2 class="text-2xl font-semibold text-gray-800 mb-4">Overall Results</h2>
        <div class="overflow-x-auto">
          <table class="min-w-full bg-white border border-gray-200">
            <thead>
              <tr class="bg-gray-50">
                <th class="px-6 py-3 text-left text-sm font-semibold text-gray-600 uppercase">Candidate</th>
                <th class="px-6 py-3 text-left text-sm font-semibold text-gray-600 uppercase">Party</th>
                <th class="px-6 py-3 text-left text-sm font-semibold text-gray-600 uppercase">Votes</th>
                <th class="px-6 py-3 text-left text-sm font-semibold text-gray-600 uppercase">Percentage</th>
              </tr>
            </thead>
            <tbody>
              <tr
                v-for="item in filteredOverallResults"
                :key="item.name"
                :class="{ 'bg-green-100': item.isWinner, 'bg-white': !item.isWinner }"
                class="border-b hover:bg-gray-100"
              >
                <td class="px-6 py-4 text-gray-800">{{ item.name }}</td>
                <td class="px-6 py-4 text-gray-800">{{ item.party }}</td>
                <td class="px-6 py-4 text-gray-800">{{ item.votes }}</td>
                <td class="px-6 py-4 text-gray-800">{{ item.percentage.toFixed(2) }}%</td>
              </tr>
            </tbody>
          </table>
        </div>
      </section>

      <!-- Results Chart Section -->
      <section class="bg-white rounded-lg shadow p-6 mb-8">
        <h2 class="text-2xl font-semibold text-gray-800 mb-4">Results Visualization</h2>
        <div class="h-80">
          <Bar :data="chartData" :options="chartOptions" />
        </div>
      </section>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, watch, computed } from 'vue'
import MenuComponent from '../components/childcomponents/MenuComponent.vue'
import ElectionService from '../service/electionService'
import ElectionCentreService from '../service/electionCentreService'
import { Bar } from 'vue-chartjs'
import { Chart as ChartJS, CategoryScale, LinearScale, BarElement, Title, Tooltip, Legend } from 'chart.js'

// Register Chart.js components
ChartJS.register(CategoryScale, LinearScale, BarElement, Title, Tooltip, Legend)

// Define a consistent color palette for all charts
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

const Eservice = new ElectionService();
const ECservice = new ElectionCentreService();

const filters = ref({
  election: '',
  centre: '',
  party: '',
})

const elections = ref<string[]>([])   // List of election names
const centres = ref<{ id: string; name: string }[]>([]);  // List of centre names
const electionsData = ref<any[]>([])  // Store full elections with id and name

const overallResultsHeaders = [
  { text: 'Candidate', value: 'name' },
  { text: 'Party', value: 'party' },
  { text: 'Votes', value: 'votes' },
  { text: 'Percentage', value: 'percentage' },
]

const filteredOverallResults = ref([
  { name: '', party: '', votes: 0, percentage: 0 }
]);

// Chart data computed property that updates when filteredOverallResults changes
const chartData = computed(() => {
  const labels = filteredOverallResults.value.map(item => item.name);
  const data = filteredOverallResults.value.map(item => item.votes);
  const colors = getChartColors(labels.length);
  
  return {
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
});

// Chart options
const chartOptions = {
  responsive: true,
  maintainAspectRatio: false,
  plugins: {
    legend: {
      display: false
    },
    title: {
      display: true,
      text: 'Vote Distribution by Candidate',
      font: {
        size: 16,
        weight: 'bold'
      }
    },
    tooltip: {
      callbacks: {
        label: function(context: any) {
          const label = context.dataset.label || '';
          const value = context.parsed.y || 0;
          const total = context.dataset.data.reduce((a: number, b: number) => a + b, 0);
          const percentage = ((value / total) * 100).toFixed(2);
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
};

const fetchResults = async () => {
  if (filters.value.election && filters.value.centre) {
    try {
      console.log('Selected Centre from filters:', filters.value.centre);  // Debugging: Log selected centre value

      // Find the selected centre object from the centres list based on the name
      const selectedCentre = centres.value.find(centre => centre.name.trim() === filters.value.centre.trim());

      console.log('Selected Centre:', selectedCentre);  // Debugging: Log selected centre object

      if (!selectedCentre) {
        console.warn('Centre not found');
        return;
      }

      const centreId = selectedCentre.id;  // Use the 'id' of the selected centre
      const electionId = filters.value.election;

      // Debugging: Log the centreId being used
      console.log('Using Centre ID:', centreId);

      // Fetch the results using the ElectionCentreService
      const results = await ECservice.getCandidateVotes(centreId);

      // Debugging: Log the results returned
      console.log('Election Results:', results);

      // Update the filteredOverallResults
      filteredOverallResults.value = results.map((result: any) => ({
        name: result.candidateName,
        party: result.partyName,
        votes: result.votes,
        percentage: result.votePercentage,
        isWinner: result.votePercentage > 50 // Example condition for determining the winner
      }));
    } catch (error) {
      console.error('Error fetching results:', error);
    }
  } else {
    console.warn('Please select both an election and a centre.');
  }
};

const fetchElections = async () => {
  try {
    const fetchedElections = await Eservice.getAllElection();
    electionsData.value = fetchedElections;
    elections.value = fetchedElections.map((election: any) => election.name);
  } catch (error) {
    console.error('Failed to fetch elections', error);
  }
}

const fetchCentres = async (electionId: string) => {
  try {
    const centers = await ECservice.getCentersByElection(electionId);
    console.log('Fetched centers:', centers);

    // Extracting the id values
    centres.value = centers.map((center: any) => ({
      id: center.id,
      name: center.name,
    }));
  } catch (error) {
    console.error('Failed to fetch centers', error);
  }
}

// When election changes, load centres
watch(() => filters.value.election, (newElectionName) => {
  const selectedElection = electionsData.value.find(e => e.name === newElectionName);
  if (selectedElection) {
    fetchCentres(selectedElection.id);
  } else {
    centres.value = [];
  }
})

onMounted(() => {
  fetchElections();
})
</script>