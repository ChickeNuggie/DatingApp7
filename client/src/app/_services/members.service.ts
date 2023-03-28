import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map, of } from 'rxjs';
import { environment } from 'src/environments/environment';
import { Member } from '../_models/member';

// Services are Connector to HttpClient
@Injectable({
  providedIn: 'root'
})

// Good place to store users or other information after receive from API as services last a lifetime (singleton)
// But API component gets destroyed after loading methods, members will also get destroyed and have to reload members all over again.
export class MembersService {
  baseUrl = environment.apiUrl;
  members: Member[] = [];
  //inject http client
  constructor(private http: HttpClient) { }

  getMembers() {
    if (this.members.length > 0) return of(this.members); // return an observable of members
    return this.http.get<Member[]>(this.baseUrl + 'users').pipe(
      map(members => {
        this.members = members;
        return members;
      })
    ) // return array of members from http client and store in member inside services
  }

  getMember(username: string) {
    const member = this.members.find(x => x.userName === username);
    if (member) return of(member);
    return this.http.get<Member>(this.baseUrl + 'users/' + username);
  }

  // Send Http request to the client-side(html) API to update user in the client(html template) API
  updateMember (member: Member) {
    return this.http.put(this.baseUrl + 'users', member).pipe(
      // Shows the Updated member array.
      map(() => {
        const index = this.members.indexOf(member); // shows index of the element of this updated member array
        //Updated member with updated information inside the array of member service.
        this.members[index] = {...this.members[index], ...member}  // update info on the properties inside member  
        // '...' spread oeprator takers all elements of the member at this location in this.member array and spreads them (i.e Username, ID, etc.)
      })
    ); // pass member which is going to be sent to our API
  }

  //sends an HTTP PUT request to the server (HttpClient service) to set a user's main photo.
  //The method returns an Observable of the HTTP response, which the caller can subscribe to in order to receive the response from the server
  setMainPhoto(photoId: number) {
    return this.http.put(this.baseUrl + 'users/set-main-photo/' + photoId, {});
  }

  //delete photo from client-side (html) 
  deletePhoto(photoId: number) {
    return this.http.delete(this.baseUrl + 'users/delete-photo/' + photoId);
  }
}
