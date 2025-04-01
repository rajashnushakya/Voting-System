import apiService from './apiService';


export default class loginService {
    #api: apiService;

    constructor() {
        this.#api = new apiService();  
    }

    // Add Voter
    async login(email: string, password: string) {
        const url = `api/Voter/Login?email=${encodeURIComponent(email)}&password=${encodeURIComponent(password)}`;
        try {
          const response = await this.#api.post(url); 
          return response;
        } catch (error) {
          console.error("Error logging in", error);
          throw error;
        }
      }

}