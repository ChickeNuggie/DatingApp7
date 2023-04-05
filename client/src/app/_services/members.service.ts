import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map, of, take } from 'rxjs';
import { environment } from 'src/environments/environment';
import { Member } from '../_models/member';
import { PaginatedResult } from '../_models/pagination';
import { User } from '../_models/user';
import { UserParams } from '../_models/userParams';
import { AccountService } from './account.service';

// Services are Connector to HttpClient
@Injectable({
  providedIn: 'root'
})

// Good place to store users or other information after receive from API as services last a lifetime (singleton)
// But API component gets destroyed after loading methods, members will also get destroyed and have to reload members all over again.
export class MembersService {
  baseUrl = environment.apiUrl;
  members: Member[] = [];
  memberCache = new Map(); // mapping allows access to get and set property (set result and keys in member cache when get back request from API)
// Note: the value of map is the paginatedresult

  user: User | undefined;
  userParams: UserParams | undefined;

  //inject http client
  constructor(private http: HttpClient, private accountService: AccountService) {
    // take first or default current user observable and subscribe
    // Allow to remember query when task is called in component and destroyed
    this.accountService.currentUser$.pipe(take(1)).subscribe({
      next: user => {
        if (user) {
          this.userParams = new UserParams(user);
          this.user = user;
        }
      }
    })
   }

   // Allow to remember query when task is called in component and destroyed
   getUserParams() {
    return this.userParams;
   }

   setUserParams(params: UserParams) {
    this.userParams = params;
   }

   // Allow to remember query when task is called in component and destroyed
   resetUserParams() {
    if (this.user) {
      this.userParams = new UserParams(this.user);
      return this.userParams;
    }
    return;
   }

  getMembers(userParams: UserParams) {
    //store query object as a key and store results as a value.
    //For each repeated request with same parameters, obtain results from memory than from API.
    // each time get these result, it will check to see if query has been made befeore using that key
    const response = this.memberCache.get(Object.values(userParams).join('-'));

    if (response) return of(response);

    //set query string parameters along with http request.
    let params = this.getPaginationHeaders(userParams.pageNumber, userParams.pageSize);

    params = params.append('minAge', userParams.minAge);
    params = params.append('maxAge', userParams.maxAge);
    params = params.append('gender', userParams.gender);
    params = params.append('orderBy', userParams.orderBy);
    // Access and return the pagination header.
    // observe the body of the response and pass the parameters
    //map and set result to store inside membercache instead of using it from API for repeated request.
    return this.getPaginatedResult<Member[]>(this.baseUrl + 'users', params).pipe(
      map(response => {
        this.memberCache.set(Object.values(userParams).join('-'), response) // set response to this result
        return response;
      })
    )
     // return array of members from http client and store in member inside services
  }


  getMember(username: string) {
    const member = [...this.memberCache.values()]
    .reduce((arr, elem) => arr.concat(elem.result), []) // concat previous value with current array working inside the function into one array.
    .find((member: Member) => member.userName === username); //find member among repeated arrays which selects first element that matches the users' query and return the result.

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

  //add like method inside member card component from client-side.
  addLike(username: string) {
    return this.http.post(this.baseUrl + 'likes/' + username, {});
  }

  getLikes(predicate: string, pageNumber: number, pageSize: number)
  {
    let params = this.getPaginationHeaders(pageNumber, pageSize);

    params = params.append('predicate', predicate);

    return this.getPaginatedResult<Member[]>(this.baseUrl + 'likes', params);
  }


  // Make method generic using <T> to be reused on other class/objects
  private getPaginatedResult<T>(url: string, params: HttpParams) {
    const paginatedResult: PaginatedResult<T> = new PaginatedResult<T>;
    return this.http.get<T>(url, { observe: 'response', params }).pipe(
      map(response => {
        if (response.body) {
          paginatedResult.result = response.body;
        }
        const pagination = response.headers.get('Pagination');
        if (pagination) {
          paginatedResult.pagination = JSON.parse(pagination); // get the serialized JSON into an object.
        }
        console.log(paginatedResult);
        return paginatedResult;
      })
    );
  }


  private getPaginationHeaders(pageNumber: number, pageSize: number) {
    let params = new HttpParams();

    params = params.append('pageNumber', pageNumber);
    params = params.append('pageSize', pageSize);
    
    return params;
  }


}
