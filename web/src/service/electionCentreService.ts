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
  electionId: number | null;
  electionCentre: string;
}

export interface ElectionCenter {
  id: number;
  name: string;
  address: string;
  city: string;
  currentEnrollment: number;
  capacity: number;
  distance: string;
}

export default class ElectionCentreService {
  #api: apiService;

  constructor() {
    this.#api = new apiService();
  }

  async getMunicipalities(districtId: number): Promise<Municipality[]> {
    const url = `api/Data/Election/Municipality?districtId=${districtId}`;
    try {
      const response = await this.#api.get(url);
      return response.data;
    } catch (error) {
      console.error('Error fetching municipalities:', error);
      throw error;
    }
  }

  async getWards(municipalityId: number): Promise<Ward[]> {
    const url = `api/Data/Election/Ward?municipalityId=${municipalityId}`;
    try {
      const response = await this.#api.get(url);
      return response.data;
    } catch (error) {
      console.error('Error fetching wards:', error);
      throw error;
    }
  }

  async getCentreName(districtId: number, municipalityId: number, wardId: number): Promise<string> {
    const url = `api/Data/Centre/Name?districtId=${districtId}&municipalityId=${municipalityId}&wardId=${wardId}`;
    try {
      const response = await this.#api.get(url);
      return response.data;
    } catch (error) {
      console.error('Error fetching centre name:', error);
      throw error;
    }
  }

  async addElectionCentre(centreData: CentreData): Promise<void> {
    const url = `api/Election/Centre`;
    try {
      await this.#api.post(url, centreData);
    } catch (error) {
      console.error('Error adding election centre:', error);
      throw error;
    }
  }

  async enrollVoter(electionId: number, centerId: number): Promise<void> {
    const url = `/Election/Enroll`;
    try {
      await this.#api.post(url, { electionId, centerId });
    } catch (error) {
      console.error('Error enrolling voter:', error);
      throw error;
    }
  }

  async getCentersByElection(electionId: string): Promise<ElectionCenter[]> {
    const url = `api/Election/ElectionCentre?electionId=${electionId}`;
    try {
      const response = await this.#api.get(url);
      return response.data;
    } catch (error) {
      console.error('Error fetching election centers:', error);
      throw error;
    }
  }
}
