<template>
    <v-container fluid>
      <h1 class="text-h4 mb-6">Election Management System</h1>
  
      <!-- Election List -->
      <v-card>
        <v-card-title>
          Election List
          <v-spacer></v-spacer>
          <v-btn color="primary" @click="openElectionDialog()">
            Add New Election
          </v-btn>
        </v-card-title>
        <v-data-table
          :headers="headers"
          :items="elections"
          :items-per-page="5"
          class="elevation-1"
        >
          <template v-slot:item.status="{ item }">
            <v-chip
              :color="getStatusColor(item.status)"
              text-color="white"
            >
              {{ item.status }}
            </v-chip>
          </template>
          <template v-slot:item.actions="{ item }">
            <v-icon small class="mr-2" @click="editElection(item)">
              mdi-pencil
            </v-icon>
            <v-icon small @click="deleteElection(item)">
              mdi-delete
            </v-icon>
          </template>
        </v-data-table>
      </v-card>
  
      <!-- Election Setup Dialog -->
      <v-dialog v-model="dialog" max-width="600px">
        <v-card>
          <v-card-title>
            <span class="text-h5">{{ formTitle }}</span>
          </v-card-title>
          <v-card-text>
            <v-form ref="form" v-model="valid" lazy-validation>
              <v-text-field
                v-model="editedItem.name"
                label="Election Name"
                required
              ></v-text-field>
              <v-row>
                <v-col cols="12" sm="6">
                  <v-menu
                    ref="startMenu"
                    v-model="startMenu"
                    :close-on-content-click="false"
                    transition="scale-transition"
                    offset-y
                    min-width="auto"
                  >
                    <template v-slot:activator="{ on, attrs }">
                      <v-text-field
                        v-model="editedItem.startDate"
                        label="Start Date"
                        prepend-icon="mdi-calendar"
                        readonly
                        v-bind="attrs"
                        v-on="on"
                      ></v-text-field>
                    </template>
                    <v-date-picker
                      v-model="editedItem.startDate"
                      @input="startMenu = false"
                    ></v-date-picker>
                  </v-menu>
                </v-col>
                <v-col cols="12" sm="6">
                  <v-menu
                    ref="endMenu"
                    v-model="endMenu"
                    :close-on-content-click="false"
                    transition="scale-transition"
                    offset-y
                    min-width="auto"
                  >
                    <template v-slot:activator="{ on, attrs }">
                      <v-text-field
                        v-model="editedItem.endDate"
                        label="End Date"
                        prepend-icon="mdi-calendar"
                        readonly
                        v-bind="attrs"
                        v-on="on"
                      ></v-text-field>
                    </template>
                    <v-date-picker
                      v-model="editedItem.endDate"
                      @input="endMenu = false"
                    ></v-date-picker>
                  </v-menu>
                </v-col>
              </v-row>
              <v-combobox
                v-model="editedItem.positions"
                label="Positions Contested"
                multiple
                chips
                small-chips
                deletable-chips
              ></v-combobox>
              <v-combobox
                v-model="editedItem.regions"
                label="Eligible Regions/Districts"
                multiple
                chips
                small-chips
                deletable-chips
              ></v-combobox>
            </v-form>
          </v-card-text>
          <v-card-actions>
            <v-spacer></v-spacer>
            <v-btn color="blue darken-1" text @click="close">
              Cancel
            </v-btn>
            <v-btn color="blue darken-1" text @click="save">
              Save
            </v-btn>
          </v-card-actions>
        </v-card>
      </v-dialog>
  
      <!-- Delete Confirmation Dialog -->
      <v-dialog v-model="deleteDialog" max-width="400px">
        <v-card>
          <v-card-title class="text-h5">Delete Election</v-card-title>
          <v-card-text>Are you sure you want to delete this election?</v-card-text>
          <v-card-actions>
            <v-spacer></v-spacer>
            <v-btn color="blue darken-1" text @click="deleteDialog = false">Cancel</v-btn>
            <v-btn color="red darken-1" text @click="confirmDelete">Delete</v-btn>
          </v-card-actions>
        </v-card>
      </v-dialog>
  
      <!-- Voting Control and Election Monitoring -->
      <v-card class="mt-6">
        <v-card-title>Election Control and Monitoring</v-card-title>
        <v-card-text>
          <v-row>
            <v-col cols="12" sm="6">
              <h3 class="text-h6 mb-2">Voting Control</h3>
              <v-btn-toggle v-model="selectedElection" mandatory>
                <v-btn v-for="election in elections" :key="election.id" :value="election.id">
                  {{ election.name }}
                </v-btn>
              </v-btn-toggle>
              <v-btn-group class="mt-4">
                <v-btn color="success" @click="startElection" :disabled="!canStart">Start</v-btn>
                <v-btn color="warning" @click="pauseElection" :disabled="!canPause">Pause</v-btn>
                <v-btn color="error" @click="endElection" :disabled="!canEnd">End</v-btn>
              </v-btn-group>
              <p class="mt-2">Current Status: {{ currentStatus }}</p>
            </v-col>
            <v-col cols="12" sm="6">
              <h3 class="text-h6 mb-2">Real-time Stats</h3>
              <v-list dense>
                <v-list-item>
                  <v-list-item-content>
                    <v-list-item-title>Votes Cast</v-list-item-title>
                    <v-list-item-subtitle>{{ currentElection ? currentElection.totalVotes : 'N/A' }}</v-list-item-subtitle>
                  </v-list-item-content>
                </v-list-item>
                <v-list-item>
                  <v-list-item-content>
                    <v-list-item-title>Voter Turnout</v-list-item-title>
                    <v-list-item-subtitle>{{ voterTurnout }}%</v-list-item-subtitle>
                  </v-list-item-content>
                </v-list-item>
                <v-list-item>
                  <v-list-item-content>
                    <v-list-item-title>Total Candidates</v-list-item-title>
                    <v-list-item-subtitle>{{ currentElection ? currentElection.totalCandidates : 'N/A' }}</v-list-item-subtitle>
                  </v-list-item-content>
                </v-list-item>
              </v-list>
            </v-col>
          </v-row>
        </v-card-text>
      </v-card>
  
      <!-- Export Options -->
      <v-card class="mt-6">
        <v-card-title>Export Options</v-card-title>
        <v-card-text>
          <v-btn color="primary" class="mr-2" @click="exportAsPDF">Export as PDF</v-btn>
          <v-btn color="primary" class="mr-2" @click="exportAsExcel">Export as Excel</v-btn>
          <v-btn color="primary" @click="printAsCSV">Print as CSV</v-btn>
        </v-card-text>
      </v-card>
    </v-container>
  </template>
  
  <script>
  import { ref, reactive, computed } from 'vue'
  
  export default {
    name: 'ElectionManagement',
    setup() {
      const dialog = ref(false)
      const deleteDialog = ref(false)
      const form = ref(null)
      const valid = ref(true)
      const startMenu = ref(false)
      const endMenu = ref(false)
  
      const headers = [
        { text: 'Election Name', value: 'name' },
        { text: 'Start Date', value: 'startDate' },
        { text: 'End Date', value: 'endDate' },
        { text: 'Status', value: 'status' },
        { text: 'Total Candidates', value: 'totalCandidates' },
        { text: 'Total Votes Cast', value: 'totalVotes' },
        { text: 'Actions', value: 'actions', sortable: false },
      ]
  
      const elections = ref([
        {
          id: 1,
          name: 'City Council Election 2023',
          startDate: '2023-09-01',
          endDate: '2023-09-15',
          status: 'Upcoming',
          totalCandidates: 12,
          totalVotes: 0,
          positions: ['Mayor', 'Council Member'],
          regions: ['Downtown', 'Suburbs', 'Industrial District'],
        },
        {
          id: 2,
          name: 'State Governor Election 2023',
          startDate: '2023-11-01',
          endDate: '2023-11-15',
          status: 'Upcoming',
          totalCandidates: 5,
          totalVotes: 0,
          positions: ['Governor'],
          regions: ['North Region', 'South Region', 'Central Region'],
        },
      ])
  
      const editedIndex = ref(-1)
      const editedItem = reactive({
        id: null,
        name: '',
        startDate: '',
        endDate: '',
        status: 'Upcoming',
        totalCandidates: 0,
        totalVotes: 0,
        positions: [],
        regions: [],
      })
      const defaultItem = {
        id: null,
        name: '',
        startDate: '',
        endDate: '',
        status: 'Upcoming',
        totalCandidates: 0,
        totalVotes: 0,
        positions: [],
        regions: [],
      }
  
      const selectedElection = ref(null)
      const currentElection = computed(() => elections.value.find(e => e.id === selectedElection.value))
      const currentStatus = computed(() => currentElection.value ? currentElection.value.status : 'No election selected')
      const voterTurnout = computed(() => {
        if (!currentElection.value) return 'N/A'
        // Assuming 1000 registered voters for this example
        return ((currentElection.value.totalVotes / 1000) * 100).toFixed(2)
      })
  
      const canStart = computed(() => currentElection.value && currentElection.value.status === 'Upcoming')
      const canPause = computed(() => currentElection.value && currentElection.value.status === 'Ongoing')
      const canEnd = computed(() => currentElection.value && (currentElection.value.status === 'Ongoing' || currentElection.value.status === 'Paused'))
  
      const formTitle = computed(() => {
        return editedIndex.value === -1 ? 'New Election' : 'Edit Election'
      })
  
      function getStatusColor(status) {
        if (status === 'Upcoming') return 'blue'
        if (status === 'Ongoing') return 'green'
        if (status === 'Paused') return 'orange'
        if (status === 'Completed') return 'gray'
        return 'primary'
      }
  
      function openElectionDialog(item) {
        if (item) {
          editedIndex.value = elections.value.indexOf(item)
          Object.assign(editedItem, item)
        } else {
          editedIndex.value = -1
          Object.assign(editedItem, defaultItem)
        }
        dialog.value = true
      }
  
      function editElection(item) {
        openElectionDialog(item)
      }
  
      function deleteElection(item) {
        editedIndex.value = elections.value.indexOf(item)
        deleteDialog.value = true
      }
  
      function close() {
        dialog.value = false
        nextTick(() => {
          Object.assign(editedItem, defaultItem)
          editedIndex.value = -1
        })
      }
  
      function save() {
        if (editedIndex.value > -1) {
          Object.assign(elections.value[editedIndex.value], editedItem)
        } else {
          editedItem.id = elections.value.length + 1
          elections.value.push(editedItem)
        }
        close()
      }
  
      function confirmDelete() {
        elections.value.splice(editedIndex.value, 1)
        deleteDialog.value = false
      }
  
      function startElection() {
        if (currentElection.value) {
          currentElection.value.status = 'Ongoing'
        }
      }
  
      function pauseElection() {
        if (currentElection.value) {
          currentElection.value.status = 'Paused'
        }
      }
  
      function endElection() {
        if (currentElection.value) {
          currentElection.value.status = 'Completed'
        }
      }
  
      function exportAsPDF() {
        alert('Exporting as PDF...')
        // Implement PDF export logic here
      }
  
      function exportAsExcel() {
        alert('Exporting as Excel...')
        // Implement Excel export logic here
      }
  
      function printAsCSV() {
        if (!currentElection.value) {
          alert('Please select an election first.')
          return
        }
  
        const election = currentElection.value
        const csvContent = `
          Election Name,${election.name}
          Start Date,${election.startDate}
          End Date,${election.endDate}
          Status,${election.status}
          Total Candidates,${election.totalCandidates}
          Total Votes Cast,${election.totalVotes}
          Positions Contested,${election.positions.join('; ')}
          Eligible Regions/Districts,${election.regions.join('; ')}
        `.trim()
  
        const blob = new Blob([csvContent], { type: 'text/csv;charset=utf-8;' })
        const link = document.createElement('a')
        if (link.download !== undefined) {
          const url = URL.createObjectURL(blob)
          link.setAttribute('href', url)
          link.setAttribute('download', `${election.name}_details.csv`)
          link.style.visibility = 'hidden'
          document.body.appendChild(link)
          link.click()
          document.body.removeChild(link)
        }
      }
  
      return {
        dialog,
        deleteDialog,
        form,
        valid,
        startMenu,
        endMenu,
        headers,
        elections,
        editedIndex,
        editedItem,
        formTitle,
        selectedElection,
        currentElection,
        currentStatus,
        voterTurnout,
        canStart,
        canPause,
        canEnd,
        getStatusColor,
        openElectionDialog,
        editElection,
        deleteElection,
        close,
        save,
        confirmDelete,
        startElection,
        pauseElection,
        endElection,
        exportAsPDF,
        exportAsExcel,
        printAsCSV,
      }
    },
  }
  </script>