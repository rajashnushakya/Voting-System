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
              <label for="start-date" class="block text-sm font-medium text-gray-700 mb-1">Start Date & Time</label>
              <input
                id="start-date"
                type="datetime-local"
                v-model="electionData.start_date"
                :min="todayDate"
                required
                class="mt-1 block w-full px-3 py-2 bg-white border border-gray-300 rounded-md text-sm shadow-sm placeholder-gray-400 focus:outline-none focus:border-sky-500 focus:ring-1 focus:ring-sky-500"
              />
            </div>

            <div>
              <label for="end-date" class="block text-sm font-medium text-gray-700 mb-1">End Date & Time</label>
              <input
                id="end-date"
                type="datetime-local"
                v-model="electionData.end_date"
                :min="minEndDate"
                required
                class="mt-1 block w-full px-3 py-2 bg-white border border-gray-300 rounded-md text-sm shadow-sm placeholder-gray-400 focus:outline-none focus:border-sky-500 focus:ring-1 focus:ring-sky-500"
              />
            </div>
            <div class="mt-1 block w-full">
              <label for="election-type" class="block text-sm font-medium text-gray-700 mb-1">Election Type</label>
              <v-select
                label="Election Type"
                v-model="electionData.Type"  
                :items="electionTypes"
                item-title="name"   
                item-value="id" 
                required
                variant="outlined"
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
import { ref, watch, defineProps, defineEmits, computed } from 'vue';
import electionService from '../../service/electionService'; 

const props = defineProps({
  updateDialog: Boolean,
  dialogActive: Boolean,
});

const emit = defineEmits(['update:dialogActive']); 

const localDialogActive = ref(props.dialogActive);

const electionData = ref({
  name: '',
  start_date: null,
  end_date: null,
  Type: null,
});

const service = new electionService();
const electionTypes = ref([]);

// Fetch the election types
const getElectionTypes = async () => {
  try {
    const data = await service.getElectionType();
    console.log("Raw election type data:", data); 
    electionTypes.value = data.map(item => ({
      id: item.electionTypeId,
      name: item.electionTypeName
    }));
    console.log("Election Types:", electionTypes.value);
  } catch (error) {
    console.error("Error fetching election types:", error);
  }
};// Computed property for today's date (used for min start_date)
const todayDate = computed(() => {
  const now = new Date();
  return now.toISOString().slice(0, 16);
});

// Computed property for minimum valid end_date (1 day after start_date)
const minEndDate = computed(() => {
  if (!electionData.value.start_date) return todayDate.value;

  const start = new Date(electionData.value.start_date);
  start.setDate(start.getDate() + 1); // Add 1 day

  return start.toISOString().slice(0, 16);
});

// Watch start_date: clear end_date if it is less than 1 day after start_date
watch(
  () => electionData.value.start_date,
  (newStart) => {
    if (electionData.value.end_date && newStart) {
      const end = new Date(electionData.value.end_date);
      const minValidEnd = new Date(newStart);
      minValidEnd.setDate(minValidEnd.getDate() + 1); // Add 1 day

      if (end < minValidEnd) {
        electionData.value.end_date = null; // Clear invalid end_date
      }
    }
  }
);






// Watcher for dialog active state, to reload election types when opening the dialog
watch(() => props.dialogActive, (newValue) => {
  localDialogActive.value = newValue;
});

watch(localDialogActive, (newVal) => {
  if (newVal) {
    getElectionTypes();
  }
});

// Submit the form and save the data
const submitForm = async () => {
  try {
    // // Ensure only the 'id' is sent in Type
    // if (electionData.value.Type && typeof electionData.value.Type === 'object') {
    //   electionData.value.Type = electionData.value.Type.id;
    // }
    if (typeof electionData.value.Type === 'number') {
      electionData.value.Type = String(electionData.value.Type);
    }
    await service.addElection(electionData.value);
    window.alert("Election added successfully!");
    electionData.value = { id: 0, name: '', start_date: null, end_date: null, Type: null };
    closeDialog();
    window.location.reload();

  } catch (error) {
    console.error('Error adding election:', error);
  }

};

// Close the dialog
const closeDialog = () => {
  localDialogActive.value = false;
  emit('update:dialogActive', false);
};
</script>
