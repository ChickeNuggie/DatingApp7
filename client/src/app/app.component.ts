import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit { // inject http client into appcomponent
  title = 'Dating app';
  users: any;  // class property  of any data types
  constructor(private http: HttpClient) {}

  ngOnInit(): void { // On inititialization , get request from users and return void
    this.http.get('https://localhost:5001/api/users').subscribe({ //.get return stream of observed data when get request. from .net API sever application
      next: response => this.users = response,
      error: error => console.log(error),
      complete: () => console.log('Request has completed') // next step after returning the data
    }) 
  } // subscribe to force our request to go get the observed data as observables are lazy by nature and they will not happen if its not observed.
  
}
