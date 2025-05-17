<template>
  <div class="fill-height election-centers-page">
    <v-card class="fill-height elevation-0 rounded-0">
      <v-card-title class="d-flex align-center py-4 px-4">
        <h1 class="text-h4">Election Centers</h1>
        <v-spacer></v-spacer>
      </v-card-title>

      <v-data-table
        :headers="headers"
        :items="electionCenters"
        :search="search"
        :items-per-page="10"
        :items-per-page-options="[5, 10, 15, 20]"
        class="fill-height"
        fixed-header
        :loading="loading"
      >
        <template v-slot:item.status="{ item }">
          <v-chip
            :color="getStatusColor(item.status)"
            text-color="white"
            size="small"
          >
            {{ item.status }}
          </v-chip>
        </template>

        <template v-slot:item.actions="{ item }">
          <v-btn
            color="primary"
            size="small"
            variant="text"
            @click="showDetails(item)"
          >
            Details
          </v-btn>
        </template>
      </v-data-table>
    </v-card>

    <!-- Back to Dashboard Button -->
    <v-btn
      color="primary"
      class="back-to-dashboard-btn"
      @click="goToDashboard"
    >
      <ArrowLeftIcon class="mr-1 back-icon" />
      Back to Election list
    </v-btn>

    <!-- Details Dialog -->
    <v-dialog v-model="detailsDialog" max-width="700px">
      <v-card v-if="selectedCenter">
        <v-card-title class="text-h5 bg-primary text-white">
          {{ selectedCenter.name }}
        </v-card-title>
        
        <v-card-text class="pa-0">
          <v-row class="ma-0">
            <!-- Left side - Information -->
            <v-col cols="12" md="7" class="pa-4">
              <v-row>
                <v-col cols="12" sm="6">
                  <v-list-item>
                    <template v-slot:prepend>
                      <MapPinIcon class="mr-2 details-icon" />
                    </template>
                    <v-list-item-title>Location</v-list-item-title>
                    <v-list-item-subtitle>{{ selectedCenter.location }}</v-list-item-subtitle>
                  </v-list-item>
                </v-col>
                
                <v-col cols="12" sm="6">
                  <v-list-item>
                    <template v-slot:prepend>
                      <UserGroupIcon class="mr-2 details-icon" />
                    </template>
                    <v-list-item-title>Total Voters</v-list-item-title>
                    <v-list-item-subtitle>{{ centerDetails?.totalVoters || 0 }}</v-list-item-subtitle>
                  </v-list-item>
                </v-col>

                <v-col cols="12" sm="6">
                  <v-list-item>
                    <template v-slot:prepend>
                      <UserGroupIcon class="mr-2 details-icon" />
                    </template>
                    <v-list-item-title>Total Candidates</v-list-item-title>
                    <v-list-item-subtitle>{{ centerDetails?.totalCandidates || 0 }}</v-list-item-subtitle>
                  </v-list-item>
                </v-col>
                
                <v-col cols="12" sm="6">
                  <v-list-item>
                    <template v-slot:prepend>
                      <CheckCircleIcon class="mr-2 details-icon" />
                    </template>
                    <v-list-item-title>Total Votes</v-list-item-title>
                    <v-list-item-subtitle>{{ centerDetails?.totalVotes || 0 }}</v-list-item-subtitle>
                  </v-list-item>
                </v-col>
              </v-row>
            </v-col>
            
            <!-- Right side - Chart -->
            <v-col cols="12" md="5" class="pa-4">
              <div class="text-center mb-2">
                <h3 class="text-h6">Voting Status</h3>
              </div>
              <div class="chart-container">
                <canvas ref="chartCanvas"></canvas>
              </div>
              <div class="text-center mt-3" v-if="centerDetails">
                <div class="stats-container">
                  <div class="stat-item">
                    <div class="stat-value">{{ centerDetails.totalVotes || 0 }}</div>
                    <div class="stat-label">Votes Cast</div>
                  </div>
                  <div class="stat-item">
                    <div class="stat-value">{{ (centerDetails.totalVoters || 0) - (centerDetails.totalVotes || 0) }}</div>
                    <div class="stat-label">Remaining Voters</div>
                  </div>
                  <div class="stat-item">
                    <div class="stat-value">
                      {{ centerDetails.totalVoters ? Math.round((centerDetails.totalVotes / centerDetails.totalVoters) * 100) : 0 }}%
                    </div>
                    <div class="stat-label">Turnout</div>
                  </div>
                </div>
              </div>
            </v-col>
          </v-row>
        </v-card-text>
        
        <v-card-actions class="pa-4">
          <v-btn
            color="secondary"
            variant="outlined"
            prepend-icon=""
            @click="detailsDialog = false"
          >
            <XMarkIcon class="mr-1 close-icon" />
            Close
          </v-btn>
          <v-spacer></v-spacer>
        </v-card-actions>
      </v-card>
    </v-dialog>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted, onBeforeUnmount, watch, nextTick } from 'vue';
import { 
  ArrowLeftIcon,
  MapPinIcon,
  UserGroupIcon,
  InformationCircleIcon,
  XMarkIcon,
  CheckCircleIcon
} from '@heroicons/vue/24/solid';
import { useRoute } from 'vue-router';
import router from '../../router';
import Chart from 'chart.js/auto';
import electionService from '../../service/electionService';
import ElectionCentreService from '../../service/electionCentreService';

// Route
const route = useRoute();
const electionId = ref(null);

// Search
const search = ref('');
const loading = ref(true);

// Service instance
const service = new electionService();
const sservice = new ElectionCentreService();

// Table headers
const headers = [
  { title: 'ID', key: 'id', align: 'start', sortable: true },
  { title: 'Name', key: 'name', align: 'start', sortable: true },
  { title: 'Location', key: 'location', align: 'start', sortable: true },
  { title: 'Actions', key: 'actions', align: 'center', sortable: false }
];

// Election Centers Data
const electionCenters = ref([]);

// Detail dialog and selected item
const detailsDialog = ref(false);
const selectedCenter = ref(null);
const centerDetails = ref(null);
const detailsLoading = ref(false);

// Chart reference
const chartCanvas = ref(null);
let chartInstance = null;

// Show details function
const showDetails = async (item) => {
  selectedCenter.value = item;
  detailsDialog.value = true;
  await fetchCenterDetails(item.id);
};

// Fetch center details
const fetchCenterDetails = async (centerId) => {
  try {
    detailsLoading.value = true;
    // Use the ElectionCentreService to fetch details using the center ID
    const response = await sservice.getElectionCentreDetails(centerId);
    centerDetails.value = response[0]; // Use the first item from the response array
    console.log('Center details:', centerDetails.value);
  } catch (error) {
    console.error("Error fetching election centre details:", error);
    centerDetails.value = {
      totalCandidates: 0,
      totalVoters: 0,
      totalVotes: 0
    };
  } finally {
    detailsLoading.value = false;
  }
};

// Watch for changes in the centerDetails
watch(() => [detailsDialog.value, centerDetails.value], async () => {
  if (detailsDialog.value && centerDetails.value) {
    // Ensure createChart is called after the DOM is updated
    await nextTick();
    createChart();
  }
});

// Create the chart
const createChart = () => {
  if (!chartCanvas.value) return; // Guard against null chartCanvas.value

  if (chartInstance) {
    chartInstance.destroy();
    chartInstance = null; // Reset chartInstance after destroying
  }

  if (!centerDetails.value) return; // Guard against null centerDetails.value

  const ctx = chartCanvas.value.getContext('2d');
  const totalVotes = centerDetails.value.totalVotes || 0;
  const totalCandidates = centerDetails.value.totalCandidates || 0;
  const totalVoters = centerDetails.value.totalVoters || 0;
  const remainingVoters = totalVoters - totalVotes;
  
  chartInstance = new Chart(ctx, {
    type: 'doughnut',
    data: {
      labels: ['Votes Cast', 'Total Candidates', 'Total Voters'],
      datasets: [{
        data: [totalVotes, totalCandidates, totalVoters],
        backgroundColor: ['#4CAF50', '#6A994E','#FED18C'],
        hoverBackgroundColor: ['#388E3C', '#FCB07E', '#9E9E9E'],
        borderWidth: 0
      }]
    },
    options: {
      responsive: true,
      maintainAspectRatio: false,
      cutout: '70%',
      plugins: {
        legend: {
          position: 'bottom'
        },
        tooltip: {
          callbacks: {
            label: function(context) {
              const value = context.raw;
              const total = context.dataset.data.reduce((a, b) => a + b, 0);
              const percentage = total > 0 ? Math.round((value / total) * 100) : 0;
              return `${context.label}: ${value} (${percentage}%)`;
            }
          }
        }
      }
    }
  });
};

// Get status color
const getStatusColor = (status) => {
  switch (status) {
    case 'Active':
      return 'success';
    case 'Inactive':
      return 'error';
    case 'Maintenance':
      return 'warning';
    default:
      return 'grey';
  }
};

// Navigate to dashboard
const goToDashboard = () => {
  router.push('/election/detail').then(() => {
    setTimeout(() => {
      window.location.reload();
    }, 100); // Delay gives the router time to navigate
  });
};


// Fetch election centers from API
const fetchElectionCenters = async () => {
  try {
    loading.value = true;
    electionId.value = route.params.electionId || route.query.electionId;
    
    const response = await service.getElectionCentreDetail(electionId.value);
    
    // Map API response to expected table format
    electionCenters.value = response.map((item) => ({
      id: item.centreId,
      name: item.centreName,
      location: `${item.districtName}, ${item.municipalityName}, Ward No. ${item.wardNumber}`,
      status: 'Active' // You can update this if API includes a real status
    }));
  } catch (err) {
    console.error('Error fetching election centers:', err);
    electionCenters.value = [];
  } finally {
    loading.value = false;
  }
};

// Clean up the chart when the component is unmounted
onBeforeUnmount(() => {
  if (chartInstance) {
    chartInstance.destroy();
  }
});

onMounted(() => {
  electionId.value = route.params.electionId || route.query.electionId;
  fetchElectionCenters();
});
</script>

<style scoped>
.election-centers-page {
  display: flex;
  flex-direction: column;
  height: 100vh;
  width: 100vw;
  overflow: hidden;
  position: relative;
}

.fill-height {
  height: 100%;
}

:deep(.v-table) {
  height: calc(100% - 80px);
}

:deep(.v-data-table__wrapper) {
  height: 100%;
  overflow-y: auto;
}

:deep(.v-card-title) {
  border-bottom: 1px solid rgba(0, 0, 0, 0.12);
}

.back-to-dashboard-btn {
  position: fixed;
  bottom: 20px;
  left: 20px;
  z-index: 10;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.15);
}

.back-icon, .close-icon {
  width: 18px;
  height: 18px;
}

.details-icon {
  width: 20px;
  height: 20px;
  color: rgb(var(--v-theme-primary));
}

.chart-container {
  height: 220px;
  position: relative;
}

.stats-container {
  display: flex;
  justify-content: space-around;
  margin-top: 10px;
}

.stat-item {
  text-align: center;
}

.stat-value {
  font-size: 1.2rem;
  font-weight: bold;
  color: rgb(var(--v-theme-primary));
}

.stat-label {
  font-size: 0.8rem;
  color: rgba(0, 0, 0, 0.6);
}
</style>