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
  constructor() { }

  ngOnInit(): void {
  }

  registerToggle(){
    this.registerMode = !this.registerMode;
  }


  cancelRegisterMode(event: boolean) { // take in a boolean event value passed from child component (from html $event) (false)
    this.registerMode = event; 
  }
}
