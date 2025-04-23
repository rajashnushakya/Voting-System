import apiService from "./apiService";

export interface CandidateData {
    id: number;
    fullname: string;
    fatherName: string;
    motherName: string;
    grandFatherName: string;
    grandMotherName: string;
    dateOfBirth: Date;
    gender: string;
    partyId: string | null;
    districtId: string | null;
    municipalityId: string | null;
    wardId: string | null;
    electionId: string | null;
}

export default class candidateService {

    #api: apiService;

    constructor() {
        this.#api = new apiService();
    }

    async addCandidate(candidateData: CandidateData) {
        const url = `api/Candidate/RegisterCandidate`;
        try {
            const response = await this.#api.post(url, candidateData);
            return response;
        } catch (error) {
            console.error('Error adding candidate:', error);
            throw error;
        }
    }

    async getCandidatesByCentreId(voterId: string) {
        const url = `/api/Candidate/Candidates?voterId=${voterId}`;
        try {
            const response = await this.#api.get(url);
            return response;
        } catch (error) {
            console.error("Error fetching candidates:", error);
            throw error;
        }
    }
    async getCandidatesByElectionId(electionId: string) {
        const url = `/api/Candidate/GetCandidateByElection?electionId=${electionId}`;
        try {
            const response = await this.#api.get(url);
            return response;
        } catch (error) {
            console.error("Error fetching candidates by election ID:", error);
            throw error;
        }
    }
    async enrollCandidateinElectionCentre (centreId: string, candidateId: string) {
        const url = '/api/Candidate/CandidateCentre';
        try {
            const response = await this.#api.post(url, [{
                candidateId,
                centreId  
            }]);
            return response;
        } catch (error) {
            console.error("Error enrolling candidate in election centre:", error);
            throw error;
        }
    }

    async getCandidateByCentreId (centreId: string,) {
        const url = `/api/Candidate/GetCandidateByElectionCentre?centreId=${centreId}`;
        try {
            const response = await this.#api.get(url, [{
                centreId  
            }]);
            return response;
        } catch (error) {
            console.error("Error enrolling candidate in election centre:", error);
            throw error;
        }
    }

    async getCandidatePosition() {
        const url = `api/Data/Candidate/Position`;
        try {
            const response = await this.#api.get(url);
            return response.data;
        } catch (error) {
            console.error('Error fetching election type:', error);
            throw error;
        }
    }

    
}
