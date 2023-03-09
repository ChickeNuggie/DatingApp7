import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
  registerMode = false;
  users: any;

  //effectively show list of content from container and hide aove content/remove from DOM.   
  constructor(private http: HttpClient) { }

  ngOnInit(): void {
    this.getUsers();
  }

  registerToggle(){
    this.registerMode = !this.registerMode;
  }

    
  getUsers() {
    this.http.get('https://localhost:5001/api/users').subscribe({ //.get return stream of observed data when get request. from .net API sever application
      next: response => this.users = response,
      error: error => console.log(error),
      complete: () => console.log('Request has completed') // next step after returning the data
    }) 
  } // subscribe to force our request to go get the observed data as observables are lazy by nature and they will not happen if its not observed.

  cancelRegisterMode(event: boolean) { // take in a boolean event value passed from child component (from html $event) (false)
    this.registerMode = event; 
  }
}
