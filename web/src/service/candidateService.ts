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

export default class Candidate {

    #api: apiService;
    constructor() {
        this.#api = new apiService();
    }

        async addCandidate(CandidateData: CandidateData) {
            const url = `api/Data/Centre/Add`;
            try {
                const response = await this.#api.post(url, CandidateData);
                return response;
            } catch (error) {
                console.error('Error adding centre:', error);
                throw error;
            }
        }
}
