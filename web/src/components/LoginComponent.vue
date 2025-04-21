<template>
  <div class="flex min-h-full flex-1 flex-col justify-center px-6 py-12 lg:px-8">
    <div class="sm:mx-auto sm:w-full sm:max-w-sm">
      <h2 class="mt-10 text-center text-2xl font-bold tracking-tight text-gray-900">
        Sign in to your account
      </h2>
    </div>

    <div class="mt-10 sm:mx-auto sm:w-full sm:max-w-sm">
      <form class="space-y-6" @submit.prevent="handleLogin">
        <div>
          <label for="email" class="block text-sm font-medium text-gray-900">Email address</label>
          <div class="mt-2">
            <input
              type="email"
              v-model="email"
              id="email"
              autocomplete="email"
              class="block w-full rounded-md bg-white px-3 py-1.5 text-base text-gray-900 outline outline-1 -outline-offset-1 outline-gray-300 placeholder:text-gray-400 focus:outline focus:outline-2 focus:-outline-offset-2 focus:outline-indigo-600 sm:text-sm"
              required
            />
          </div>
        </div>

        <div>
          <div class="flex items-center justify-between">
            <label for="password" class="block text-sm font-medium text-gray-900">Password</label>
            <a href="/forgot-password" class="text-sm font-semibold text-indigo-600 hover:text-indigo-500">
              Forgot password?
            </a>
          </div>
          <div class="mt-2 relative">
            <input
              :type="showPassword ? 'text' : 'password'"
              v-model="password"
              id="password"
              autocomplete="current-password"
              class="block w-full rounded-md bg-white px-3 py-1.5 text-base text-gray-900 outline outline-1 -outline-offset-1 outline-gray-300 placeholder:text-gray-400 focus:outline focus:outline-2 focus:-outline-offset-2 focus:outline-indigo-600 sm:text-sm"
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

        <div v-if="loginError" class="text-red-600 text-sm text-center">
          {{ loginError }}
        </div>

        <div>
          <button
            type="submit"
            class="flex w-full justify-center rounded-md bg-indigo-600 px-3 py-1.5 text-sm font-semibold text-white shadow-sm hover:bg-indigo-500 focus-visible:outline focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-indigo-600"
          >
            Sign in
          </button>
        </div>
      </form>

      <p class="mt-4 text-center text-sm text-gray-600">
        Don’t have an account?
        <a href="/Registration" class="font-semibold text-indigo-600 hover:text-indigo-500">Register as a voter</a>
      </p>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from "vue";
import { useRouter } from "vue-router";
import loginService from "../service/loginService";

// State
const email = ref("");
const password = ref("");
const loginError = ref("");
const showPassword = ref(false);

// Router
const router = useRouter();
const service = new loginService();

// Toggle password visibility
const togglePassword = () => {
  showPassword.value = !showPassword.value;
};

// Handle login
const handleLogin = async () => {
  loginError.value = "";

  try {
    const response = await service.login(email.value, password.value);

    if (response.data.token) {
      localStorage.setItem("token", response.data.token);
      localStorage.setItem("voterid", response.data.voterid);
      router.push("/dashboard/voter");
    } else {
      throw new Error("Token not found in response");
    }
  } catch (error) {
    console.error("Login failed:", error);
    loginError.value = "Invalid email or password. Please try again.";
  }
};
</script>
