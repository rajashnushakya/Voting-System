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
        const url = `api/Candidate`;
        try {
            const response = await this.#api.post(url, candidateData);
            return response;
        } catch (error) {
            console.error('Error adding candidate:', error);
            throw error;
        }
    }

    async getCandidatesByVoterId(voterId: string) {
        const url = `/api/Candidate/Candidates?voterId=${voterId}`;
        try {
            const response = await this.#api.get(url);
            return response;
        } catch (error) {
            console.error("Error fetching candidates:", error);
            throw error;
        }
    }
}
