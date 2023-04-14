import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { ToastrService } from 'ngx-toastr';
import { environment } from 'src/environments/environment';
import { User } from '../_models/user';
import { BehaviorSubject, take } from 'rxjs';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class PresenceService {
  hubUrl= environment.hubUrl;
  private hubConnection?: HubConnection; // use optional chaining as not available/undefined when initial service is constructed.
  
  //allow components to subscribe to this information(event) so that they're notified if something has changed
  private onlineUsersSource = new BehaviorSubject<string[]>([]);// allows to give it an initial value
  onlineUsers$ = this.onlineUsersSource.asObservable();// gives us something to subscribe from components. (for updating )

  constructor(private toastr: ToastrService, private router: Router) { }


  createHubConnection(user: User) {
    this.hubConnection = new HubConnectionBuilder()
    .withUrl(this.hubUrl + 'presence', { // specify url to access database
      accessTokenFactory: () => user.token
    })
    //retry connection to API server if client is disconnected
    .withAutomaticReconnect()
    .build(); // build the hub connection.

    this.hubConnection.start().catch(error => console.log(error));

    //listen to specific method that sends information of the username online. (need to match name of the method on the server)
    this.hubConnection.on('UserIsOnline', username => {
      this.onlineUsers$.pipe(take(1)).subscribe({
        //update array from users observables by creating new version of this array and replace old array with new array
        next: usernames => this.onlineUsersSource.next([...usernames, username])
      })
    })

    this.hubConnection.on('UserIsOffline', username => {
      this.onlineUsers$.pipe(take(1)).subscribe({
        //update array from users observables by creating new array and replace existing array with newly returned array excluding the username.
        next: usernames => this.onlineUsersSource.next(usernames.filter(x => x !== username))
      })
    })

    this.hubConnection.on('GetOnlineUsers', usernames => {
      this.onlineUsersSource.next(usernames); // update that particular list
    })

    this.hubConnection.on('NewMessageReceived', ({username, knownAs}) => {
      this.toastr.info(knownAs + ' has sent you a new message! Click me to see it.')
      //navigate users to message tab
        .onTap
        .pipe(take(1))
        .subscribe({
          // query string to take the user directly to the meaage tab
          next: () => this.router.navigateByUrl('/members/' + username + '?tab=Messages')
        })
    })

  }

  stopHubConnection() {
    this.hubConnection?.stop().catch(error => error.console.log(error));
  }

}
