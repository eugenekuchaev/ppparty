import { User } from "./user";

export class EventParams {
    eventName: string;
    fromDate: string;
    toDate: string;
    country: string;
    region: string;
    city: string;
    eventTag: string;
    pageNumber = 1;
    pageSize = 10;

    constructor(user: User) {}
}