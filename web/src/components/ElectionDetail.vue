<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { ChevronLeft, Users, Calendar, CheckSquare, BarChart2 } from 'lucide-vue-next'

interface Candidate {
  id: number;
  name: string;
  votes: number;
}

interface ElectionDetails {
  id: number;
  name: string;
  startDate: string;
  endDate: string;
  totalVoters: number;
  totalVotes: number;
  candidates: Candidate[];
}

const props = defineProps<{
  electionId: number;
}>()

const election = ref<ElectionDetails | null>(null)

const fetchElectionDetails = async (id: number) => {
//static datas
  election.value = {
    id: id,
    name: "Student Council Election 2023",
    startDate: "2023-05-01",
    endDate: "2023-05-07",
    totalVoters: 1000,
    totalVotes: 750,
    candidates: [
      { id: 1, name: "Alice Johnson", votes: 300 },
      { id: 2, name: "Bob Smith", votes: 250 },
      { id: 3, name: "Charlie Brown", votes: 200 },
    ]
  }
}

onMounted(() => {
  fetchElectionDetails(props.electionId)
})

const calculatePercentage = (votes: number) => {
  if (!election.value) return 0
  return ((votes / election.value.totalVotes) * 100).toFixed(1)
}

const emits = defineEmits(['back'])

const goBack = () => {
  emits('back')
}
</script>

<template>
  <div v-if="election" class="bg-white shadow rounded-lg p-6">
    <div class="flex items-center mb-6">
      <button @click="goBack" class="text-blue-500 hover:text-blue-600 flex items-center">
        <ChevronLeft class="w-5 h-5 mr-1" />
        Back to Dashboard
      </button>
    </div>
    <h2 class="text-2xl font-bold text-gray-800 mb-4">{{ election.name }}</h2>
    <div class="grid grid-cols-1 md:grid-cols-2 gap-6 mb-6">
      <div class="bg-gray-50 p-4 rounded-lg flex items-center">
        <Calendar class="w-8 h-8 text-blue-500 mr-3" />
        <div>
          <p class="text-sm text-gray-500">Election Period</p>
          <p class="font-semibold">{{ election.startDate }} to {{ election.endDate }}</p>
        </div>
      </div>
      <div class="bg-gray-50 p-4 rounded-lg flex items-center">
        <Users class="w-8 h-8 text-green-500 mr-3" />
        <div>
          <p class="text-sm text-gray-500">Total Voters</p>
          <p class="font-semibold">{{ election.totalVoters }}</p>
        </div>
      </div>
      <div class="bg-gray-50 p-4 rounded-lg flex items-center">
        <CheckSquare class="w-8 h-8 text-purple-500 mr-3" />
        <div>
          <p class="text-sm text-gray-500">Votes Cast</p>
          <p class="font-semibold">{{ election.totalVotes }}</p>
        </div>
      </div>
      <div class="bg-gray-50 p-4 rounded-lg flex items-center">
        <BarChart2 class="w-8 h-8 text-yellow-500 mr-3" />
        <div>
          <p class="text-sm text-gray-500">Voter Turnout</p>
          <p class="font-semibold">{{ ((election.totalVotes / election.totalVoters) * 100).toFixed(1) }}%</p>
        </div>
      </div>
    </div>
    <h3 class="text-xl font-semibold text-gray-800 mb-4">Candidate Results</h3>
    <div class="space-y-4">
      <div v-for="candidate in election.candidates" :key="candidate.id" class="bg-gray-50 p-4 rounded-lg">
        <div class="flex justify-between items-center mb-2">
          <span class="font-semibold">{{ candidate.name }}</span>
          <span class="text-sm text-gray-500">{{ candidate.votes }} votes ({{ calculatePercentage(candidate.votes) }}%)</span>
        </div>
        <div class="w-full bg-gray-200 rounded-full h-2.5">
          <div class="bg-blue-600 h-2.5 rounded-full" :style="{ width: `${calculatePercentage(candidate.votes)}%` }"></div>
        </div>
      </div>
    </div>
  </div>
</template>

