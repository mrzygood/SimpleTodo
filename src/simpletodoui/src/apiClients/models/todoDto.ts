export class TodoDto {
    id: string;
    title: string;
    description: string;
    doneAt?: Date;
    createdAt: Date;

    constructor(
        id: string,
        title: string,
        description: string,
        createdAt: Date,
        doneAt?: Date
    ) {
        this.id = id;
        this.title = title;
        this.description = description;
        this.doneAt = doneAt;
        this.createdAt = createdAt;
    }
}
