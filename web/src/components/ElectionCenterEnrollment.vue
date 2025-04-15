<template>
  <v-container>
    <v-card class="mb-6 rounded-lg shadow-md">
      <v-card-title class="text-h5 font-medium">Election Center Enrollment</v-card-title>
      <v-card-subtitle>
        Select an election center where you would like to cast your vote for the upcoming election.
      </v-card-subtitle>
      <v-card-text>
        <v-text-field
          v-model="searchQuery"
          label="Search Election Centers"
          placeholder="Search by name, address, or city..."
          prepend-inner-icon="mdi-magnify"
          variant="outlined"
          class="mb-4"
        ></v-text-field>

        <div class="text-gray-600 dark:text-gray-300 mb-4">Available Election Centers</div>

        <v-skeleton-loader v-if="loading" type="card" class="mb-4" />

        <v-radio-group v-model="selectedCenter" v-else>
          <v-card
            v-for="center in filteredCenters"
            :key="center.id"
            class="mb-3 transition-colors cursor-pointer"
            :class="{ 'border-2 border-primary': selectedCenter === center.id }"
          >
            <v-card-text class="p-4" @click="selectedCenter = center.id">
              <div class="flex justify-between">
                <div class="flex items-start">
                  <v-radio :value="center.id" class="mt-0 mr-2"></v-radio>
                  <div>
                    <div class="text-h6">{{ center.name }}</div>
                    <div class="flex items-center text-sm text-gray-500 dark:text-gray-400">
                      <v-icon size="small" class="mr-1">mdi-map-marker</v-icon>
                      {{ center.address }}, {{ center.city }}
                    </div>
                  </div>
                </div>
                <div class="text-right">
                  <div class="text-sm">{{ center.distance }}</div>
                  <div class="text-sm text-gray-500 dark:text-gray-400 mt-1">
                   : {{ center.currentEnrollment }}/{{ center.municipality }}
                  </div>
                </div>
              </div>
              <v-progress-linear
                :model-value="(center.currentEnrollment / center.capacity) * 100"
                color="primary"
                class="mt-2"
                rounded
              ></v-progress-linear>
            </v-card-text>
          </v-card>

          <div v-if="filteredCenters.length === 0" class="text-center py-8">
            <v-icon size="large" class="text-gray-400">mdi-map-search</v-icon>
            <div class="text-h6 mt-4">No Election Centers Found</div>
            <div class="text-body-1 text-gray-600 dark:text-gray-400 mt-2">
              No election centers match your search criteria.
            </div>
          </div>
        </v-radio-group>
      </v-card-text>

      <v-card-actions class="px-4 pb-4">
        <v-btn variant="outlined" @click="navigateToEnrollment">Back to Elections</v-btn>
        <v-spacer></v-spacer>
        <v-btn color="primary" :disabled="!selectedCenter" @click="confirmEnrollment">Enroll at Selected Center</v-btn>
      </v-card-actions>
    </v-card>

    <!-- Confirmation Dialog -->
    <v-dialog v-model="showConfirmation" max-width="500">
      <v-card>
        <v-card-title class="text-h5">Confirm Enrollment</v-card-title>
        <v-card-text>
          <p>
            You are about to enroll at <strong>{{ selectedCenterName }}</strong>.
          </p>
          <p class="text-sm text-gray-600 dark:text-gray-400 mt-4">
            By confirming, you agree to cast your vote at this location for the upcoming election.
            You can change your enrollment up to 48 hours before the election day.
          </p>
        </v-card-text>
        <v-card-actions>
          <v-spacer></v-spacer>
          <v-btn variant="outlined" @click="showConfirmation = false">Cancel</v-btn>
          <v-btn color="primary" @click="confirmEnrollment">
            <v-icon size="small" class="mr-1">mdi-check</v-icon>
            Confirm Enrollment
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>

    <!-- Success Dialog -->
    <v-dialog v-model="showSuccessDialog" max-width="400">
      <v-card>
        <v-card-title class="text-h5 bg-success text-white">Enrollment Successful</v-card-title>
        <v-card-text class="pt-4">
          <div class="text-center">
            <v-icon size="x-large" color="success">mdi-check-circle</v-icon>
            <div class="text-h6 mt-2">You're all set!</div>
            <div class="text-body-1 mt-2">
              You have successfully enrolled at {{ selectedCenterName }} for the upcoming election.
            </div>
          </div>
        </v-card-text>
        <v-card-actions>
          <v-spacer></v-spacer>
          <v-btn variant="outlined" @click="navigateToEnrollment">Back to Elections</v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
  </v-container>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue';
import ElectionCentreService from '../service/electionCentreService';
import { useRouter } from 'vue-router';

import voterService from '../service/voterService';

const router = useRouter();
// Props & Emits
const props = defineProps<{ electionId: string }>();
const emit = defineEmits(['update:activeTab', 'enrollment-complete']);
const voterEnrollment = new voterService();

// State
const electionCentreService = new ElectionCentreService();
const searchQuery = ref('');
const selectedCenter = ref<string | null>(null);
const showConfirmation = ref(false);
const showSuccessDialog = ref(false);
const electionCenters = ref<any[]>([]);
const loading = ref(true);

const navigateToEnrollment = () => {
  router.push({ name: 'voter-dashboard' });
};

// Fetch election centers from API
onMounted(async () => {
  try {
    loading.value = true;
    electionCenters.value = await electionCentreService.getCentersByElection(props.electionId);
  } catch (err) {
    console.error('Failed to fetch election centers:', err);
  } finally {
    loading.value = false;
  }
});

// Computed Properties
const filteredCenters = computed(() => {
  if (!searchQuery.value) return electionCenters.value;
  const query = searchQuery.value.toLowerCase();
  return electionCenters.value.filter(center =>
    center.name.toLowerCase().includes(query) ||
    center.address.toLowerCase().includes(query) ||
    center.city.toLowerCase().includes(query)
  );
});

const selectedCenterName = computed(() => {
  const center = electionCenters.value.find(c => c.id === selectedCenter.value);
  return center ? center.name : '';
});



const confirmEnrollment = async () => {
  try {
    showConfirmation.value = false;

    // Get the voterId from localStorage
    const voterIdFromLocalStorage = localStorage.getItem("voterid");

    if (!voterIdFromLocalStorage) {
      throw new Error("Voter ID not found in localStorage");
    }

    const selectedCenterObj = electionCenters.value.find(
      (center) => center.name === selectedCenterName.value
    );
    if (!selectedCenterObj) {
      throw new Error("Selected center not found");
    }

    // Perform the enrollment API call here
    await voterEnrollment.enrollVoterToElectionCenter(
      voterIdFromLocalStorage, 
      selectedCenterObj.id 
    );

    showSuccessDialog.value = true;

    emit('enrollment-complete', {
      electionId: props.electionId,
      centerId: selectedCenterObj.id,
    });
  } catch (err) {
    console.error('Enrollment failed:', err);
    // Optionally show an error dialog
    alert('Enrollment failed. Please try again.');
  }
};
</script>

<style scoped>
.v-card {
  transition: all 0.3s ease;
}
.v-card:hover {
  transform: translateY(-2px);
}
</style>
