
<template>
  <v-dialog
    :model-value="Settingdialog" max-width="600px" @update:modelValue="updatesettingDialog">
      <v-card title="Update Admin Credentials">
        <v-card-text>
          <v-container>
            <v-row>
              <!-- Email Section
              <v-col cols="12">
                <div class="d-flex align-center justify-space-between mb-2">
                  <div class="text-subtitle-1">Email Address</div>
                  <v-checkbox
                    v-model="isChangingPassword"
                    label="Update email only"
                    hide-details
                    density="compact"
                    class = "text-blue-darken-1"
                  ></v-checkbox>
                </div>
                <v-col cols="12" v-if="isChangingEmail">
                <v-text-field
                  v-model="formData.email"
                  label="Email"
                  type="email"
                  :disabled="!isChangingEmail"
                  variant="outlined"
                  density="comfortable"

                ></v-text-field>
              </v-col>
              </v-col> -->

  
              <!-- Password Section
              <v-col cols="12">
                <div class="d-flex align-center justify-space-between mb-2">
                  <div class="text-subtitle-1">Password</div>
                  <v-checkbox
                    v-model="isChangingEmail"
                    label="Update password only"
                    hide-details
                    density="compact"
                    class = "text-blue-darken-1"
                  ></v-checkbox>
                </div>
              </v-col> -->
  
              <!-- Current Password
              <v-col cols="12" >
                <v-text-field
                  v-model="formData.currentPassword"
                  label="Current Password"
                  type="password"
                  variant="outlined"
                  density="comfortable"
                ></v-text-field>
              </v-col> -->
  
              <!-- New Password -->
              <v-col cols="12" >
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
              <v-col cols="12" >
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

          >
            Save Changes
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
  </template>
  
  
  <script setup lang="ts">
import { ref, watch } from 'vue';
import { defineProps } from 'vue';
import voterService from '../../service/voterService'; 

const service = new voterService();

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

// Form data
interface FormData {
  userId: string; // Renamed from userid to userId

  newPassword: string;
  confirmPassword: string;
}

const formData = ref<FormData>({
  userId: localStorage.getItem('userid') || '',
  newPassword: '',
  confirmPassword: ''
});


// Validation
const validationMessage = ref('');
const isSubmitting = ref(false);

// Close dialog
const closeDialog = () => {
  emit('update:settingActive', false);
  resetForm();
};

const resetForm = () => {
  formData.value = {
    userId: localStorage.getItem('userid') || '',
    newPassword: '',
    confirmPassword: ''
  };
  validationMessage.value = '';
  isSubmitting.value = false;
};

// Validation logic
const validateForm = () => {
  validationMessage.value = '';

  if (!formData.value.newPassword) {
    validationMessage.value = 'New password is required';
    return false;
  }

  if (formData.value.newPassword.length < 8) {
    validationMessage.value = 'Password must be at least 8 characters';
    return false;
  }

  if (!formData.value.confirmPassword) {
    validationMessage.value = 'Please confirm your new password';
    return false;
  }

  if (formData.value.newPassword !== formData.value.confirmPassword) {
    validationMessage.value = 'Passwords do not match';
    return false;
  }

  return true;
};

// Submit handler
const submit = async () => {
  if (!validateForm()) return;

  try {
    isSubmitting.value = true;
    const userId = localStorage.getItem('userid');
    console.log(userId);
    if (!userId) {
      validationMessage.value = 'User ID not found. Please log in again.';
      return;
    }
    const response = await service.changePassword(formData.value);

    if (response.status === 0) {
      alert('Password updated successfully!');
      closeDialog();
    } else {
      validationMessage.value = response.error_msg || 'Failed to update password. Please try again.';
    }
  } catch (error) {
    console.error('Error changing password:', error);
    validationMessage.value = 'Failed to update password. Please try again.';
  } finally {
    isSubmitting.value = false;
  }
};
</script>
