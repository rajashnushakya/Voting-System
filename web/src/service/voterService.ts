import apiService from './apiService';

interface FormData {
    id: number;
    name: string;
    fatherName: string;
    motherName: string;
    grandFatherName: string;
    grandMotherName: string;
    phno: number;
    email: string;
    DateOfBirth: Date;
    gender: string;
    user: {
        roleId: number;
        userName: string;
        password: string;
    };
    address: {
        houseNumber: string;
        streetName: string;
        wardNumber: string;
        municipality: string;
        district: string;
        province: string;
    };
}

export default class voterService {
    #api: apiService;

    constructor() {
        this.#api = new apiService();  
    }

    // Add Voter
    async addVoter(formData: FormData) {
        const url = `/Voter/Register`;  
        try {
            const response = await this.#api.post(url, formData);
            return response;
        } catch (error) {
            console.error('Error adding voter:', error);
            throw error;
        }
    }

    // Get all Voters
    async getVoter() {
        const url = `/Voter/Register`;  
        try {
            const response = await this.#api.get(url);
            return response;
        } catch (error) {
            console.error('Error fetching voters:', error);
            throw error;
        }
    }

    // Edit Voter
    async editVoter(formData: FormData) {
        const url = `/Voter/Register/${formData.id}`;  
        try {
            const response = await this.#api.put(url, formData);
            return response;
        } catch (error) {
            console.error('Error editing voter:', error);
            throw error;
        }
    }

    // Delete Voter
    async deleteVoter(id: number) {
        const url = `/Voter/Register/${id}`;  
        try {
            const response = await this.#api.delete(url);
            return response;
        } catch (error) {
            console.error('Error deleting voter:', error);
            throw error;
        }
    }

    async getVoterCount(){
        const url = `api/Voter/Count`;
        try {
            const response = await this.#api.get(url);
            return response.data;
        } catch (error) {
            console.error('Error fetching voter count:', error);
            throw error;
        }
    }
}
