import { Component, OnInit } from '@angular/core';
import { User } from './_models/user';
import { AccountService } from './_services/account.service';

//API
// root component of an Angular application, and it typically contains logic related to routing, authentication, and other aspects of the application's behavior
// use whenever you need to define the behavior and structure of the root component of your Angular application.
@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit { // inject http client into appcomponent
  title = 'Dating app';
  constructor( private accountService: AccountService) {}

  ngOnInit(): void { // On inititialization , get request from users and return void
    this.setCurrentUser();
  } 

  setCurrentUser() {
    const userString = localStorage.getItem('user');
    if (!userString) return;
    const user: User = JSON.parse(userString);
    this.accountService.setCurrentUser(user);
  }
}
