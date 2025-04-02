<template>
    <v-card class="mb-6 rounded-lg shadow-md">
      <v-card-title class="text-h5 font-medium d-flex align-center">
        Your Voting History
        <v-spacer></v-spacer>
        <v-text-field
          v-model="searchQuery"
          append-inner-icon="mdi-magnify"
          label="Search history"
          density="compact"
          hide-details
          variant="outlined"
          class="max-w-xs"
        ></v-text-field>
      </v-card-title>
      
      <v-card-text>
        <div class="mb-4">
          <v-chip-group v-model="selectedFilter">
            <v-chip filter value="all">All</v-chip>
            <v-chip filter value="presidential">Presidential</v-chip>
            <v-chip filter value="local">Local</v-chip>
            <v-chip filter value="referendum">Referendum</v-chip>
          </v-chip-group>
        </div>
        
        <v-timeline v-if="filteredHistory.length > 0" align-top dense>
          <v-timeline-item
            v-for="(history, index) in filteredHistory"
            :key="index"
            :dot-color="history.partyColor"
            size="small"
          >
            <template v-slot:opposite>
              <div class="text-caption text-grey">
                {{ history.date }}
              </div>
            </template>
            
            <v-card class="elevation-1 mb-3">
              <v-card-text>
                <div class="d-flex align-start">
                  <v-avatar size="48" class="mr-4">
                    <v-img :src="history.candidateAvatar" :alt="history.candidate"></v-img>
                  </v-avatar>
                  
                  <div class="flex-grow-1">
                    <div class="d-flex align-center">
                      <div class="text-h6">{{ history.election }}</div>
                      <v-chip
                        size="x-small"
                        :color="getElectionTypeColor(history.electionType)"
                        class="ml-2"
                      >
                        {{ history.electionType }}
                      </v-chip>
                    </div>
                    
                    <div class="d-flex align-center mt-1">
                      <div class="font-weight-medium">{{ history.candidate }}</div>
                      <v-chip
                        size="x-small"
                        :color="history.partyColor"
                        class="ml-2"
                      >
                        {{ history.party }}
                      </v-chip>
                    </div>
                    
                    <div class="mt-2 text-body-2 text-grey-darken-1">
                      <div class="d-flex align-center">
                        <v-icon size="small" class="mr-1">mdi-map-marker</v-icon>
                        <span>Voted at: {{ history.votingLocation }}</span>
                      </div>
                      
                      <div class="d-flex align-center mt-1">
                        <v-icon size="small" class="mr-1">mdi-check-circle</v-icon>
                        <span>Vote verified: {{ history.verificationTime }}</span>
                      </div>
                    </div>
                  </div>
                  
                  <v-btn
                    icon
                    variant="text"
                    size="small"
                    @click="expandedItem = expandedItem === index ? null : index"
                  >
                    <v-icon>{{ expandedItem === index ? 'mdi-chevron-up' : 'mdi-chevron-down' }}</v-icon>
                  </v-btn>
                </div>
                
                <v-expand-transition>
                  <div v-if="expandedItem === index" class="mt-3 pt-3 border-t border-gray-200">
                    <div class="text-body-2">
                      <div class="font-weight-medium mb-1">Election Details:</div>
                      <div class="ml-2 mb-2">
                        <div>Type: {{ history.electionType }}</div>
                        <div>Ballot ID: {{ history.ballotId }}</div>
                        <div>Voter ID: {{ history.voterId }}</div>
                      </div>
                      
                      <div class="font-weight-medium mb-1">Candidate Platform:</div>
                      <div class="ml-2 mb-2">
                        <p>{{ history.candidatePlatform }}</p>
                      </div>
                      
                      <div v-if="history.keyIssues && history.keyIssues.length > 0">
                        <div class="font-weight-medium mb-1">Key Issues:</div>
                        <div class="d-flex flex-wrap gap-1 ml-2">
                          <v-chip
                            v-for="(issue, i) in history.keyIssues"
                            :key="i"
                            size="x-small"
                            :color="history.partyColor"
                            variant="outlined"
                          >
                            {{ issue }}
                          </v-chip>
                        </div>
                      </div>
                    </div>
                    
                    <div class="d-flex justify-end mt-3">
                      <v-btn
                        size="small"
                        variant="outlined"
                        prepend-icon="mdi-file-document-outline"
                        @click="viewBallot(history)"
                      >
                        View Ballot
                      </v-btn>
                    </div>
                  </div>
                </v-expand-transition>
              </v-card-text>
            </v-card>
          </v-timeline-item>
        </v-timeline>
        
        <div v-else class="text-center py-8">
          <v-icon size="large" class="text-gray-400">mdi-history</v-icon>
          <div class="text-h6 mt-4">No Voting History Found</div>
          <div class="text-body-1 text-gray-600 dark:text-gray-400 mt-2">
            {{ searchQuery ? 'No results match your search criteria.' : 'You haven\'t voted in any elections yet.' }}
          </div>
          <v-btn
            color="primary"
            class="mt-4"
            @click="$emit('update:activeTab', 'elections')"
          >
            View Elections
          </v-btn>
        </div>
      </v-card-text>
      
      <!-- Ballot View Dialog -->
      <v-dialog v-model="showBallotDialog" max-width="600">
        <v-card>
          <v-card-title class="text-h5">
            Ballot Details
            <v-spacer></v-spacer>
            <v-btn icon @click="showBallotDialog = false">
              <v-icon>mdi-close</v-icon>
            </v-btn>
          </v-card-title>
          <v-card-text v-if="selectedBallot">
            <v-img
              src="https://via.placeholder.com/600x400?text=Official+Ballot"
              class="mb-4 rounded-lg"
              alt="Official Ballot"
            ></v-img>
            
            <div class="d-flex flex-column gap-2">
              <div class="d-flex">
                <div class="font-weight-medium min-w-[150px]">Election:</div>
                <div>{{ selectedBallot.election }}</div>
              </div>
              <div class="d-flex">
                <div class="font-weight-medium min-w-[150px]">Date:</div>
                <div>{{ selectedBallot.date }}</div>
              </div>
              <div class="d-flex">
                <div class="font-weight-medium min-w-[150px]">Ballot ID:</div>
                <div>{{ selectedBallot.ballotId }}</div>
              </div>
              <div class="d-flex">
                <div class="font-weight-medium min-w-[150px]">Voter ID:</div>
                <div>{{ selectedBallot.voterId }}</div>
              </div>
              <div class="d-flex">
                <div class="font-weight-medium min-w-[150px]">Voting Location:</div>
                <div>{{ selectedBallot.votingLocation }}</div>
              </div>
              <div class="d-flex">
                <div class="font-weight-medium min-w-[150px]">Selected Candidate:</div>
                <div>{{ selectedBallot.candidate }} ({{ selectedBallot.party }})</div>
              </div>
              <div class="d-flex">
                <div class="font-weight-medium min-w-[150px]">Verification Time:</div>
                <div>{{ selectedBallot.verificationTime }}</div>
              </div>
              <div class="d-flex">
                <div class="font-weight-medium min-w-[150px]">Verification Hash:</div>
                <div class="font-mono text-xs">{{ selectedBallot.verificationHash }}</div>
              </div>
            </div>
          </v-card-text>
          <v-card-actions>
            <v-spacer></v-spacer>
            <v-btn
              color="primary"
              variant="outlined"
              prepend-icon="mdi-printer"
            >
              Print Ballot
            </v-btn>
            <v-btn
              color="primary"
              prepend-icon="mdi-download"
            >
              Download PDF
            </v-btn>
          </v-card-actions>
        </v-card>
      </v-dialog>
    </v-card>
  </template>
  
  <script setup lang="ts">
  import { ref, computed } from 'vue';
  
  const props = defineProps({
    votingHistory: {
      type: Array,
      required: true
    }
  });
  
  const emit = defineEmits(['update:activeTab']);
  
  // State
  const searchQuery = ref('');
  const selectedFilter = ref('all');
  const expandedItem = ref(null);
  const showBallotDialog = ref(false);
  const selectedBallot = ref(null);
  
  // Enhanced voting history with more details
  const enhancedHistory = computed(() => {
    return props.votingHistory.map(history => {
      // Generate additional details for each history item
      const electionType = getElectionType(history.election);
      const votingLocation = getVotingLocation(history);
      const verificationTime = getVerificationTime(history.date);
      const ballotId = generateBallotId();
      const voterId = generateVoterId();
      const verificationHash = generateVerificationHash();
      const candidatePlatform = getCandidatePlatform(history.candidate, history.party);
      const keyIssues = getKeyIssues(history.candidate, history.party);
      
      return {
        ...history,
        electionType,
        votingLocation,
        verificationTime,
        ballotId,
        voterId,
        verificationHash,
        candidatePlatform,
        keyIssues
      };
    });
  });
  
  // Filtered history based on search and filter
  const filteredHistory = computed(() => {
    let result = enhancedHistory.value;
    
    // Apply search filter
    if (searchQuery.value) {
      const query = searchQuery.value.toLowerCase();
      result = result.filter(item => 
        item.election.toLowerCase().includes(query) ||
        item.candidate.toLowerCase().includes(query) ||
        item.party.toLowerCase().includes(query) ||
        item.electionType.toLowerCase().includes(query)
      );
    }
    
    // Apply type filter
    if (selectedFilter.value !== 'all') {
      result = result.filter(item => 
        item.electionType.toLowerCase() === selectedFilter.value
      );
    }
    
    return result;
  });
  
  // Helper functions
  const getElectionType = (electionName) => {
    if (electionName.includes('Presidential')) return 'Presidential';
    if (electionName.includes('Midterm')) return 'Federal';
    if (electionName.includes('City') || electionName.includes('Local')) return 'Local';
    if (electionName.includes('Referendum')) return 'Referendum';
    return 'Other';
  };
  
  const getElectionTypeColor = (type) => {
    switch (type) {
      case 'Presidential': return 'purple';
      case 'Federal': return 'deep-purple';
      case 'Local': return 'teal';
      case 'Referendum': return 'orange';
      default: return 'grey';
    }
  };
  
  const getVotingLocation = (history) => {
    // Mock voting locations based on election type
    const locations = {
      'Presidential': 'Downtown Community Center, 123 Main St',
      'Federal': 'Riverside Elementary School, 456 River Rd',
      'Local': 'Oakwood Library, 789 Oak Ave',
      'Referendum': 'Westside High School, 101 West Blvd'
    };
    
    const type = getElectionType(history.election);
    return locations[type] || 'Meadowbrook Recreation Center, 202 Meadow Ln';
  };
  
  const getVerificationTime = (date) => {
    // Generate a random time on the same day
    const baseDate = new Date(date);
    const hours = Math.floor(Math.random() * 10) + 8; // 8 AM to 6 PM
    const minutes = Math.floor(Math.random() * 60);
    
    return `${hours}:${minutes.toString().padStart(2, '0')} ${hours >= 12 ? 'PM' : 'AM'}`;
  };
  
  const generateBallotId = () => {
    // Generate a random ballot ID
    const chars = 'ABCDEFGHJKLMNPQRSTUVWXYZ23456789';
    let id = '';
    for (let i = 0; i < 10; i++) {
      id += chars.charAt(Math.floor(Math.random() * chars.length));
    }
    return `BLT-${id}`;
  };
  
  const generateVoterId = () => {
    // Generate a random voter ID
    const chars = '0123456789';
    let id = '';
    for (let i = 0; i < 8; i++) {
      id += chars.charAt(Math.floor(Math.random() * chars.length));
    }
    return `VTR-${id}`;
  };
  
  const generateVerificationHash = () => {
    // Generate a mock verification hash
    const chars = 'abcdef0123456789';
    let hash = '';
    for (let i = 0; i < 64; i++) {
      hash += chars.charAt(Math.floor(Math.random() * chars.length));
    }
    return hash;
  };
  
  const getCandidatePlatform = (candidate, party) => {
    // Return mock platform based on candidate and party
    if (party === 'Progressive Party') {
      return 'Focused on healthcare reform, climate action, and economic equality. Supports expanding social programs and increasing taxes on the wealthy.';
    } else if (party === 'Conservative Party') {
      return 'Advocated for fiscal responsibility, strong national defense, and traditional values. Supports tax cuts and reducing government regulations.';
    } else if (party === 'Liberty Party') {
      return 'Championed individual freedoms, limited government intervention, and free market solutions. Supports privacy rights and reducing bureaucracy.';
    } else if (party === 'Green Party') {
      return 'Prioritized environmental protection, sustainable development, and social justice. Supports renewable energy and organic farming initiatives.';
    } else if (party === 'N/A') {
      return 'This was a referendum or proposition vote rather than a candidate election.';
    }
    
    return 'Focused on community development, infrastructure improvements, and public services.';
  };
  
  const getKeyIssues = (candidate, party) => {
    // Return mock key issues based on candidate and party
    if (party === 'Progressive Party') {
      return ['Healthcare Reform', 'Climate Action', 'Economic Equality', 'Education Funding'];
    } else if (party === 'Conservative Party') {
      return ['Tax Reform', 'National Security', 'Job Creation', 'Government Spending'];
    } else if (party === 'Liberty Party') {
      return ['Individual Freedom', 'Digital Privacy', 'Free Markets', 'Government Reform'];
    } else if (party === 'Green Party') {
      return ['Environmental Protection', 'Renewable Energy', 'Sustainable Agriculture', 'Public Transportation'];
    } else if (party === 'N/A' && candidate.includes('Infrastructure')) {
      return ['Road Repairs', 'Bridge Safety', 'Public Transit', 'Rural Connectivity'];
    } else if (party === 'N/A' && candidate.includes('Education')) {
      return ['Teacher Pay', 'School Funding', 'Early Childhood', 'After-School Programs'];
    }
    
    return [];
  };
  
  // Methods
  const viewBallot = (history) => {
    selectedBallot.value = history;
    showBallotDialog.value = true;
  };
  </script>
  
  <style scoped>
  .v-timeline-item {
    margin-bottom: 8px;
  }
  
  .min-w-\[150px\] {
    min-width: 150px;
  }
  
  .font-mono {
    font-family: monospace;
  }
  </style>
  
  