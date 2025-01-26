<template>
    <v-container fluid>
      <h1 class="text-h4 mb-6">Activity and Error Logging System</h1>
  
      <!-- Search and Filters -->
      <v-card class="mb-6">
        <v-card-title>Search and Filters</v-card-title>
        <v-card-text>
          <v-row>
            <v-col cols="12" sm="4">
              <v-text-field
                v-model="search.userId"
                label="User ID"
                clearable
              ></v-text-field>
            </v-col>
            <v-col cols="12" sm="4">
              <v-select
                v-model="search.actionType"
                :items="actionTypes"
                label="Action Type"
                clearable
              ></v-select>
            </v-col>
            <v-col cols="12" sm="4">
              <v-checkbox
                v-model="search.criticalOnly"
                label="Critical Logs Only"
              ></v-checkbox>
            </v-col>
          </v-row>
          <v-row>
            <v-col cols="12" sm="6">
              <v-menu
                ref="startMenu"
                v-model="startMenu"
                :close-on-content-click="false"
                transition="scale-transition"
                offset-y
                max-width="290px"
                min-width="auto"
              >
                <template v-slot:activator="{ on, attrs }">
                  <v-text-field
                    v-model="search.startDate"
                    label="Start Date"
                    prepend-icon="mdi-calendar"
                    readonly
                    v-bind="attrs"
                    v-on="on"
                  ></v-text-field>
                </template>
                <v-date-picker
                  v-model="search.startDate"
                  no-title
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
                max-width="290px"
                min-width="auto"
              >
                <template v-slot:activator="{ on, attrs }">
                  <v-text-field
                    v-model="search.endDate"
                    label="End Date"
                    prepend-icon="mdi-calendar"
                    readonly
                    v-bind="attrs"
                    v-on="on"
                  ></v-text-field>
                </template>
                <v-date-picker
                  v-model="search.endDate"
                  no-title
                  @input="endMenu = false"
                ></v-date-picker>
              </v-menu>
            </v-col>
          </v-row>
        </v-card-text>
      </v-card>
  
      <!-- Activity Logs Table -->
      <v-card class="mb-6">
        <v-card-title>
          Activity Logs
          <v-spacer></v-spacer>
          <v-text-field
            v-model="search.keyword"
            append-icon="mdi-magnify"
            label="Search"
            single-line
            hide-details
          ></v-text-field>
        </v-card-title>
        <v-data-table
          :headers="activityHeaders"
          :items="filteredActivityLogs"
          :search="search.keyword"
          :items-per-page="10"
          class="elevation-1"
        >
          <template v-slot:item.timestamp="{ item }">
            {{ formatDate(item.timestamp) }}
          </template>
        </v-data-table>
      </v-card>
  
      <!-- Error Logs -->
      <v-card class="mb-6">
        <v-card-title>Error Logs</v-card-title>
        <v-data-table
          :headers="errorHeaders"
          :items="errorLogs"
          :items-per-page="5"
          class="elevation-1"
        >
          <template v-slot:item.timestamp="{ item }">
            {{ formatDate(item.timestamp) }}
          </template>
          <template v-slot:item.severity="{ item }">
            <v-chip :color="getSeverityColor(item.severity)" dark>
              {{ item.severity }}
            </v-chip>
          </template>
        </v-data-table>
      </v-card>
  
      <!-- Export Options -->
      <v-card>
        <v-card-title>Export Options</v-card-title>
        <v-card-text>
          <v-btn color="primary" class="mr-4" @click="exportActivityLogs">
            Export Activity Logs
          </v-btn>
          <v-btn color="error" @click="exportErrorLogs">
            Export Error Logs
          </v-btn>
        </v-card-text>
      </v-card>
    </v-container>
  </template>
  
  <script>
  import { ref, computed } from 'vue'
  
  export default {
    name: 'ActivityErrorLogs',
    setup() {
      const search = ref({
        userId: '',
        actionType: '',
        startDate: '',
        endDate: '',
        criticalOnly: false,
        keyword: '',
      })
  
      const startMenu = ref(false)
      const endMenu = ref(false)
  
      const actionTypes = [
        'Voter Management',
        'Candidate Management',
        'Election Control',
        'System Configuration',
      ]
  
      const activityHeaders = [
        { text: 'Action', value: 'action' },
        { text: 'User ID', value: 'userId' },
        { text: 'Timestamp', value: 'timestamp' },
        { text: 'Details', value: 'details' },
      ]
  
      const errorHeaders = [
        { text: 'Error Description', value: 'description' },
        { text: 'Timestamp', value: 'timestamp' },
        { text: 'Severity', value: 'severity' },
      ]
  
      // Sample activity logs data
      const activityLogs = ref([
        { action: 'Voter added', userId: 'admin1', timestamp: new Date('2023-05-01T10:30:00'), details: 'New voter registered' },
        { action: 'Candidate deleted', userId: 'admin2', timestamp: new Date('2023-05-02T14:45:00'), details: 'Candidate withdrawn from election' },
        { action: 'Election started', userId: 'admin1', timestamp: new Date('2023-05-03T09:00:00'), details: 'City Council Election 2023 started' },
        { action: 'Failed login attempt', userId: 'unknown', timestamp: new Date('2023-05-04T22:15:00'), details: 'Multiple failed login attempts detected' },
        { action: 'System configuration changed', userId: 'admin3', timestamp: new Date('2023-05-05T11:20:00'), details: 'Updated vote counting algorithm' },
      ])
  
      // Sample error logs data
      const errorLogs = ref([
        { description: 'Database connection timeout', timestamp: new Date('2023-05-02T03:45:00'), severity: 'High' },
        { description: 'Invalid input data for vote submission', timestamp: new Date('2023-05-03T14:30:00'), severity: 'Medium' },
        { description: 'Memory usage exceeded threshold', timestamp: new Date('2023-05-04T19:20:00'), severity: 'Low' },
      ])
  
      const filteredActivityLogs = computed(() => {
        return activityLogs.value.filter(log => {
          const matchUserId = !search.value.userId || log.userId.includes(search.value.userId)
          const matchActionType = !search.value.actionType || log.action.includes(search.value.actionType)
          const matchDate = (!search.value.startDate || new Date(log.timestamp) >= new Date(search.value.startDate)) &&
                            (!search.value.endDate || new Date(log.timestamp) <= new Date(search.value.endDate))
          const matchCritical = !search.value.criticalOnly || log.action.toLowerCase().includes('failed') || log.action.toLowerCase().includes('error')
  
          return matchUserId && matchActionType && matchDate && matchCritical
        })
      })
  
      function formatDate(date) {
        return new Date(date).toLocaleString()
      }
  
      function getSeverityColor(severity) {
        switch (severity.toLowerCase()) {
          case 'high':
            return 'red'
          case 'medium':
            return 'orange'
          case 'low':
            return 'green'
          default:
            return 'grey'
        }
      }
  
      function exportActivityLogs() {
        exportToCSV(filteredActivityLogs.value, 'activity_logs.csv')
      }
  
      function exportErrorLogs() {
        exportToCSV(errorLogs.value, 'error_logs.csv')
      }
  
      function exportToCSV(data, filename) {
        const csvContent = data.map(item => Object.values(item).join(',')).join('\n')
        const blob = new Blob([csvContent], { type: 'text/csv;charset=utf-8;' })
        const link = document.createElement('a')
        if (link.download !== undefined) {
          const url = URL.createObjectURL(blob)
          link.setAttribute('href', url)
          link.setAttribute('download', filename)
          link.style.visibility = 'hidden'
          document.body.appendChild(link)
          link.click()
          document.body.removeChild(link)
        }
      }
  
      return {
        search,
        startMenu,
        endMenu,
        actionTypes,
        activityHeaders,
        errorHeaders,
        filteredActivityLogs,
        errorLogs,
        formatDate,
        getSeverityColor,
        exportActivityLogs,
        exportErrorLogs,
      }
    },
  }
  </script>