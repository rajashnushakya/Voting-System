<template>
  <v-dialog :model-value="ElectionCentredialog" max-width="1100px" @update:modelValue="updateElectionCentreDialog">
    <v-card title="Add Election Centre">
      <v-card-text>
        <v-container>
          <v-row>
            <v-col v-for="(option, key) in options" :key="key" cols="12" sm="6" md="3">
              <v-select
                v-model="formData[key]"
                :items="option.items"
                :label="option.label"
                required
              ></v-select>
            </v-col>
          </v-row>
        </v-container>

        <v-btn color="success" class="mr-4" @click="addItem" :disabled="!isFormValid">
          Add to Table
        </v-btn>

        <v-alert v-if="validationMessage" type="error" dense outlined class="mt-2">
          {{ validationMessage }}
        </v-alert>

        <v-data-table v-if="tableData.length > 0" :headers="dynamicHeaders" :items="tableData" class="mt-4" dense>
          <template v-slot:item.actions="{ item }">
            <v-btn
              color="red"
              :text-size="8"
              @click="removeItem(item)">

            Remove
          </v-btn>
          </template>
        </v-data-table>
      </v-card-text>

      <v-card-actions>
        <v-spacer></v-spacer>
        <v-btn color="blue darken-1" text @click="closeDialog">Close</v-btn>
        <v-btn color="blue darken-1" text @click="submitData" :disabled="tableData.length === 0">
          Submit
        </v-btn>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<script setup>
import { ref, watch, defineEmits, reactive, computed } from 'vue';
import { defineProps } from 'vue';

const dialog = ref(false);
const tableData = ref([]);
const validationMessage = ref('');

const props = defineProps({
  updateElectionCentreDialog: Boolean,
  EdialogActive: Boolean,
});

const emit = defineEmits(['update:EdialogActive']);

const ElectionCentredialog = ref(props.EdialogActive);

const options = {
  electionName: { label: 'Election', items: ['General Election', 'Local Election', 'Presidential'] },
  district: { label: 'District', items: ['ktm', 'bkt', 'lalitput', ] },
  municipality: { label: 'Municipality', items: ['Not Started', 'In Progress', 'Review', 'Completed'] },
  ward: { label: 'Ward', items: ['John Doe', 'Jane Smith', 'Bob Johnson', 'Alice Williams'] },
  electionCentre : { label: 'Election Centre', items: ['Kathmandu', 'Bhaktapur', 'Lalitpur'] }
};

const formData = reactive({
  electionName: '',
  district: '',
  municipality: '',
  ward: ''
});

// Dynamically generate headers based on selected labels
const dynamicHeaders = computed(() => {
  let headers = Object.keys(options).map((key) => ({
    text: options[key].label,
    value: key
  }));
  headers.push({ text: 'Actions', value: 'actions', sortable: false });
  return headers;
});

const isFormValid = computed(() => {
  return Object.values(formData).every(value => value !== '');
});

const addItem = () => {
  if (!isFormValid.value) {
    validationMessage.value = 'Please fill in all fields before adding to the table.';
    return;
  }

  validationMessage.value = '';
  
  // Add selected form data to the table as an object
  tableData.value.push({ ...formData, actions: '' });

  // Reset form
  Object.keys(formData).forEach(key => formData[key] = '');
};

const removeItem = (item) => {
  const index = tableData.value.indexOf(item);
  tableData.value.splice(index, 1);
};

const submitData = () => {
  console.log('Submitting data:', tableData.value);
  dialog.value = false;
  tableData.value = [];
  Object.keys(formData).forEach(key => formData[key] = '');
};

watch(() => props.EdialogActive, (newValue) => {
  ElectionCentredialog.value = newValue;
});

const closeDialog = () => {
  emit('update:EdialogActive', false);
};
</script>
