<template>
  <v-dialog :model-value="dialog" max-width="500px" @update:model-value="emit('update:dialog', $event)">
    <v-card title="Add New Center">
      <v-card-text>
        <v-container>
          <v-row>
            <v-col cols="12">
              <v-text-field v-model="name" label="Name" required></v-text-field>
            </v-col>
            <v-col cols="12">
              <v-select
                label="District"
                v-model="selectedDistrict"
                :items="districts"
                item-title="name"
                item-value="id"
                required
                variant="outlined"
                @update:modelValue="onDistrictChange"
              >
                <!-- <template v-slot:item="{ item }">
                  <v-list-item>
                    <v-list-item-content>
                      <v-list-item-title>{{ item.name }}</v-list-item-title>
                    </v-list-item-content>
                  </v-list-item>
                </template> -->
              </v-select>
            </v-col>
            <v-col cols="12">
              <v-select
                v-model="selectedMunicipality"
                :items="municipalities"
                item-title="name"
                item-value="id"
                label="Municipality"
                required
                variant="outlined"
                @update:modelValue="onMunicipalityChange"
              >
                <!-- <template v-slot:item="{ item }">
                  <v-list-item>
                    <v-list-item-content>
                      <v-list-item-title>{{ item.name }}</v-list-item-title>
                    </v-list-item-content>
                  </v-list-item>
                </template> -->
              </v-select>
            </v-col>
            <v-col cols="12">
              <v-select
                v-model="selectedWard"
                :items="wards"
                item-title="wardNumber"
                item-value="id"
                label="Ward"
                required
                variant="outlined"

              >
                <!-- <template v-slot:item="{ item }">
                  <v-list-item>
                    <v-list-item-content>
                      <v-list-item-title>{{ item.name }}</v-list-item-title>
                    </v-list-item-content>
                  </v-list-item>
                </template> -->
              </v-select>
            </v-col>
          </v-row>
        </v-container>
      </v-card-text>
      <v-card-actions>
        <v-spacer></v-spacer>
        <v-btn color="blue-darken-1" variant="text" @click="close">
          Cancel
        </v-btn>
        <v-btn color="blue-darken-1" variant="text" @click="save">
          Add
        </v-btn>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<script setup>
import { ref, watch, defineProps, defineEmits } from "vue";
import Centre from "../../service/Centre";  

const props = defineProps(["dialog"]);
const emit = defineEmits(["update:dialog"]);

const localDialog = ref(false);
const selectedDistrict = ref(null);
const selectedMunicipality = ref(null);
const selectedWard = ref(null);

// Initialize services
const centreService = new Centre();
const districts = ref([]);
const municipalities = ref([]);
const wards = ref([]);

// Fetch data from API
const fetchDistricts = async () => {
  try {
    municipalities.value = [];
    wards.value = [];
    const data = await centreService.getAllDistricts();
    districts.value = data;

    console.log(districts.value)
  } catch (error) {
    console.error("Error fetching districts:", error);
  }
};

const fetchMunicipalities = async (districtId) => {
  try {
    wards.value = [];
    const data = await centreService.getMunicipalities(districtId);
    municipalities.value = data;
  } catch (error) {
    console.error("Error fetching municipalities:", error);
  }
};

const fetchWards = async (municipalityId) => {
  try {
    const data = await centreService.getWards(municipalityId);
    wards.value = data;
    console.log(wards.value)
  } catch (error) {
    console.error("Error fetching wards:", error);
  }
};

// Handle district change
const onDistrictChange = () => {
  selectedMunicipality.value = [];
  selectedWard.value = [];
  if (selectedDistrict.value) {
    fetchMunicipalities(selectedDistrict.value);
  } else {
    municipalities.value = [];
    selectedMunicipality.value = null;
    wards.value = [];
    selectedWard.value = null;
    
  }
};

// Handle municipality change
const onMunicipalityChange = () => {
  
  selectedWard.value = [];
  if (selectedMunicipality.value) {
    fetchWards(selectedMunicipality.value);
  } else {
    wards.value = [];
    selectedWard.value = null;
  }
};

// Watch for prop changes
watch(
  () => props.dialog,
  (newVal) => {
    localDialog.value = newVal;
    if (newVal) {
      fetchDistricts();  
    }
  }
);

// Close dialog and reset selected values
const close = () => {
  emit("update:dialog", false);
  selectedDistrict.value = null;
  selectedMunicipality.value = null;
  selectedWard.value = null;
};

// Save the selected data
const save = () => {
  
  close();
};


</script>
