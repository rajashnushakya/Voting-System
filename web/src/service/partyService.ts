import apiService from './apiService';

export default class PartyService {    
    #api: apiService
    constructor() {
        this.#api = new apiService();
    }                                    
    async getAllParties() {        
        const url = `api/Party`;
        try {
            const response = await this.#api.get(url);
            return response.data;
        } catch (error) {
            console.error('Error fetching parties:', error);
            throw error;
        }
    }    
    }