import apiService from './apiService';

export interface ElectionData {
    id: number;
    name: string;
    start_date: string | null; // Accepting string to ensure correct conversion
    end_date: string | null;
    formatted_start_date?: string;
    formatted_end_date?: string;
}

export default class ElectionService {
    #api: apiService;

    constructor() {
        this.#api = new apiService();
    }

    async addElection(electionData: ElectionData) {
        const url = `api/Election/Add`;

        if (electionData.start_date) {
            const startDate = new Date(electionData.start_date);
            if (!isNaN(startDate.getTime())) {
                electionData.formatted_start_date = this.formatDateTime(startDate);
            } else {
                console.error("Invalid start_date format:", electionData.start_date);
            }
        }

        if (electionData.end_date) {
            const endDate = new Date(electionData.end_date);
            if (!isNaN(endDate.getTime())) {
                electionData.formatted_end_date = this.formatDateTime(endDate);
            } else {
                console.error("Invalid end_date format:", electionData.end_date);
            }
        }

        try {
            const response = await this.#api.post(url, electionData);
            return response;
        } catch (error) {
            console.error('Error adding election:', error);
            throw error;
        }
    }

    private formatDateTime(date: Date): string {
        if (!(date instanceof Date) || isNaN(date.getTime())) {
            console.error("Invalid date passed to formatDateTime:", date);
            return '';
        }

        const year = date.getFullYear();
        const month = String(date.getMonth() + 1).padStart(2, '0'); 
        const day = String(date.getDate()).padStart(2, '0');
        const hours = String(date.getHours()).padStart(2, '0');
        const minutes = String(date.getMinutes()).padStart(2, '0');
        const seconds = String(date.getSeconds()).padStart(2, '0');

        return `${year}-${month}-${day} ${hours}:${minutes}:${seconds}`;
    }

    async getActiveElection() {
        const url = `api/Election/Get`;
        try {
            const response = await this.#api.get(url);
            return response.data;
        } catch (error) {
            console.error('Error fetching active election:', error);
            throw error;
        }
    }

    async getElectionCount() {
        const url = `api/Election/Count`;
        try {
            const response = await this.#api.get(url);
            return response.data;
        } catch (error) {
            console.error('Error fetching election count:', error);
            throw error;
        }
    }

    async endElection(id: number) {
        const url = `api/Election/EndElection?electionId=${id}`;
        try {
            const response = await this.#api.post(url, {
                electionId: id 
            });
            return response.data;
        } catch (error) {
            console.error('Error ending election:', error);
            throw new Error('Failed to end the election');
        }
    }

    async startElection(id: number) {
        const url = `api/Election/StartElection?electionId=${id}`;
        try {
            const response = await this.#api.post(url, { electionId: id });
            return response.data;
        } catch (error) {
            console.error("Error starting election:", error);
            throw new Error("Failed to start the election");
        }
    }
    
      async enrollVoter(voterId: string, electionId: number): Promise<void> {
        const url = `api/Voter/VoterEnrollment?voterId=${voterId}&electionId=${electionId}`;
      
        try {
          await this.#api.post(url);
        } catch (error) {
          console.error('Error enrolling voter:', error);
          throw error;
        }
      }
    
}
