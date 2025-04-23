<template>
  <div>
    <div class="text-gray-600 mb-8 mx-4">
      <p class="font-semibold text-lg mb-2">Voters Details</p>
    </div>

    <div class="lg:col-span-2">
      <div class="mx-6 my-8">
        <!-- Form Fields -->
        <form @submit.prevent="handleSubmit">
          <div class="grid grid-cols-1 md:grid-cols-2 gap-6 mb-4 p-4">
            <!-- Personal Details -->
            <div>
              <label for="name" class="block text-gray-700 font-medium mb-2">Full Name</label>
              <input
                type="text"
                v-model="formData.name"
                id="name"
                class="h-10 border mt-1 rounded px-4 w-full bg-gray-50 focus:ring focus:ring-blue-300"
                placeholder="John Doe"
              />
            </div>
            <div>
              <label for="fatherName" class="block text-gray-700 font-medium mb-2">Father's Name</label>
              <input
                type="text"
                v-model="formData.fatherName"
                id="fatherName"
                class="h-10 border mt-1 rounded px-4 w-full bg-gray-50 focus:ring focus:ring-blue-300"
                placeholder="John Sr."
              />
            </div>
            <div>
              <label for="motherName" class="block text-gray-700 font-medium mb-2">Mother's Name</label>
              <input
                type="text"
                v-model="formData.motherName"
                id="motherName"
                class="h-10 border mt-1 rounded px-4 w-full bg-gray-50 focus:ring focus:ring-blue-300"
                placeholder="Jane Doe"
              />
            </div>
            <div>
              <label for="grandFatherName" class="block text-gray-700 font-medium mb-2">Grandfather's Name</label>
              <input
                type="text"
                v-model="formData.grandFatherName"
                id="grandFatherName"
                class="h-10 border mt-1 rounded px-4 w-full bg-gray-50 focus:ring focus:ring-blue-300"
                placeholder="James Doe"
              />
            </div>
            <div>
              <label for="grandMotherName" class="block text-gray-700 font-medium mb-2">Grandmother's Name</label>
              <input
                type="text"
                v-model="formData.grandMotherName"
                id="grandMotherName"
                class="h-10 border mt-1 rounded px-4 w-full bg-gray-50 focus:ring focus:ring-blue-300"
                placeholder="Mary Doe"
              />
            </div>
            <div>
              <label for="email" class="block text-gray-700 font-medium mb-2">Email Address</label>
              <input
                type="email"
                v-model="formData.email"
                id="email"
                class="h-10 border mt-1 rounded px-4 w-full bg-gray-50 focus:ring focus:ring-blue-300"
                placeholder="email@domain.com"
              />
            </div>
            <div>
              <label for="dob" class="block text-gray-700 font-medium mb-2">Date of Birth</label>
              <input
                type="date"
                v-model="formData.dateofBirth"
                id="dob"
                class="h-10 border mt-1 rounded px-4 w-full bg-gray-50 focus:ring focus:ring-blue-300"
              />
            </div>
            <div>
              <label for="gender" class="block text-gray-700 font-medium mb-2">Gender</label>
              <select
                v-model="formData.gender"
                id="gender"
                class="h-10 border mt-1 rounded px-4 w-full bg-gray-50 focus:ring focus:ring-blue-300"
              >
                <option value="Male">Male</option>
                <option value="Female">Female</option>
                <option value="Other">Other</option>
              </select>
            </div>

            <div>
              <label for="NationalId" class="block text-gray-700 font-medium mb-2">National Id </label>
              <input
                type="tel"
                v-model="formData.NationalId"
                id="phno"
                class="h-10 border mt-1 rounded px-4 w-full bg-gray-50 focus:ring focus:ring-blue-300"
                placeholder="Enter Phone Number"
                style="appearance: textfield; -moz-appearance: textfield; -webkit-appearance: textfield;"
              />
            </div>
            <div>
              <label for="phNo" class="block text-gray-700 font-medium mb-2">Phone Number</label>
              <input
                type="tel"
                v-model="formData.phoneNumber"
                id="phno"
                class="h-10 border mt-1 rounded px-4 w-full bg-gray-50 focus:ring focus:ring-blue-300"
                placeholder="Enter Phone Number"
                style="appearance: textfield; -moz-appearance: textfield; -webkit-appearance: textfield;"
              />
            </div>
            <div>
              <label for="userName" class="block text-gray-700 font-medium mb-2">Username</label>
              <input
                type="text"
                v-model="formData.user.userName"
                id="userName"
                class="h-10 border mt-1 rounded px-4 w-full bg-gray-50 focus:ring focus:ring-blue-300"
                placeholder="Enter Username"
              />
            </div>
            <div class="mt-2 relative">
              <label for="password" class="block text-gray-700 font-medium mb-2">Password</label>
              <input
              :type="showPassword ? 'text' : 'password'"
              v-model="formData.user.password"
              id="password"
              autocomplete="current-password"
              class="h-10 border mt-1 rounded px-4 w-full bg-gray-50 focus:ring focus:ring-blue-300"
              required
            />
            <span
              @click="togglePassword"
              class="absolute right-3 top-1/2 -translate-y-1/2 text-sm text-indigo-600 cursor-pointer select-none"
            >
              {{ showPassword ? 'Hide' : 'Show' }}
            </span>
            </div>
          </div>

          <!-- Address Section -->
          <div class="grid grid-cols-1 md:grid-cols-2 gap-6 mb-4 p-4">
            <div>
              <label for="houseNumber" class="block text-gray-700 font-medium mb-2">House Number</label>
              <input
                type="number"
                v-model="formData.address.houseNumber"
                id="houseNumber"
                class="h-10 border mt-1 rounded px-4 w-full bg-gray-50 focus:ring focus:ring-blue-300"
                placeholder="123"
              />
            </div>
            <div>
              <label for="streetName" class="block text-gray-700 font-medium mb-2">Street Name</label>
              <input
                type="text"
                v-model="formData.address.streetName"
                id="streetName"
                class="h-10 border mt-1 rounded px-4 w-full bg-gray-50 focus:ring focus:ring-blue-300"
                placeholder="Main Street"
              />
            </div>
            <div>
              <label for="district" class="block text-gray-700 font-medium mb-2">District</label>
              <v-select
                label="District"
                v-model="selectedDistrict"
                :items="districts"
                item-title="name"
                item-value="id"
                required
                variant="outlined"
                @update:modelValue="onDistrictChange"
              />
            </div>
            <div>
              <label for="municipality" class="block text-gray-700 font-medium mb-2">Municipality</label>
              <v-select
                v-model="selectedMunicipality"
                :items="municipalities"
                item-title="name"
                item-value="id"
                label="Municipality"
                required
                variant="outlined"
                @update:modelValue="onMunicipalityChange"
              />
            </div>
            <div>
              <label for="wardNumber" class="block text-gray-700 font-medium mb-2">Ward Number</label>
              <v-select
                v-model="selectedWard"
                :items="wards"
                item-title="wardNumber"
                item-value="id"
                label="Ward"
                required
                variant="outlined"

              />


            </div>
            <div>
              <label for="province" class="block text-gray-700 font-medium mb-2">Province</label>
              <input
                type="text"
                v-model="formData.address.province"
                id="province"
                class="h-10 border mt-1 rounded px-4 w-full bg-gray-50 focus:ring focus:ring-blue-300"
                placeholder="Province 3"
              />
            </div>
          </div>

          <div class="text-right mx-2 mb-4">
  <!-- Submit Button -->
  <button
    type="submit"
    class="bg-[#003893] hover:bg-[#002b73] text-white font-bold py-2 px-6 rounded"
  >
    Submit
  </button>

  <!-- Back to Login Button with spacing -->
  <button
    type="button"
    @click="goToLogin"
    class="ml-2 bg-gray-500 hover:bg-[#002b73] text-white font-bold py-2 px-6 rounded"
  >
    Back to login
  </button>
</div>


        </form>
      </div>
    </div>
  </div>
</template>
<script setup lang="ts">
import { ref,reactive, onMounted } from "vue";
import { AxiosError } from 'axios';
import voterService from "../service/voterService"; 
import Centre from "../service/centreService";  


const service = new voterService();
const centreService = new Centre();
const districts = ref([]);
const municipalities = ref([]);
const wards = ref([]);
const selectedDistrict = ref(null);
const selectedMunicipality = ref(null);
const selectedWard = ref(null);
const showPassword = ref(false);


// Reactive data model for form data
const formData = reactive({

  name: "",
  fatherName: "",
  motherName: "",
  grandFatherName: "",
  grandMotherName: "",
  phoneNumber: 0,
  NationalId: 0,
  email: "",
  dateofBirth: "", 
  gender: "",
  user: {
    userName: "",
    password: "",
  },
  address: {
    houseNumber: "",
    streetName: "",
    wardId: "",
    municipalityId: "",
    districtId: "",
    province: "",
  },
});

const togglePassword = () => {
  showPassword.value = !showPassword.value;
};

// Reactive data for success message
const successMessage = reactive({
  message: "",
  isVisible: false,
});
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


const goToLogin = () => {
  window.location.href = "/login";
};
const fetchMunicipalities = async (districtId: number) => {
  try {
    wards.value = [];
    const data = await centreService.getMunicipalities(districtId);
    municipalities.value = data;
  } catch (error) {
    console.error("Error fetching municipalities:", error);
  }
};

const fetchWards = async (municipalityId:number) => {
  try {
    const data = await centreService.getWards(municipalityId);
    wards.value = data;
    console.log(wards.value)
  } catch (error) {
    console.error("Error fetching wards:", error);
  }
};

const onDistrictChange = () => {
  selectedMunicipality.value = null;
  selectedWard.value = null;
  if (selectedDistrict.value) {
    fetchMunicipalities(selectedDistrict.value);
  } else {
    municipalities.value = [];
    selectedMunicipality.value = null;
    wards.value = [];
    selectedWard.value = null;
    
  }
};

onMounted(() => {
  fetchDistricts();
  
})
// Handle municipality change
const onMunicipalityChange = () => {
  
  selectedWard.value = null;
  if (selectedMunicipality.value) {
    fetchWards(selectedMunicipality.value);
  } else {
    wards.value = [];
    selectedWard.value = null;
  }
};

const handleSubmit = async () => {
  try {
    const dateofBirth = formData.dateofBirth ? new Date(formData.dateofBirth) : null;
    formData.address.districtId = selectedDistrict.value ?? '';
    formData.address.municipalityId = selectedMunicipality.value ?? '';
    formData.address.wardId = selectedWard.value ?? '';

    const response = await service.addVoter({ ...formData, dateofBirth });

    // Check backend status before success message
    if (response.data.status === 0) {
      alert(response.data.message || "Registration failed.");
      return; // Exit early if registration is not successful
    }

    // Show success message
    successMessage.message = "Registration successful!";
    successMessage.isVisible = true;

    setTimeout(() => {
      successMessage.isVisible = false;
    }, 3000);
    
  } catch (error) {
    if (error instanceof AxiosError) {
      console.error("Error details:", error.response?.data);
      console.error("Error message:", error.message);

      successMessage.message = `Error: ${error.response?.data?.message || 'An error occurred'}`;
      successMessage.isVisible = true;
    } else {
      console.error("Unknown error:", error);
      successMessage.message = 'An unknown error occurred.';
      successMessage.isVisible = true;
    }
  }
};

</script>
