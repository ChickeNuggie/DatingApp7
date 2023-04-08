import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { getPaginatedResult, getPaginationHeaders } from './paginationHelper';
import { Message } from '../_models/message';

// Connector to database. (code from library)
@Injectable({
  providedIn: 'root'
})
export class MessageService {
  baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }

  // Get messages will be paginated (filtered according to desired pagesize, page number and container (inbox/outbox))
  getMessages(pageNumber: number, pageSize: number, container: string) {
     let params = getPaginationHeaders(pageNumber, pageSize); 
     params = params.append('Container', container);
     return getPaginatedResult<Message[]>(this.baseUrl + 'messages', params, this.http);
  }

  getMessageThread(username: string) { 
    return this.http.get<Message[]>(this.baseUrl + 'messages/thread/' + username);
  }

  sendMessage(username: string, content: string) {
    //ensure it matches and bind the API that is expecting to receive in create member dto.
    return this.http.post<Message>(this.baseUrl + 'messages',
     {recipientUsername: username, content});
  }
  
  // delete message based on id from messages datase.
  deleteMessage(id: number) {
    return this.http.delete(this.baseUrl + 'messages/' + id)
  }
  
}




