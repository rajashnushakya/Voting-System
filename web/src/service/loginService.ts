import apiService from './apiService';
interface loginFormData {
  email: string;
  password: string; 
}

export default class loginService {
    #api: apiService;

    constructor() {
        this.#api = new apiService();  
    }


    // Add Voter
    async login(loginFormData: loginFormData) {
        const url = `api/Voter/Login`;
        try {
          const response = await this.#api.post(url, loginFormData); 
          return response;
        } catch (error) {
          console.error("Error logging in", error);
          throw error;
        }
      }

}