// src/types/apiService.d.ts
declare module 'apiService' {
    interface ApiService {
      getData(endpoint: string): Promise<any>;
      postData(endpoint: string, data: any): Promise<any>;
      putData(endpoint: string, data: any): Promise<any>;
      deleteData(endpoint: string, data: any): Promise<any>;
    }
  
    const apiService: ApiService;
    export default apiService;
  }
  