import axios from 'axios';


const apiClient = axios.create({
    baseURL: import.meta.env.VITE_APP_URL || '/api', 
    headers: {
        "Content-Type": 'application/json',
        Accept: 'application/json',
    }
});

apiClient.interceptors.response.use(
    response => response, 
    error => {
        console.error("API error: ", error);
        return Promise.reject(error);
    }
);

export default class apiService {
    // GET request
    get(endpoint:string, params = {}) {
        return apiClient.get(endpoint, { params });
    }

    // POST request
    post(endpoint:string, data = {}) {
        return apiClient.post(endpoint, data);
    }

    // PUT request
    put(endpoint:string, data = {}) {
        return apiClient.put(endpoint, data);
    }

    // DELETE request
    delete(endpoint:string) {
        return apiClient.delete(endpoint);
    }
};
