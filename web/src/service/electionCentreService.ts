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

export interface ElectionCentreDetails {
  TotalCandidates: number;
  TotalVoters: number;
  TotalVotes: number;
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

  async addElectionCentre(centreData: CentreData): Promise<any> {
    const url = `api/Election/Centre`;
    try {
      const response = await this.#api.post(url, centreData);
      
      // Return the response data
      return response.data; // Assuming the message is inside response.data
    } catch (error) {
      console.error('Error adding election centre:', error);
      throw error;  // Throw the error to be caught in the caller
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

  async getElectionCentreDetails(electionCentreId: string): Promise<ElectionCentreDetails[]> {
    const url = `api/Data/Election/Centre/Detail?electionCentreId=${electionCentreId}`;
    try {
      const response = await this.#api.get(url);
      return response.data;
    } catch (error) {
      console.error('Error fetching election centers:', error);
      throw error;
    }
  }

  async getCandidateVotes(centreId: string): Promise<ElectionCentreDetails[]> {
    const url = `api/Data/Candidate/Votes?centreId=${centreId}`;
    try {
      const response = await this.#api.get(url);
      return response.data;
    } catch (error) {
      console.error('Error fetching election centers:', error);
      throw error;
    }
  }
}
