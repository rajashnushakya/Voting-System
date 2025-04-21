
<template>
  <v-dialog
    :model-value="Settingdialog" max-width="600px" @update:modelValue="updatesettingDialog">
      <v-card title="Update Admin Credentials">
        <v-card-text>
          <v-container>
            <v-row>
              <!-- Email Section -->
              <v-col cols="12">
                <div class="d-flex align-center justify-space-between mb-2">
                  <div class="text-subtitle-1">Email Address</div>
                  <v-checkbox
                    v-model="isChangingEmail"
                    label="Update email"
                    hide-details
                    density="compact"
                  ></v-checkbox>
                </div>
                <v-text-field
                  v-model="formData.email"
                  label="Email"
                  type="email"
                  :disabled="!isChangingEmail"
                  variant="outlined"
                  density="comfortable"
                ></v-text-field>
              </v-col>
  
              <!-- Password Section -->
              <v-col cols="12">
                <div class="d-flex align-center justify-space-between mb-2">
                  <div class="text-subtitle-1">Password</div>
                  <v-checkbox
                    v-model="isChangingPassword"
                    label="Update password"
                    hide-details
                    density="compact"
                  ></v-checkbox>
                </div>
              </v-col>
  
              <!-- Current Password -->
              <v-col cols="12" v-if="isChangingPassword">
                <v-text-field
                  v-model="formData.currentPassword"
                  label="Current Password"
                  type="password"
                  variant="outlined"
                  density="comfortable"
                ></v-text-field>
              </v-col>
  
              <!-- New Password -->
              <v-col cols="12" v-if="isChangingPassword">
                <v-text-field
                  v-model="formData.newPassword"
                  label="New Password"
                  type="password"
                  variant="outlined"
                  density="comfortable"
                  hint="Password must be at least 8 characters"
                ></v-text-field>
              </v-col>
  
              <!-- Confirm Password -->
              <v-col cols="12" v-if="isChangingPassword">
                <v-text-field
                  v-model="formData.confirmPassword"
                  label="Confirm New Password"
                  type="password"
                  variant="outlined"
                  density="comfortable"
                ></v-text-field>
              </v-col>
            </v-row>
          </v-container>
  
          <v-alert
            v-if="validationMessage"
            type="error"
            density="compact"
            variant="outlined"
            class="mt-2"
          >{{ validationMessage }}</v-alert>
        </v-card-text>
  
        <v-card-actions>
          <v-spacer></v-spacer>
          <v-btn color="blue-darken-1" variant="text" @click="closeDialog">Cancel</v-btn>
          <v-btn
            color="blue-darken-1"
            variant="text"
            @click="submit"
            :loading="isSubmitting"
            :disabled="!isFormValid"
          >
            Save Changes
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
  </template>
<script setup lang="ts">
import { ref, computed, watch} from 'vue';
  import { defineProps} from 'vue';
// Props

const props = defineProps({
  updatesettingDialog: Boolean,
  settingActive: Boolean,
});
const Settingdialog = ref(props.settingActive);

const emit = defineEmits(['update:settingActive']);
const updatesettingDialog = (val: boolean) => {
  Settingdialog.value = val;
};

watch(() => props.settingActive, (newValue) => {
  Settingdialog.value = newValue;
});


// Emits


interface FormData {
  email: string;
  currentPassword: string;
  newPassword: string;
  confirmPassword: string;
}

const formData = ref<FormData>({
  email: '',
  currentPassword: '',
  newPassword: '',
  confirmPassword: ''
});


// Toggle options
const isChangingEmail = ref(true);
const isChangingPassword = ref(true);

// Validation
const validationMessage = ref('');
const isSubmitting = ref(false);

// Computed properties
const isFormValid = computed(() => {
  // Validate email if changing
  if (isChangingEmail.value) {
    if (!formData.value.email) return false;
    if (!/^\S+@\S+\.\S+$/.test(formData.value.email)) return false;
  }
  
  // Validate password if changing
  if (isChangingPassword.value) {
    if (!formData.value.currentPassword) return false;
    if (!formData.value.newPassword) return false;
    if (formData.value.newPassword.length < 8) return false;
    if (formData.value.newPassword !== formData.value.confirmPassword) return false;
  }
  
  // At least one field must be changing
  return isChangingEmail.value || isChangingPassword.value;
});

// Methods
const closeDialog = () => {
  emit('update:settingActive', false);
  resetForm();
};

const resetForm = () => {
  formData.value = {
    email: '',
    currentPassword: '',
    newPassword: '',
    confirmPassword: ''
  };
  validationMessage.value = '';
  isSubmitting.value = false;
};


const validateForm = () => {
  validationMessage.value = '';
  
  if (isChangingEmail.value) {
    if (!formData.value.email) {
      validationMessage.value = 'Email is required';
      return false;
    }
    if (!/^\S+@\S+\.\S+$/.test(formData.value.email)) {
      validationMessage.value = 'Please enter a valid email';
      return false;
    }
  }
  
  if (isChangingPassword.value) {
    if (!formData.value.currentPassword) {
      validationMessage.value = 'Current password is required';
      return false;
    }
    if (!formData.value.newPassword) {
      validationMessage.value = 'New password is required';
      return false;
    }
    if (formData.value.newPassword.length < 8) {
      validationMessage.value = 'Password must be at least 8 characters';
      return false;
    }
    if (formData.value.newPassword !== formData.value.confirmPassword) {
      validationMessage.value = 'Passwords do not match';
      return false;
    }
  }
  
  if (!isChangingEmail.value && !isChangingPassword.value) {
    validationMessage.value = 'No changes selected';
    return false;
  }
  
  return true;
};

const submit = async () => {
  if (!validateForm()) return;
  
  try {
    isSubmitting.value = true;
    
    // Create update data object
// Create update data object with explicit typing
const updateData: {
  email?: string;
  currentPassword?: string;
  newPassword?: string;
} = {};

if (isChangingEmail.value) {
  updateData.email = formData.value.email;
}

if (isChangingPassword.value) {
  updateData.currentPassword = formData.value.currentPassword;
  updateData.newPassword = formData.value.newPassword;
}

    
    // Here you would typically make an API call to update credentials
    // For example:
    // await adminService.updateCredentials(updateData);
    
    // Simulate API call
    await new Promise(resolve => setTimeout(resolve, 1000));
    
    // Emit update event with the updated data
    
    // Close dialog
    closeDialog();
  } catch (error) {
    console.error('Failed to update credentials:', error);
    validationMessage.value = 'Failed to update credentials. Please try again.';
  } finally {
    isSubmitting.value = false;
  }
};


</script>
