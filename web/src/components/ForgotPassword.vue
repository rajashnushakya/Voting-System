<template>
    <v-container class="fill-height" fluid>
      <v-row align="center" justify="center">
        <v-col cols="12" sm="8" md="6" lg="4">
          <v-card class="elevation-12 rounded-lg">
            <v-card-title class="text-h5 text-center py-4 primary">
              <span class="white--text">Forgot Password</span>
            </v-card-title>
            
            <v-card-text class="pt-6">
              <div class="text-subtitle-1 mb-4 text-center">
                Enter your email address which has been already registered in system.
              </div>
              
              <v-form ref="form" v-model="isFormValid" @submit.prevent="handleSubmit">
                <v-text-field
                  v-model="email"
                  :rules="emailRules"
                  label="Email"
                  prepend-inner-icon="mdi-email-outline"
                  type="email"
                  required
                  variant="outlined"
                  autocomplete="email"
                ></v-text-field>
                
                <div class="text-caption mt-2 mb-4">
                  <div class="font-weight-medium mb-2">Email requirements:</div>
                  <ul class="pl-4">
                    <li>Must be a valid email address</li>
                    <li>Must not be empty</li>
                    <li>We'll let you update the password</li>
                  </ul>
                </div>
                
                <v-btn
                  block
                  color="blue-darken-1"
                  size="large"
                  type="submit"
                  :disabled="!isFormValid"
                  :loading="isLoading"
                  class="mt-4"
                >
                  Reset Password
                </v-btn>
              </v-form>
            </v-card-text>
            
            <v-card-actions class="pb-4 px-4">
              <v-btn
                variant="text"
                to="/login"
                class="text-blue-darken-1"
                prepend-icon="mdi-arrow-left"
              >
                Back to Login
              </v-btn>
              <v-spacer></v-spacer>
            </v-card-actions>
            
            <v-snackbar
              v-model="showSnackbar"
              :color="snackbarColor"
              timeout="5000"
              location="top"
            >
              {{ snackbarText }}
              <template v-slot:actions>
                <v-btn
                  color="white"
                  variant="text"
                  @click="showSnackbar = false"
                >
                  Close
                </v-btn>
              </template>
            </v-snackbar>
          </v-card>
        </v-col>
      </v-row>
    </v-container>
    <ChangeCredentials :settingActive="settingActive" @update:settingActive="settingActive = $event" />
  </template>
  
  <script setup>
  import { ref, reactive } from 'vue';
  import voterService  from '../service/voterService';
  import ChangeCredentials from './childcomponents/ChangeCredentials.vue';

  const service = new voterService();

  // Form validation state
  const isFormValid = ref(false);
  const form = ref(null);
  const email = ref('');
  const isLoading = ref(false);
  
  // Snackbar state
  const showSnackbar = ref(false);
  const snackbarText = ref('');
  const snackbarColor = ref('success');
  const settingActive = ref(false);
  
  // Email validation rules
  const emailRules = [
    v => !!v || 'Email is required',
    v => /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(v) || 'Email must be valid'
  ];
  
  // Form submission handler
  const handleSubmit = async () => {
  isLoading.value = true;
  try {
    const userId = await service.getUserId(email.value);
    localStorage.setItem('userid', userId);
    if (userId) {
      settingActive.value = true;
    }

    // You can show success message here
    snackbarText.value = `Valid Email`;
    snackbarColor.value = 'success';
    showSnackbar.value = true;

    // Proceed to next step like redirecting or opening a reset form
  } catch (error) {
    snackbarText.value = 'Failed to fetch user ID';
    snackbarColor.value = 'error';
    showSnackbar.value = true;
  } finally {
    isLoading.value = false;
  }
};

  </script>
  
  <style scoped>
  .v-card {
    border-radius: 12px;
  }
  
  .v-card-title {
    border-top-left-radius: 12px;
    border-top-right-radius: 12px;
  }
  </style>