<template>
  <div class="min-h-screen bg-gray-100">
    <MenuComponent></MenuComponent>
    <div class="p-8">
      <h1 class="text-3xl font-bold text-gray-800 mb-8">Election Results and Analysis</h1>

      <!-- Filters Section -->
      <section class="bg-white rounded-lg shadow p-6 mb-8">
        <h2 class="text-2xl font-semibold text-gray-800 mb-4">Filters</h2>
        <div class="grid grid-cols-1 md:grid-cols-3 gap-6">
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-2">Election</label>
            <select
              v-model="filters.region"
              class="w-full bg-gray-50 border border-gray-300 rounded-lg p-2 focus:ring-blue-500 focus:border-blue-500"
            >
              <option v-for="region in regions" :key="region" :value="region">
                {{ region }}
              </option>
            </select>
          </div>
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-2">Centres</label>
            <select
              v-model="filters.position"
              class="w-full bg-gray-50 border border-gray-300 rounded-lg p-2 focus:ring-blue-500 focus:border-blue-500"
            >
              <option v-for="position in positions" :key="position" :value="position">
                {{ position }}
              </option>
            </select>
          </div>
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-2">Search</label>
            <button>Search</button>
          </div>
        </div>
      </section>

      <!-- Overall Results Section -->
      <section class="bg-white rounded-lg shadow p-6 mb-8">
        <h2 class="text-2xl font-semibold text-gray-800 mb-4">Overall Results</h2>
        <div class="overflow-x-auto">
          <table class="min-w-full bg-white border border-gray-200">
            <thead>
              <tr class="bg-gray-50">
                <th class="px-6 py-3 text-left text-sm font-semibold text-gray-600 uppercase">Candidate</th>
                <th class="px-6 py-3 text-left text-sm font-semibold text-gray-600 uppercase">Party</th>
                <th class="px-6 py-3 text-left text-sm font-semibold text-gray-600 uppercase">Votes</th>
                <th class="px-6 py-3 text-left text-sm font-semibold text-gray-600 uppercase">Percentage</th>
              </tr>
            </thead>
            <tbody>
              <tr
                v-for="item in filteredOverallResults"
                :key="item.name"
                :class="{ 'bg-green-100': item.isWinner, 'bg-white': !item.isWinner }"
                class="border-b hover:bg-gray-100"
              >
                <td class="px-6 py-4 text-gray-800">{{ item.name }}</td>
                <td class="px-6 py-4 text-gray-800">{{ item.party }}</td>
                <td class="px-6 py-4 text-gray-800">{{ item.votes }}</td>
                <td class="px-6 py-4 text-gray-800">{{ item.percentage.toFixed(2) }}%</td>
              </tr>
            </tbody>
          </table>
        </div>
      </section>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import MenuComponent from '../components/childcomponents/MenuComponent.vue'

const filters = ref({
  region: '',
  position: '',
  party: '',
})

const regions = ['Region 1', 'Region 2', 'Region 3']
const positions = ['President', 'Secretary', 'Treasurer']

const overallResultsHeaders = [
  { text: 'Candidate', value: 'name' },
  { text: 'Party', value: 'party' },
  { text: 'Votes', value: 'votes' },
  { text: 'Percentage', value: 'percentage' },
]

const filteredOverallResults = ref([
  { name: 'John Doe', party: 'Party A', votes: 450, percentage: 50.0, isWinner: true },
  { name: 'Jane Smith', party: 'Party B', votes: 300, percentage: 33.3, isWinner: false },
  { name: 'Alex Johnson', party: 'Party C', votes: 150, percentage: 16.7, isWinner: false },
])
</script>

<style>
/* Additional custom styles if needed */
</style>
