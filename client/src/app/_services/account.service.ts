import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, map } from 'rxjs';
import { environment } from 'src/environments/environment';
import { User } from '../_models/user';
import { PresenceService } from './presence.service';

//API: 
//Injectable providers can be used to provide services, repositories, and other objects that can be shared across different components and modules.
//t can be injected into other classes by adding it to the constructor parameter list using TypeScript's constructor injection syntax.
@Injectable({
  providedIn: 'root' // automatic add newly created services into provided array in the root moduke: app.module.ts
})

// responsbile for making HTTP requests from client to our server. (able to centralize http request)
// stores state request where app will remember no matter the user is in the application.
// singletons: instatiated when application starts and destroyed and app shuts down
export class AccountService {
  baseURL = environment.apiUrl;
  // allows you to store a current value and emit it to subscribers whenever they subscribe to the observable.
  // i.e. User's subscribe to newsletter (observable) and notification will be automatically send to user endpoint
  //similar to the Subject type, but it also has a current value that can be accessed even if there are no subscribers.
  private currentUserSource = new BehaviorSubject<User | null>(null); 

  // .asObservable is a method provided by BehaviorSubject and other types of observables in RxJS
  // allows you to expose the observable as a read-only version of itself, preventing external code from calling its next() method and modifying its internal state.
  // $ indicates property as an observable.
  //By exposing currentUsers$ as a public property, other parts of the application can subscribe to it to receive updates whenever the current user object changes. 
  //For example, a component could subscribe to currentUsers$ to update its display whenever the current user object changes.
  currentUser$ = this.currentUserSource.asObservable(); // signify that it is an observable
  constructor(private http: HttpClient, private presenceService: PresenceService) { }

  // pipe to transform data when get back request to the api server .
  login(model: any) {
    //return 'User' type of response
    return this.http.post<User>(this.baseURL + 'account/login', model).pipe(
      //transform response data from http post request to specific endpoint on the server by extracting response data and save in local variable 'user'.
      map((response: User) => { // response will return as type of 'User' interface.
        const user =  response;
        if (user) { // if user exist, store 'user' object in brower's localstorage as a JSON string from JavaScript.(persist the user's login state between page refreshes)
                localStorage.setItem('user', JSON.stringify(user)); 
                // .next() allows you to emit new values to all subscribers of the subject.
                //This means that the user object will be emitted to all subscribers of the currentUserSource subject, allowing them to access and work with the updated user object.
                //can be useful for keeping different parts of an Angular application in sync with each other and ensuring that they have access to the latest data.
                this.currentUserSource.next(user);
              }
            })
          )
        }

        register(model: any) {
          //Use http client to make the call to API service and return User's DTO back from API server.
          return this.http.post<User>(this.baseURL + 'account/register', model).pipe(
            map(user => {
              if (user) { 
                this.setCurrentUser(user);
              }
              // return user;
            })
          ) // do something to the observable before subscribing to it  
        }

        //create hub connection in this method as this method will always be called whenever login or register or refresh browser and get token from local storage
        setCurrentUser(user: User) {
          user.roles = [];
          const roles = this.getDecodedToken(user.token).role;
          // check if roles is an array of roles, if it is, set user roles to it, else push the roles into array of user's roles property
          Array.isArray(roles) ? user.roles = roles : user.roles.push(roles);
          localStorage.setItem('user', JSON.stringify(user));
          this.currentUserSource.next(user);
          this.presenceService.createHubConnection(user);
        } 

      logout() {
        localStorage.removeItem('user');
        this.currentUserSource.next(null);
        this.presenceService.stopHubConnection();
    }

    //get information of the json web token than its alogirthm and signature.
    getDecodedToken(token: string) {
      return JSON.parse(atob(token.split('.')[1]))
    }


  }

//Overall, this code is a simplified example of an Angular authentication service that sends login credentials to a server,
//retrieves a user object, and stores it in the browser's localStorage.

