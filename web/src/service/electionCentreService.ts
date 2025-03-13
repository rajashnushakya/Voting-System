import apiService from './apiService';

export interface District {
    id: number;
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

export interface CentreData {
    id: number;
    name: string;
    districtId: number | null;
    municipalityId: number | null;
    wardId: number | null;
}

export default class ElectionCentreService {
    #api: apiService;

    constructor() {
        this.#api = new apiService();
    }


    async getMunicipalities(districtId: number): Promise<Municipality[]> {
        const url = `api/Data/Election/Municipality?${districtId}`;
        try {
            const response = await this.#api.get(url);
            return response.data;
        } catch (error) {
            console.error('Error fetching municipalities:', error);
            throw error;
        }
    }

    async getWards(municipalityId: number): Promise<Ward[]> {
        const url = `api/Data/Election/Ward?${municipalityId}`;
        try {
            const response = await this.#api.get(url);
            return response.data;
        } catch (error) {
            console.error('Error fetching wards:', error);
            throw error;
        }
    }

}
