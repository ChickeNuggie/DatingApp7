import { User } from "./user";

export class UserParams {
    gender: string;
    minAge = 18;
    maxAge = 99;
    pageNumber = 1;
    pageSize = 5;
    orderBy = 'lastActive';

    // Class allows us to create constructor to initialize values inside class upon using it.
    constructor(user: User) {
        this.gender = user.gender === 'female' ? 'male' : 'female'
    }
}