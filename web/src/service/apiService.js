import axios from 'axios'

const apiClient = axios.create({
    baseURL: '/api',
    headers: {
        "Content-Type": 'application/json',

    }
});

export default{
    getData(endpoint){
        return apiClient.get('/'+endpoint);
    },
    postData(endpoint, data){
        return apiClient.post('/'+endpoint, data);
    },
    putData(endpoint, data){
        return apiClient.put('/'+endpoint, data);
    },
    deleteData(endpoint, data){
        return apiClient.delete('/'+endpoint, data);
    }
}

apiClient.interceptors.response.use(
    response => response,
    error=>{
        console.error("API error: ", error);
        return Promise.reject(error);
    }
)