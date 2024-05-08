import axios, { AxiosInstance } from "axios";
import KeyCloakService from "@/auth/KeycloakService";

const apiClient: AxiosInstance = axios.create({
    baseURL: "http://localhost:5238",
    headers: {
        "Content-type": "application/json",
    },
});

apiClient.interceptors.request.use(
    async config => {
        const token = await KeyCloakService.GetBearerToken();
        if (token) {
            config.headers.Authorization = "Bearer " + token;
        }
        return config;
    },
    error => {
        return Promise.reject(error);
    }
);

export default apiClient;
