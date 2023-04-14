import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { getPaginatedResult, getPaginationHeaders } from './paginationHelper';
import { Message } from '../_models/message';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { User } from '../_models/user';
import { BehaviorSubject, take } from 'rxjs';
import { Group } from '../modals/group';

// Connector to database. (code from library)
@Injectable({
  providedIn: 'root'
})
export class MessageService {
  baseUrl = environment.apiUrl;
  hubUrl = environment.hubUrl;
  private hubConnection?: HubConnection;
  private messageThreadSource = new BehaviorSubject<Message[]>([]); // initial value of empty array
  messageThread$ = this.messageThreadSource.asObservable(); //observable

  //Only connects when inside messages tab of the member-detail component,
  //So that users only get private messaging between two users and not application wide.
  constructor(private http: HttpClient) { }

  
  createHubConnection(user: User, otherUsername: string) {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(this.hubUrl + 'message?user=' + otherUsername, {
        accessTokenFactory: () => user.token // authenticate this particular messagehub as well from users, require to add authorize in messagehub in order to allow authentication.
      })
      .withAutomaticReconnect()
      .build();

      this.hubConnection.start().catch(error => console.log(error)); 
  
      //create observable that's going to store these messages so we can subscribe to them inside our component.  
      this.hubConnection.on('ReceiveMessageThread', messages => {
        this.messageThreadSource.next(messages);
      })

      //If users joins the group, it will not receive message thread
      //not going to know or our message thread is not going to be updated with the fact that they've read the message
      //check inside this messagethread to see if there's any messages from user that joins the group that are currently unread.
      //if they are, going to mark them as read on client to match how they're going to be in the server.
      this.hubConnection.on('UpdatedGroup',(group: Group) => {
        // check if username is the person that's joining the group
        //check to seeif there's any unread messages as we'll take the opportuntiy to mark it as read.
        if (group.connections.some(x => x.username === otherUsername)) {
          this.messageThread$.pipe(take(1)).subscribe({
            next: messages => {
              messages.forEach(message => {
                if (!message.dateRead) {
                  message.dateRead = new Date(Date.now())
                }
              })
              //replace array with updated version of this array and passing the messages
              //ensure that the the read is updated on sender's message thread when recipient connected to the message hub and marked as read.
              this.messageThreadSource.next([...messages]);
            }
          })
        }
      })

      
      //get message from signalR hub
      this.hubConnection.on('NewMessage', message => {
        this.messageThread$.pipe(take(1)).subscribe({
          next: messages => {
            //spread oeprator to create new array and replace existing array
            this.messageThreadSource.next([...messages, message])
          }
        })
      })
    }

    stopHubConnection() {
      //defensive programming: stop connection when hubconnection exists/connected.
      if (this.hubConnection) {
        this.hubConnection?.stop();
      }

    }


  // Get messages will be paginated (filtered according to desired pagesize, page number and container (inbox/outbox))
  getMessages(pageNumber: number, pageSize: number, container: string) {
     let params = getPaginationHeaders(pageNumber, pageSize); 
     params = params.append('Container', container);
     return getPaginatedResult<Message[]>(this.baseUrl + 'messages', params, this.http);
  }

  getMessageThread(username: string) { 
    return this.http.get<Message[]>(this.baseUrl + 'messages/thread/' + username);
  }

  //async effe ctively guarantees we are going to return a promise from invoke method.
  async sendMessage(username: string, content: string) {
    //invoke message on the server, on the API hub
    return this.hubConnection?.invoke('SendMessage', {recipientUsername: username, content})
      .catch(error => console.log(error.stack)); 
  
  }
  
  // delete message based on id from messages datase.
  deleteMessage(id: number) {
    return this.http.delete(this.baseUrl + 'messages/' + id)
  }
  
}




