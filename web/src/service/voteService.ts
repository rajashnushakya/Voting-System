import apiService from './apiService';

export interface VotePayload {
    candidateId: number;
    voterId: number;
    electionId: number;
    electionCentreId: number;
  }
  

export default class voteService {
  #api: apiService;

  constructor() {
    this.#api = new apiService();
  }

  async castVote(payload: VotePayload): Promise<any> {
    const url = `api/CandidateVote/Vote`;
    try {
      const response = await this.#api.post(url, payload);
      return response.data;
    } catch (error) {
      console.error('Error casting vote:', error);
      throw error;
    }
  }
  async getVotesByVoter(voterId: string) {
    const url = `/api/CandidateVote/GetVotesByVoter/${voterId}`;
    try {
      const response = await this.#api.get(url);  
      return response.data;
    } catch (error) {
      console.error("Error fetching votes:", error);
      throw error;
    }
  }
  
}
