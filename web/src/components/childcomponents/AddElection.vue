<script setup lang="ts">
import { ref } from 'vue'
import { PlusCircle, X } from 'lucide-vue-next'

interface Candidate {
  id: number;
  name: string;
}

interface ElectionData {
  name: string;
  startDate: string;
  endDate: string;
  candidates: Candidate[];
}

const electionData = ref<ElectionData>({
  name: '',
  startDate: '',
  endDate: '',
  candidates: []
})

const newCandidate = ref('')
let nextCandidateId = 1

const addCandidate = () => {
  if (newCandidate.value.trim()) {
    electionData.value.candidates.push({
      id: nextCandidateId++,
      name: newCandidate.value.trim()
    })
    newCandidate.value = ''
  }
}

const removeCandidate = (id: number) => {
  electionData.value.candidates = electionData.value.candidates.filter(c => c.id !== id)
}

const submitForm = () => {
  console.log('Submitting election data:', electionData.value)
  // Reset the form after submission
  electionData.value = {
    name: '',
    startDate: '',
    endDate: '',
    candidates: []
  }
nextCandidateId = 1
}

defineEmits(['close'])
</script>

<template>
  <div class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center p-4">
    <div class="bg-white rounded-lg shadow-xl p-6 w-full max-w-md">
      <h2 class="text-2xl font-bold mb-4">Add New Election</h2>
      <form @submit.prevent="submitForm">
        <div class="space-y-4">
          <div>
            <label for="election-name" class="block text-sm font-medium text-gray-700 mb-1">Election Name</label>
            <input id="election-name" v-model="electionData.name" required
                   class="mt-1 block w-full px-3 py-2 bg-white border border-gray-300 rounded-md text-sm shadow-sm placeholder-gray-400
                          focus:outline-none focus:border-sky-500 focus:ring-1 focus:ring-sky-500" />
          </div>
          <div>
            <label for="start-date" class="block text-sm font-medium text-gray-700 mb-1">Start Date</label>
            <input id="start-date" type="date" v-model="electionData.startDate" required
                   class="mt-1 block w-full px-3 py-2 bg-white border border-gray-300 rounded-md text-sm shadow-sm placeholder-gray-400
                          focus:outline-none focus:border-sky-500 focus:ring-1 focus:ring-sky-500" />
          </div>
          <div>
            <label for="end-date" class="block text-sm font-medium text-gray-700 mb-1">End Date</label>
            <input id="end-date" type="date" v-model="electionData.endDate" required
                   class="mt-1 block w-full px-3 py-2 bg-white border border-gray-300 rounded-md text-sm shadow-sm placeholder-gray-400
                          focus:outline-none focus:border-sky-500 focus:ring-1 focus:ring-sky-500" />
          </div>
          <div>
            <label for="new-candidate" class="block text-sm font-medium text-gray-700 mb-1">Add Candidate</label>
            <div class="flex space-x-2">
              <input id="new-candidate" v-model="newCandidate" placeholder="Enter candidate name"
                     class="mt-1 block w-full px-3 py-2 bg-white border border-gray-300 rounded-md text-sm shadow-sm placeholder-gray-400
                            focus:outline-none focus:border-sky-500 focus:ring-1 focus:ring-sky-500" />
              <button type="button" @click="addCandidate"
                      class="inline-flex items-center px-4 py-2 border border-transparent text-sm font-medium rounded-md shadow-sm text-white bg-blue-600 hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500">
                <PlusCircle class="w-4 h-4" />
              </button>
            </div>
          </div>
          <div v-if="electionData.candidates.length > 0">
            <h3 class="font-semibold mb-2">Candidates:</h3>
            <ul class="space-y-2">
              <li v-for="candidate in electionData.candidates" :key="candidate.id" class="flex items-center justify-between bg-gray-100 p-2 rounded">
                <span>{{ candidate.name }}</span>
                <button @click="removeCandidate(candidate.id)"
                        class="text-red-500 hover:text-red-700 focus:outline-none">
                  <X class="w-4 h-4" />
                </button>
              </li>
            </ul>
          </div>
        </div>
        <div class="mt-6 flex justify-end space-x-2">
          <button type="button" @click="$emit('close')"
                  class="px-4 py-2 border border-gray-300 rounded-md shadow-sm text-sm font-medium text-gray-700 bg-white hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500">
            Cancel
          </button>
          <button type="submit"
                  class="inline-flex justify-center py-2 px-4 border border-transparent shadow-sm text-sm font-medium rounded-md text-white bg-blue-600 hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500">
            Create Election
          </button>
        </div>
      </form>
    </div>
  </div>
</template>

