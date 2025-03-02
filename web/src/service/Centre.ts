import apiService from './apiService';

export interface District {
    id: number;
    name: string;
}

export interface Municipality {
    id: number;
    name: string;
    districtId: number;
}

export interface Ward {
    id: number;
    name: string;
    municipalityId: number;
}

export default class Centre {
    #api: apiService;

    constructor() {
        this.#api = new apiService();
    }

    async getAllDistricts() {
        const url = `api/Data/districts`;
        try {
            const response = await this.#api.get(url);
            return response.data;
        } catch (error) {
            console.error('Error fetching districts:', error);
            throw error;
        }
    }

    async getMunicipalities(districtId?: number) {
        const url = `api/Data/municipalities`;
        try {
            const response = await this.#api.get(url, { params: { districtId } });
            return response.data;
        } catch (error) {
            console.error('Error fetching municipalities:', error);
            throw error;
        }
    }

    async getWards(municipalityId?: number) {
        const url = `api/Data/ward`;
        try {
            const response = await this.#api.get(url, { params: { municipalityId } });
            return response.data;
        } catch (error) {
            console.error('Error fetching wards:', error);
            throw error;
        }
    }
}