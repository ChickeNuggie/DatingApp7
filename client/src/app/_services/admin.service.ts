import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { User } from '../_models/user';

//Connector to database 'html'
@Injectable({
  providedIn: 'root'
})
export class AdminService {
  baseUrl = environment.apiUrl;
  
  constructor(private http: HttpClient) { }


  //get array of users.
  //The routes parameter need to match those in admin controller.
  getUsersWithRoles() {
    return this.http.get<User[]>(this.baseUrl + 'admin/users-with-roles');
  }

  updateUserRoles(username: string, roles: string[]) {
    //create single query parameter
    return this.http.post<string[]>(this.baseUrl + 'admin/edit-roles/' 
    + username + '?roles=' + roles, {});

  }
  
}
