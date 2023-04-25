import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { User } from '../_models/user';
import { Photo } from '../_models/photo';

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
    //create single query parameter (send data to a server to be processed or stored.)
    return this.http.post<string[]>(this.baseUrl + 'admin/edit-roles/' 
    + username + '?roles=' + roles, {});

  }

  getPhotosForApproval()
  {//get array of photos for approval.(retrieve data from a server)
    return this.http.get<Photo[]>(this.baseUrl + 'admin/photos-to-moderate');
  }

  approvePhoto(photoid: number) 
  {//(send data to a server to be processed or stored.)
    return this.http.post<Photo[]>(this.baseUrl + 'admin/approve-photo/' + photoid, {});
  }
  
  rejectPhoto(photoid: number) 
  {
  //(send data to a server to be processed or stored.)
    return this.http.post<Photo[]>(this.baseUrl + 'admin/reject-photo/' + photoid, {});
  }
}
