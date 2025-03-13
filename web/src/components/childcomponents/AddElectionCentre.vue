<template>
  <v-dialog :model-value="ElectionCentredialog" max-width="1100px" @update:modelValue="updateElectionCentreDialog">
    <v-card title="Add Election Centre">
      <v-card-text>
        <v-container>
          <v-row>
            <!-- Election Type Dropdown -->
            <v-col cols="12" sm="6" md="3">
              <v-select
                v-model="formData.electionName"
                :items="elections"
                item-title="name"
                item-value="id"
                label="Election"
                required
              ></v-select>
            </v-col>

            <!-- District Dropdown -->
            <v-col cols="12" sm="6" md="3">
              <v-select
                v-model="formData.district"
                :items="districts"
                item-title="name"
                item-value="id"
                label="District"
                required
              ></v-select>
            </v-col>

            <!-- Municipality Dropdown -->
            <v-col cols="12" sm="6" md="3">
              <v-select
                v-model="formData.municipality"
                :items="municipalities"
                item-title="name"
                item-value="id"
                label="Municipality"
                required
              ></v-select>
            </v-col>

            <!-- Ward Dropdown -->
            <v-col cols="12" sm="6" md="3">
              <v-select
                v-model="formData.ward"
                :items="wards"
                item-title="wardNumber"
                item-value="id"
                label="Ward"
                required
              ></v-select>
            </v-col>

            <!-- Election Centre Dropdown -->
            <v-col cols="12" sm="6" md="3">
              <v-select
                v-model="formData.electionCentre"
                :items="['Kathmandu', 'Bhaktapur', 'Lalitpur']"
                label="Election Centre"
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

const electionCentreService = new ElectionCentreService();
const electionService = new ElectionService();

const dialog = ref(false);
const tableData = ref([]);
const validationMessage = ref('');
const centreService = new Centre();
const districts = ref([]);
const municipalities = ref([]);
const wards = ref([]);
const elections = ref([]);

const props = defineProps({
  updateElectionCentreDialog: Boolean,
  EdialogActive: Boolean,
});

const emit = defineEmits(['update:EdialogActive']);
const ElectionCentredialog = ref(props.EdialogActive);

const fetchDistricts = async () => {
  try {
    const data = await centreService.getAllDistricts();
    districts.value = Array.isArray(data) ? data : [];
  } catch (error) {
    console.error("Error fetching districts:", error);
    
  }
};
const fetchMunicipalities = async () => {
  try {
    if (!formData.district) return;
    const url = `districtId=${formData.district}`; 
    const data = await electionCentreService.getMunicipalities(url);
    municipalities.value = Array.isArray(data) ? data : [];
    formData.municipality = null; // Reset municipality selection
    wards.value = []; // Reset wards when district changes
  } catch (error) {
    console.error("Error fetching municipalities:", error);
  }
};

const getElectionName = async () => {
  try {
    const data = await electionService.getActiveElection();
    console.log(data);
      elections.value = Array.isArray(data) ? data : [];
  } catch (error) {
    console.error("Error fetching active election:", error);
  }
};



const fetchWards = async (municipalityId) => {
  try {
    if (!formData.municipality) return;
    const url = `municipalityId=${formData.municipality}`;
    const data = await electionCentreService.getWards(url);
    wards.value = Array.isArray(data) ? data : [];
    formData.ward = null; // Reset ward selection
  } catch (error) {
    console.error("Error fetching wards:", error);
  }
};

const formData = reactive({
  electionName: '',
  district: '',
  municipality: '',
  ward: '',
  electionCentre: ''
});

onMounted(() => {
  getElectionName();
  fetchDistricts();
  fetchMunicipalities();
});

const dynamicHeaders = computed(() => [
  { text: 'Election', value: 'electionName' },
  { text: 'District', value: 'district' },
  { text: 'Municipality', value: 'municipality' },
  { text: 'Ward', value: 'ward' },
  { text: 'Election Centre', value: 'electionCentre' },
  { text: 'Actions', value: 'actions', sortable: false }
]);

const isFormValid = computed(() => Object.values(formData).every(value => value !== ''));

const addItem = () => {
  if (!isFormValid.value) {
    validationMessage.value = 'Please fill in all fields before adding to the table.';
    return;
  }

  validationMessage.value = '';
  tableData.value.push({ ...formData, actions: '' });
  Object.keys(formData).forEach(key => (formData[key] = ''));
};

const removeItem = (item) => {
  const index = tableData.value.indexOf(item);
  tableData.value.splice(index, 1);
};

const submitData = () => {
  console.log('Submitting data:', tableData.value);
  dialog.value = false;
  tableData.value = [];
  Object.keys(formData).forEach(key => (formData[key] = ''));
};

watch(() => props.EdialogActive, (newValue) => {
  ElectionCentredialog.value = newValue;
});
watch(() => formData.district, () => {
  fetchMunicipalities();
});

watch(() => formData.municipality, (newMunicipalityId) => {
  fetchWards(newMunicipalityId);
});
const closeDialog = () => {
  emit('update:EdialogActive', false);
};
</script>
