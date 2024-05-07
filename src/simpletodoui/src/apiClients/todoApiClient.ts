import http from "@/apiClients/http-common";
import { CreateTodoRequest } from "./models/createTodoRequest"
import { UpdateTodoRequest } from "./models/updateTodoRequest"
import { TodoDto } from "./models/todoDto"

class TodoApiClient {
    getAll(): Promise<any> {
        return http.get("/todo?limit=20");
    }

    get(id: string): Promise<any> {
        return http.get(`/todo/${id}`);
    }

    create(data: CreateTodoRequest): Promise<any> {
        return http.post("/todo", data);
    }

    update(data: UpdateTodoRequest): Promise<any> {
        return http.put(`/todo`, data);
    }

    delete(id: string): Promise<any> {
        return http.delete(`/todo/${id}`);
    }

    finish(id: string): Promise<any> {
        return http.post(`/todo/${id}/finish`);
    }
}

export default new TodoApiClient();
