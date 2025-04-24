<template>
  <v-card>
    <v-card-title class="text-h5 bg-primary text-white">
      {{ center.name }}
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
                <v-list-item-subtitle>{{ center.location }}</v-list-item-subtitle>
              </v-list-item>
            </v-col>
            
            <v-col cols="12" sm="6">
              <v-list-item>
                <template v-slot:prepend>
                  <UserGroupIcon class="mr-2 details-icon" />
                </template>
                <v-list-item-title>Capacity</v-list-item-title>
                <v-list-item-subtitle>{{ center.capacity }} voters</v-list-item-subtitle>
              </v-list-item>
            </v-col>
            
            <v-col cols="12" sm="6">
              <v-list-item>
                <template v-slot:prepend>
                  <PhoneIcon class="mr-2 details-icon" />
                </template>
                <v-list-item-title>Contact</v-list-item-title>
                <v-list-item-subtitle>{{ center.contact }}</v-list-item-subtitle>
              </v-list-item>
            </v-col>
            
            <v-col cols="12" sm="6">
              <v-list-item>
                <template v-slot:prepend>
                  <ClockIcon class="mr-2 details-icon" />
                </template>
                <v-list-item-title>Opening Hours</v-list-item-title>
                <v-list-item-subtitle>{{ center.hours }}</v-list-item-subtitle>
              </v-list-item>
            </v-col>
            
            <v-col cols="12">
              <v-list-item>
                <template v-slot:prepend>
                  <InformationCircleIcon class="mr-2 details-icon" />
                </template>
                <v-list-item-title>Additional Information</v-list-item-title>
                <v-list-item-subtitle>{{ center.additionalInfo }}</v-list-item-subtitle>
              </v-list-item>
            </v-col>
          </v-row>
        </v-col>
        
        <!-- Right side - Chart -->
        <v-col cols="12" md="5" class="pa-4">
          <div class="text-center mb-2">
            <h3 class="text-h6">Capacity Utilization</h3>
          </div>
          <div class="chart-container">
            <canvas ref="chartCanvas"></canvas>
          </div>
          <div class="text-center mt-3">
            <div class="stats-container">
              <div class="stat-item">
                <div class="stat-value">{{ center.currentVoters }}</div>
                <div class="stat-label">Current Voters</div>
              </div>
              <div class="stat-item">
                <div class="stat-value">{{ center.capacity - center.currentVoters }}</div>
                <div class="stat-label">Available Capacity</div>
              </div>
              <div class="stat-item">
                <div class="stat-value">{{ Math.round((center.currentVoters / center.capacity) * 100) }}%</div>
                <div class="stat-label">Utilization</div>
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
        @click="$emit('close')"
      >
        <XMarkIcon class="mr-1 close-icon" />
        Close
      </v-btn>
      <v-spacer></v-spacer>
      <v-btn
        color="primary"
        variant="elevated"
        prepend-icon=""
      >
        <PrinterIcon class="mr-1 print-icon" />
        Print Details
      </v-btn>
    </v-card-actions>
  </v-card>
</template>

<script setup>
import { ref, onMounted, onBeforeUnmount, watch } from 'vue';
import { 
  MapPinIcon,
  UserGroupIcon,
  PhoneIcon,
  ClockIcon,
  InformationCircleIcon,
  XMarkIcon,
  PrinterIcon
} from '@heroicons/vue/24/solid';
import Chart from 'chart.js/auto';

// Define props
const props = defineProps({
  center: {
    type: Object,
    required: true
  }
});

// Define emits
defineEmits(['close']);

// Chart reference
const chartCanvas = ref(null);
let chartInstance = null;

// Create the chart
const createChart = () => {
  if (chartInstance) {
    chartInstance.destroy();
  }

  const ctx = chartCanvas.value.getContext('2d');
  const { capacity, currentVoters } = props.center;
  const availableCapacity = capacity - currentVoters;
  
  chartInstance = new Chart(ctx, {
    type: 'doughnut',
    data: {
      labels: ['Current Voters', 'Available Capacity'],
      datasets: [{
        data: [currentVoters, availableCapacity],
        backgroundColor: ['#4CAF50', '#E0E0E0'],
        hoverBackgroundColor: ['#388E3C', '#9E9E9E'],
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
              const percentage = Math.round((value / total) * 100);
              return `${context.label}: ${value} (${percentage}%)`;
            }
          }
        }
      }
    }
  });
};

// Watch for changes in the center data
watch(() => props.center, () => {
  // Use nextTick to ensure the DOM is updated
  setTimeout(() => {
    createChart();
  }, 0);
}, { deep: true });

// Initialize the chart when the component is mounted
onMounted(() => {
  createChart();
});

// Clean up the chart when the component is unmounted
onBeforeUnmount(() => {
  if (chartInstance) {
    chartInstance.destroy();
  }
});
</script>

<style scoped>
.chart-container {
  height: 220px;
  position: relative;
}

.details-icon {
  width: 20px;
  height: 20px;
  color: rgb(var(--v-theme-primary));
}

.close-icon, .print-icon {
  width: 18px;
  height: 18px;
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