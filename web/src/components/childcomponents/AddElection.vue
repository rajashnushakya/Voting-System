<template>
  <v-dialog :model-value="localDialogActive" max-width="500" @update:modelValue="updateDialog">
    <v-card title="Add New Election">
      <v-card-text>
        <form @submit.prevent="submitForm">
          <div class="space-y-4">
            <div>
              <label for="election-name" class="block text-sm font-medium text-gray-700 mb-1">Election Name</label>
              <input
                id="election-name"
                v-model="electionData.name"
                required
                class="mt-1 block w-full px-3 py-2 bg-white border border-gray-300 rounded-md text-sm shadow-sm placeholder-gray-400 focus:outline-none focus:border-sky-500 focus:ring-1 focus:ring-sky-500"
              />
            </div>

            <div>
              <label for="start-date" class="block text-sm font-medium text-gray-700 mb-1">Start Date</label>
              <input
                id="start-date"
                type="date"
                v-model="electionData.start_date"
                :min="todayDate"
                required
                class="mt-1 block w-full px-3 py-2 bg-white border border-gray-300 rounded-md text-sm shadow-sm placeholder-gray-400 focus:outline-none focus:border-sky-500 focus:ring-1 focus:ring-sky-500"
              />
            </div>

            <div>
              <label for="end-date" class="block text-sm font-medium text-gray-700 mb-1">End Date</label>
              <input
                id="end-date"
                type="date"
                v-model="electionData.end_date"
                :min="minEndDate"
                required
                class="mt-1 block w-full px-3 py-2 bg-white border border-gray-300 rounded-md text-sm shadow-sm placeholder-gray-400 focus:outline-none focus:border-sky-500 focus:ring-1 focus:ring-sky-500"
              />

            </div>
          </div>
        </form>
      </v-card-text>

      <v-card-actions>
        <v-spacer></v-spacer>
        <v-btn @click="closeDialog">Cancel</v-btn>
        <v-btn color="primary" @click="submitForm">Create Election</v-btn>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>
<script setup>
import { ref, watch, defineProps, defineEmits, computed} from 'vue';
import electionService from '../../service/electionService'; 

const props = defineProps({
  updateDialog: Boolean,
  dialogActive: Boolean,  // Controlled by parent
});

const emit = defineEmits(['update:dialogActive']); 

// Local reactive variable to prevent immediate closing
const localDialogActive = ref(props.dialogActive);

const electionData = ref({
  id: 0,
  name: '',
  start_date: null,
  end_date: null,
});

const service = new electionService();
const minEndDate = computed(() => {
  if (electionData.value.start_date) {
    const startDate = new Date(electionData.value.start_date);
    startDate.setDate(startDate.getDate() + 1); // Add one day
    return startDate.toISOString().split('T')[0];
  }
  return todayDate.value; // Default to today if start_date is not set
});

// Watch start_date to update end_date automatically
watch(() => electionData.value.start_date, (newStartDate) => {
  if (newStartDate && newStartDate >= electionData.value.end_date) {
    const nextDay = new Date(newStartDate);
    nextDay.setDate(nextDay.getDate() + 1);
    electionData.value.end_date = nextDay.toISOString().split('T')[0];
  }
});

// Watch for changes in parent prop and update local state
watch(() => props.dialogActive, (newValue) => {
  localDialogActive.value = newValue;
});

const submitForm = async () => {
  try {
    await service.addElection(electionData.value);

    // Reset form
    electionData.value = { id: 0, name: '', start_date: null, end_date: null };

    // Close dialog properly
    closeDialog();
  } catch (error) {
    console.error('Error adding election:', error);
  }
};

// Function to close the dialog
const closeDialog = () => {
  localDialogActive.value = false;  // Update local state
  emit('update:dialogActive', false); // Notify parent to update
};

const todayDate = computed(() => {
  return new Date().toISOString().split('T')[0];
});

</script>
