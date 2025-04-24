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
        prepend-icon=""
        @click="goToDashboard"
      >
        <ArrowLeftIcon class="mr-1 back-icon" />
        Back to Dashboard
      </v-btn>
  
      <!-- Details Dialog using the child component -->
      <v-dialog v-model="detailsDialog" max-width="700px">
        <ElectionCenterDetails 
          v-if="selectedCenter" 
          :center="selectedCenter" 
          @close="detailsDialog = false" 
        />
      </v-dialog>
    </div>
  </template>
  
  <script setup>
  import { ref, reactive, onMounted } from 'vue';
  import { 
    MagnifyingGlassIcon,
    ArrowLeftIcon
  } from '@heroicons/vue/24/solid';
  import ElectionCenterDetails from './ElectionCentreDetails.vue';
  import router from '../router';
  
  // Search functionality
  const search = ref('');
  
  // Table headers
  const headers = [
    { title: 'ID', key: 'id', align: 'start', sortable: true },
    { title: 'Name', key: 'name', align: 'start', sortable: true },
    { title: 'Location', key: 'location', align: 'start', sortable: true },
    { title: 'Actions', key: 'actions', align: 'center', sortable: false }
  ];
  
  // Sample data for election centers
  const electionCenters = reactive([
    {
      id: 1,
      name: 'Central Community Hall',
      location: '123 Main Street, Downtown',
      capacity: 1500,
      currentVoters: 1275,
      contact: '(555) 123-4567',
      status: 'Active',
      hours: '7:00 AM - 8:00 PM',
      additionalInfo: 'Wheelchair accessible, ample parking available.'
    },
    {
      id: 2,
      name: 'Westside Elementary School',
      location: '456 Park Avenue, West District',
      capacity: 800,
      currentVoters: 650,
      contact: '(555) 234-5678',
      status: 'Active',
      hours: '7:00 AM - 8:00 PM',
      additionalInfo: 'Gymnasium entrance, ID required.'
    },
    {
      id: 3,
      name: 'Northside Recreation Center',
      location: '789 Oak Road, North District',
      capacity: 1200,
      currentVoters: 0,
      contact: '(555) 345-6789',
      status: 'Maintenance',
      hours: '7:00 AM - 8:00 PM',
      additionalInfo: 'Temporary relocation due to renovations.'
    },
    {
      id: 4,
      name: 'Eastside Library',
      location: '101 Maple Drive, East District',
      capacity: 600,
      currentVoters: 590,
      contact: '(555) 456-7890',
      status: 'Active',
      hours: '7:00 AM - 8:00 PM',
      additionalInfo: 'Limited parking, public transportation recommended.'
    },
    {
      id: 5,
      name: 'Southside Community Center',
      location: '202 Pine Street, South District',
      capacity: 1000,
      currentVoters: 0,
      contact: '(555) 567-8901',
      status: 'Inactive',
      hours: 'Closed',
      additionalInfo: 'Temporarily closed due to facility issues.'
    },
    {
      id: 6,
      name: 'University Campus Hall',
      location: '303 College Blvd, University District',
      capacity: 2000,
      currentVoters: 1100,
      contact: '(555) 678-9012',
      status: 'Active',
      hours: '7:00 AM - 9:00 PM',
      additionalInfo: 'Extended hours, student ID accepted.'
    },
    {
      id: 7,
      name: 'Riverside Convention Center',
      location: '404 River Road, Riverside',
      capacity: 3000,
      currentVoters: 2200,
      contact: '(555) 789-0123',
      status: 'Active',
      hours: '7:00 AM - 8:00 PM',
      additionalInfo: 'Multiple polling stations, follow signs for directions.'
    },
    {
      id: 8,
      name: 'Downtown Civic Center',
      location: '505 Civic Plaza, Downtown',
      capacity: 2500,
      currentVoters: 1700,
      contact: '(555) 890-1234',
      status: 'Active',
      hours: '7:00 AM - 8:00 PM',
      additionalInfo: 'Multiple entrances, follow directional signs.'
    },
    {
      id: 9,
      name: 'Highland Park Community Center',
      location: '606 Highland Ave, Highland District',
      capacity: 900,
      currentVoters: 700,
      contact: '(555) 901-2345',
      status: 'Active',
      hours: '7:00 AM - 8:00 PM',
      additionalInfo: 'Accessible entrance on north side of building.'
    },
    {
      id: 10,
      name: 'Meadowbrook Senior Center',
      location: '707 Meadow Lane, Meadowbrook',
      capacity: 700,
      currentVoters: 450,
      contact: '(555) 012-3456',
      status: 'Active',
      hours: '7:00 AM - 8:00 PM',
      additionalInfo: 'Assistance available for elderly voters.'
    }
  ]);
  
  // Details dialog functionality
  const detailsDialog = ref(false);
  const selectedCenter = ref(null);
  
  // Function to show details dialog
  const showDetails = (item) => {
    selectedCenter.value = item;
    detailsDialog.value = true;
  };
  
  // Function to get status color
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
  
  // Function to navigate back to dashboard
  const goToDashboard = () => {
    router.push('/dashboard');
  };
  
  // Ensure the component takes full height when mounted
  onMounted(() => {
    document.documentElement.style.height = '100%';
    document.body.style.height = '100%';
    document.body.style.margin = '0';
    document.body.style.overflow = 'hidden';
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