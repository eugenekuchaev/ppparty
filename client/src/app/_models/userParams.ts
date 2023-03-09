import { User } from "./user";

export class UserParams {
    fullName: string;
    username: string;
    country: string;
    region: string;
    city: string;
    userInterest: string;
    pageNumber = 1;
    pageSize = 5;

    constructor(user: User) {}
}