<template>
    <v-dialog :model-value="Candidatedialog" max-width="1100px" @update:modelValue="updateCandidatedialog">
      <v-card title="Add Candidate">
        <v-card-text>
          <v-container>
            <v-row>
              <!-- Full Name -->
              <v-col cols="12" sm="6">
                <v-text-field v-model="formData.fullName" label="Full Name" required></v-text-field>
              </v-col>
  
              <!-- Father's Name -->
              <v-col cols="12" sm="6">
                <v-text-field v-model="formData.fatherName" label="Father's Name" required></v-text-field>
              </v-col>
  
              <!-- Mother's Name -->
              <v-col cols="12" sm="6">
                <v-text-field v-model="formData.motherName" label="Mother's Name" required></v-text-field>
              </v-col>
                <!-- Mother's Name -->
                <v-col cols="12" sm="6">
                <v-text-field v-model="formData.grandFatherName" label="Grand Father's Name" required></v-text-field>
              </v-col>
                <!-- Mother's Name -->
                <v-col cols="12" sm="6">
                <v-text-field v-model="formData.grandMotherName" label="Grand Mother's Name" required></v-text-field>
              </v-col>
              <!-- Date of Birth -->
              <v-col cols="12" sm="6">
                <v-text-field v-model="formData.dateOfBirth" label="Date of Birth" type="date" required></v-text-field>
              </v-col>
  
              <!-- Gender -->
                <v-col cols="12" sm="6">

                    <v-select v-model="formData.gender" :items="gender" item-title="name" item-value="id" label="Gender" required></v-select>

                </v-col>
    
                <!-- Party -->
                <v-col cols="12" sm="6">
                <v-select v-model="formData.partyId" :items="parties" item-title="partyName" item-value="id" label="party" required></v-select>
                </v-col>

                <v-col cols = "20" md = "12">Address</v-col>
              <!-- Election Type Dropdown -->
              <v-col cols="12" sm="6">
                <v-select v-model="formData.electionId" :items="elections" item-title="name" item-value="id" label="Election" required></v-select>
              </v-col>
  
              <!-- District Dropdown -->
              <v-col cols="12" sm="6" >
                <v-select v-model="formData.districtId" :items="districts" item-title="name" item-value="id" label="District" required></v-select>
              </v-col>
  
              <!-- Municipality Dropdown -->
              <v-col cols="12" sm="6" >
                <v-select v-model="formData.municipalityId" :items="municipalities" item-title="name" item-value="id" label="Municipality" required></v-select>
              </v-col>
  
              <!-- Ward Dropdown -->
              <v-col cols="12" sm="6" >
                <v-select v-model="formData.wardId" :items="wards" item-title="wardNumber" item-value="id" label="Ward" required></v-select>
              </v-col>
            </v-row>
          </v-container>
  
          <v-alert v-if="validationMessage" type="error" dense outlined class="mt-2">{{ validationMessage }}</v-alert>
  
        </v-card-text>
  
        <v-card-actions>
          <v-spacer></v-spacer>
          <v-btn color="blue darken-1" text @click="closeDialog">Close</v-btn>

        <v-btn 
          color="blue darken-1" 
          variant="text" 
          @click="Submit" >
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
  import PartyService from '../../service/partyService';
  import CandidateService from '../../service/candidateService';
  
  const electionCentreService = new ElectionCentreService();
  const electionService = new ElectionService();
  const partyService = new PartyService();
  const candidateService = new CandidateService();

  
  const tableData = ref([]);
  const validationMessage = ref('');
  const centreService = new Centre();
  const districts = ref([]);
  const municipalities = ref([]);
  const wards = ref([]);
  const elections = ref([]);
  const gender = ref([
      { name: 'Male' },
      {  name: 'Female' },
      {  name: 'Other' },
  ]);
  const parties = ref([]);
  
  const props = defineProps({
    updateCandidatedialog: Boolean,
    CdialogActive: Boolean,
  });
  
  const emit = defineEmits(['update:CdialogActive']);
  const Candidatedialog = ref(props.CdialogActive);
  
  const formData = reactive({
  fullname: '',
  fatherName: '',
  motherName: '',
  grandFatherName: '',
  grandMotherName: '',
  dateOfBirth: '',
  gender: null,
  partyId: '',
  electionId: '',
  districtId: '',
  municipalityId: '',
  wardId: '',
});

  
  const fetchDistricts = async () => {
    try {
      const data = await centreService.getAllDistricts();
      districts.value = Array.isArray(data) ? data : [];
    } catch (error) {
      console.error("Error fetching districts:", error);
    }
  };
  
  const fetchParties = async () => {
      try {
        const data = await partyService.getAllParties();
        parties.value = Array.isArray(data) ? data : [];
      } catch (error) {
        console.error("Error fetching parties:", error);
        }
  }

  const fetchMunicipalities = async () => {
    try {
        if (!formData.districtId) return;
        const data = await electionCentreService.getMunicipalities(`districtId=${formData.districtId}`);
        municipalities.value = Array.isArray(data) ? data : [];
        formData.municipalityId = null; 
        wards.value = [];
    } catch (error) {
        console.error("Error fetching municipalities:", error);
    }
};

  
const fetchWards = async () => {
    try {
        if (!formData.municipalityId) return; 
        const data = await electionCentreService.getWards(`municipalityId=${formData.municipalityId}`);
        wards.value = Array.isArray(data) ? data : [];
        formData.wardId = null; 
    } catch (error) {
        console.error("Error fetching wards:", error);
    }
};

  
  onMounted(() => {
    getElectionName();
    fetchDistricts();
    fetchParties();
  });
  
  const getElectionName = async () => {
    try {
      const data = await electionService.getActiveElection();
      elections.value = Array.isArray(data) ? data : [];
    } catch (error) {
      console.error("Error fetching active election:", error);
    }
  };
  watch(() => formData.districtId, () => fetchMunicipalities()); 
watch(() => formData.municipalityId, () => fetchWards()); 

  watch(() => props.CdialogActive, (newValue) => {
    Candidatedialog.value = newValue;
  });
  
  const Submit = async () => {
  try {
    await candidateService.addCandidate(formData); 
    closeDialog();
  } catch (error) {
    console.error("Error adding candidate:", error);
  }
};

  const closeDialog = () => {
    emit('update:CdialogActive', false);
  };

  const isFormValid = computed(() => {
  return formData.fullname &&  
         formData.fatherName &&
         formData.motherName &&
         formData.dateOfBirth &&
         formData.gender &&
         formData.partyId &&
         formData.electionId &&
         formData.districtId &&
         formData.municipalityId &&
         formData.wardId;
});


  </script>
  