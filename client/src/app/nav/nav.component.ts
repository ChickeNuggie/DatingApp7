import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { AccountService } from '../_services/account.service';

// Used to write code for client-side and server-side applications in the .NET framework.
// It adds static typing, classes, interfaces, and other features to JavaScript, which can help developers catch errors and write more maintainable code
//API: component is a building block of an application's user interface that encapsulates its own logic, data, and presentation (must have a template associated with it.)
//component that encapsulates its own logic, data, and presentation using a template
//used in web applications to define the navigation menu or toolbar, which provides users with a way to navigate between different parts of the application.
//often used in conjunction with routing to create a seamless navigation experience
//need to implement a navigation menu or toolbar in your web application
@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {

  //store users' information when they complete that form and put out in console.
  model: any = {}; 

  //inject accountservice component into accountservices class template, private router to route to different html links
  // inject toastr service for text box
  constructor(public accountService: AccountService, private router: Router,
    private toastr: ToastrService) { }

  //Typically used to perform initialization tasks that are related to the component's state, such as retrieving data from a service, initializing properties, or setting up event listeners.
  //Retrieve the list of items from a service and initialize its internal state.
  //Method is called only once, after the component is created and before it is rendered for the first time.
  //If the component's inputs change later on, Angular will call other lifecycle hook methods such as ngOnChanges() and ngDoCheck() to reflect these changes.
  ngOnInit(): void {
  //use AccountServices template directly to access currentUser$ observable instead of initializing it.
  }

  login () {
    //extract credientials from form and returns observable from service.
    //// use async pipe to subscribe and unsubscribe to data to prevent memory leak.
    this.accountService.login(this.model).subscribe({ 
      next: _ =>  {
      this.router.navigateByUrl('/members'); // direct to member list.
      this.model = {}; //reset template form to initially created empty object.
      //Interceptor handles error exception thus, need not specify or show error in console log.
      }
    })
  }

  logout() {
    this.accountService.logout(); // remove item from local storage.
    this.router.navigateByUrl('/'); // back to home page upon logged out

  }
}
