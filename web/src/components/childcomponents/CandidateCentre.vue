<template>
    <v-dialog :model-value="ElectionCentredialog" max-width="800px" @update:modelValue="updateElectionCentreDialog">
      <v-card title="Add Candidate & Election Centre">
        <v-card-text>
          <v-container>
            <v-row>
              <!-- Election Type Dropdown -->
              <v-col cols="12" sm="6" md="4">
                <v-select
                  v-model="formData.electionName"
                  :items="elections"
                  item-title="name"
                  item-value="id"
                  label="Election"
                  required
                ></v-select>
              </v-col>
  
              <!-- Election Centre Dropdown -->
              <v-col cols="12" sm="6" md="4">
                <v-select
                  v-model="formData.electionCentre"
                  :items="[centreName]"
                  label="Election Centre"
                  required
                ></v-select>
              </v-col>
  
              <!-- Candidate Name Input -->
              <v-col cols="12" sm="6" md="4">
                <v-select
  v-model="formData.candidateName"
  :items="candidates"
  item-title="fullName"
  item-value="candidateId"
  label="Select Candidate"
  required
  outlined
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
  
          <v-data-table
            v-if="tableData.length > 0"
            :headers="dynamicHeaders"
            :items="tableData"
            class="mt-4"
            dense
          >
            <template v-slot:item.actions="{ item }">
              <v-btn color="red" @click="removeItem(item)">Remove</v-btn>
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
import { onMounted, ref, defineEmits, reactive, computed, watch } from 'vue';
import { defineProps } from 'vue';
import Centre from '../../service/centreService';
import ElectionCentreService from '../../service/electionCentreService';
import ElectionService from '../../service/electionService';
import candidateService from '../../service/candidateService';

const electionCentreService = new ElectionCentreService();
const electionService = new ElectionService();
const centreService = new Centre();
const cservice = new candidateService();

const candidates = ref([]);
const tableData = ref([]);
const validationMessage = ref('');
const elections = ref([]);
const centreName = ref([]);
const ElectionCentredialog = ref(false);

const props = defineProps({
  updateElectionCentreDialog: Boolean,
  ECdialogActive: Boolean,
});

const emit = defineEmits(['update:ECdialogActive']);
ElectionCentredialog.value = props.ECdialogActive;

const formData = reactive({
  electionName: '',
  electionCentre: '',
  candidateName: ''
});

const getElectionName = async () => {
  try {
    const data = await electionService.getActiveElection();
    elections.value = Array.isArray(data) ? data : [];
  } catch (error) {
    console.error("Error fetching active elections:", error);
  }
};

const getCentreName = async (electionId) => {
  if (!electionId) return;
  try {
    const data = await electionCentreService.getCentersByElection(electionId);
    console.log("Centre data:", data);
    centreName.value = data.map(c => c.name); // adapt this if API structure changes
  } catch (error) {
    console.error("Error fetching centre names:", error);
  }
};

const getcandidateName = async (electionId) => {
  if (!electionId) return;
  try {
    const response = await cservice.getCandidatesByElectionId(electionId);
    console.log("Candidate data:", response);
    candidates.value = Array.isArray(response.data) ? response.data : [];
  } catch (error) {
    console.error("Error fetching candidate names:", error);
  }
};


onMounted(() => {
  getElectionName();

});
watch(() => formData.electionName, (newVal) => {
  if (newVal) {
    getCentreName(newVal);
    formData.electionCentre = ''; // clear previous selection if any
  }
});

watch(() => formData.electionName, (newVal) => {
  if (newVal) {
    getcandidateName(newVal);
    formData.candidateName = ''; // clear previous selection if any
  }
});

const dynamicHeaders = computed(() => [
  { title: 'Election', value: 'electionName' },
  { title: 'Election Centre', value: 'electionCentre' },
  { title: 'Candidate', value: 'candidateName' },
  { title: 'Actions', value: 'actions', sortable: false }
]);

const isFormValid = computed(() =>
  formData.electionName && formData.electionCentre && formData.candidateName
);

const addItem = () => {
  if (!isFormValid.value) {
    validationMessage.value = 'Please fill all fields.';
    return;
  }

  const electionName = elections.value.find(e => e.id === formData.electionName)?.name || '';

  tableData.value.push({
    electionId: formData.electionName,
    electionName,
    electionCentre: formData.electionCentre,
    candidateName: formData.candidateName
  });

  validationMessage.value = '';
  formData.electionName = '';
  formData.electionCentre = '';
  formData.candidateName = '';
};

const getApiPayload = () => {
  return tableData.value.map(item => ({
    electionId: item.electionId,
    electionCentre: item.electionCentre,
    candidateName: item.candidateName
  }));
};

const submitData = async () => {
  if (tableData.value.length === 0) {
    console.error("No data to submit.");
    return;
  }

  try {
    const payload = getApiPayload();
    console.log('Payload:', payload);
    const response = await electionCentreService.addElectionCentre(payload);
    console.log('Response:', response);
    tableData.value = [];
  } catch (error) {
    console.error("Error submitting data:", error);
  }
};

const removeItem = (item) => {
  const index = tableData.value.indexOf(item);
  if (index !== -1) tableData.value.splice(index, 1);
};

const closeDialog = () => {
  emit('update:ECdialogActive', false);
};

watch(() => props.ECdialogActive, (newVal) => {
  ElectionCentredialog.value = newVal;
});
</script>
  