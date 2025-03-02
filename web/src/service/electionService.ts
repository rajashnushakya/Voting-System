import apiService from './apiService';

export interface ElectionData {
    id: number;
    name: string;
    start_date: string | null;
    end_date: string | null;
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
            electionData.start_date = startDate.toISOString().split('T')[0];
        }
        if (electionData.end_date) {
            const endDate = new Date(electionData.end_date);
            electionData.end_date = endDate.toISOString().split('T')[0];
        }

        try {
            const response = await this.#api.post(url, electionData);
            return response;
        } catch (error) {
            console.error('Error adding election:', error);
            throw error;
        }
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
}
