import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-server-error',
  templateUrl: './server-error.component.html',
  styleUrls: ['./server-error.component.css']
})
export class ServerErrorComponent implements OnInit {
  error: any;
  // inject router to get access to the router state into this component.
  // only able to access one time in the router and after Init, it will be gone and too late to notify user that something went wrong.
  constructor(private router: Router) {
    const navigation = this.router.getCurrentNavigation();
    this.error = navigation?.extras?.state?.['error']; //state errors may be undefined thus,. need optional '?' chaining.
  } 

  ngOnInit(): void {
  }

}
