<template>
  <div class="flex min-h-full flex-1 flex-col justify-center px-6 py-12 lg:px-8">
    <div class="sm:mx-auto sm:w-full sm:max-w-sm">
      <img
        class="mx-auto h-10 w-auto"
        src="https://tailwindui.com/plus/img/logos/mark.svg?color=indigo&shade=600"
        alt="Your Company"
      />
      <h2 class="mt-10 text-center text-2xl/9 font-bold tracking-tight text-gray-900">
        Sign in to your account
      </h2>
    </div>

    <div class="mt-10 sm:mx-auto sm:w-full sm:max-w-sm">
      <form class="space-y-6" @submit.prevent="handleLogin">
        <div>
          <label for="email" class="block text-sm/6 font-medium text-gray-900">Email address</label>
          <div class="mt-2">
            <input
              type="email"
              v-model="email"
              id="email"
              autocomplete="email"
              class="block w-full rounded-md bg-white px-3 py-1.5 text-base text-gray-900 outline outline-1 -outline-offset-1 outline-gray-300 placeholder:text-gray-400 focus:outline focus:outline-2 focus:-outline-offset-2 focus:outline-indigo-600 sm:text-sm/6"
              required
            />
          </div>
        </div>

        <div>
          <div class="flex items-center justify-between">
            <label for="password" class="block text-sm/6 font-medium text-gray-900">Password</label>
          </div>
          <div class="mt-2">
            <input
              type="password"
              v-model="password"
              id="password"
              autocomplete="current-password"
              class="block w-full rounded-md bg-white px-3 py-1.5 text-base text-gray-900 outline outline-1 -outline-offset-1 outline-gray-300 placeholder:text-gray-400 focus:outline focus:outline-2 focus:-outline-offset-2 focus:outline-indigo-600 sm:text-sm/6"
              required
            />
          </div>
        </div>

        <div>
          <button
            type="submit"
            class="flex w-full justify-center rounded-md bg-indigo-600 px-3 py-1.5 text-sm/6 font-semibold text-white shadow-sm hover:bg-indigo-500 focus-visible:outline focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-indigo-600"
          >
            Sign in
          </button>
        </div>
      </form>


    </div>

  </div>
</template><script setup lang="ts">
import { ref } from "vue";
import { useRouter } from "vue-router";
import loginService from "../service/loginService";


const email = ref("");
const password = ref("");
const loginError = ref("");
const service = new loginService();
const router = useRouter();

const handleLogin = async () => {
  loginError.value = ""; 

  try {
    const response = await service.login(email.value, password.value);

    if (response.data.token) {
      localStorage.setItem("token", response.data.token);
      console.log("Token saved:", response.data.token);

      // Store the voterid in localStorage
      localStorage.setItem("voterid", response.data.voterid);
      console.log("Voter ID saved:", response.data.voterid);
      
      // Navigate to the dashboard
      router.push(`/dashboard/voter`);
    } else {
      throw new Error("Token not found in response");
    }
  } catch (error) {
    console.error("Login failed:", error);
    loginError.value = "Invalid email or password. Please try again.";
  }
};
</script>
