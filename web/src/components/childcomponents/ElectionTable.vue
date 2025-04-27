<template>
    <div class="fill-height election-page">
      <v-card class="fill-height elevation-0 rounded-0">
        <v-card-title class="d-flex align-center py-4 px-4">
          <h1 class="text-h4">Elections</h1>
          <v-spacer></v-spacer>
        </v-card-title>
  
        <v-data-table
          :headers="headers"
          :items="elections"
          :search="search"
          :items-per-page="10"
          :items-per-page-options="[5, 10, 15, 20]"
          class="fill-height"
          fixed-header
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
  @click="showElectionCenters(item.id)"
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
        prepend-icon=""
        @click="goToDashboard"
      >
        <ArrowLeftIcon class="mr-1 back-icon" />
        Back to Dashboard
      </v-btn>
  
    </div>
  </template>
<script setup>
import { ref, onMounted } from 'vue';
import router from '../../router';
import electionService from '../../service/electionService';
import { ArrowLeftIcon } from '@heroicons/vue/24/solid';

const search = ref('');
const elections = ref([]);

const service = new electionService();

const headers = [
  { title: 'ID', key: 'id', align: 'start', sortable: true },
  { title: 'Election Name', key: 'name', align: 'start', sortable: true },
  { title: 'Start Date', key: 'startDate', align: 'start', sortable: true },
  { title: 'End Date', key: 'endDate', align: 'start', sortable: true },
  { title: 'Actions', key: 'actions', align: 'center', sortable: false }
];

const getStatusColor = (status) => {
  switch (status) {
    case 'Active':
      return 'success';
    case 'Upcoming':
      return 'info';
    case 'Completed':
      return 'grey';
    default:
      return 'grey';
  }
};

const getElectionStatus = (startDate, endDate) => {
  const now = new Date();
  const start = new Date(startDate);
  const end = new Date(endDate);
  if (now < start) return 'Upcoming';
  if (now >= start && now <= end) return 'Active';
  return 'Completed';
};

const showElectionCenters = (electionId) => {
  router.push({
    name: 'election-centre-detail',
    params: { electionId } 
  });
};



const goToDashboard = () => {

  router.push({ name: 'Dashboard'});
};

onMounted(async () => {
  document.documentElement.style.height = '100%';
  document.body.style.height = '100%';
  document.body.style.margin = '0';
  document.body.style.overflow = 'hidden';

  try {
    const response = await service.getAllElection();
    elections.value = response.map(election => ({
      id: election.id,
      name: election.name,
      startDate: election.start_date.split('T')[0],
      endDate: election.end_date.split('T')[0],

    }));
  } catch (error) {
    console.error('Failed to load elections:', error);
  }
});
</script>

  
  <style scoped>
  .election-page {
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
    height: calc(100% - 80px); /* Adjust based on header height */
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
  
  /* Heroicon sizing */
  .search-icon {
    width: 20px;
    height: 20px;
  }
  
  .back-icon {
    width: 18px;
    height: 18px;
  }
  </style>