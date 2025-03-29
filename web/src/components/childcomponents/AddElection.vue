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
  id: 0,
  name: '',
  start_date: null,
  end_date: null,
});

const service = new electionService();

const todayDate = computed(() => {
  const now = new Date();
  return now.toISOString().slice(0, 16); // YYYY-MM-DDTHH:MM
});

const minEndDate = computed(() => {
  if (electionData.value.start_date) {
    const startDate = new Date(electionData.value.start_date);
    startDate.setHours(startDate.getHours() + 1); 
    return startDate.toISOString().slice(0, 16); 
  }
  return todayDate.value;
});


watch(() => electionData.value.start_date, (newStartDate) => {
  if (newStartDate && (!electionData.value.end_date || newStartDate >= electionData.value.end_date)) {
    const endDate = new Date(newStartDate);
    endDate.setHours(endDate.getHours() + 1); 
    electionData.value.end_date = endDate.toISOString().slice(0, 16);
  }
});

watch(() => props.dialogActive, (newValue) => {
  localDialogActive.value = newValue;
});

const submitForm = async () => {
  try {
    await service.addElection(electionData.value);
    electionData.value = { id: 0, name: '', start_date: null, end_date: null };
    closeDialog();
  } catch (error) {
    console.error('Error adding election:', error);
  }
};

const closeDialog = () => {
  localDialogActive.value = false;
  emit('update:dialogActive', false);
};
</script>
