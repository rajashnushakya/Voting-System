// src/types/apiService.d.ts
declare module 'apiService' {
    interface ApiService {
      get(endpoint: string): Promise<any>;
      post(endpoint: string, data: any): Promise<any>;
      put(endpoint: string, data: any): Promise<any>;
      delete(endpoint: string, data: any): Promise<any>;
    }
  
    const apiService: ApiService;
    export default apiService;
  }
  