import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';

// To test different area of error responses
@Component({
  selector: 'app-test-error',
  templateUrl: './test-error.component.html',
  styleUrls: ['./test-error.component.css']
})
export class TestErrorComponent implements OnInit {
  baseUrl = 'https://localhost:5001/api/';
  validationErrors: string[] = [];

  constructor(private http: HttpClient) { } //inject Http client inside this component

  ngOnInit(): void {
  }

  get404Error() {
    this.http.get(this.baseUrl + 'buggy/not-found').subscribe({ //name of endpoint that we are trying to access that returns an observable and requires to subscribe.
      next: response => console.log(response),
      error: error => console.log(error)
    })
  }  

  get400Error() {
    this.http.get(this.baseUrl + 'buggy/bad-request').subscribe({ //name of endpoint that we are trying to access that returns an observable and requires to subscribe.
      next: response => console.log(response),
      error: error => console.log(error)
    })
  } 

  get500Error() {
    this.http.get(this.baseUrl + 'buggy/server-error').subscribe({ //name of endpoint that we are trying to access that returns an observable and requires to subscribe.
      next: response => console.log(response),
      error: error => console.log(error)
    })
  }  

  get401Error() {
    this.http.get(this.baseUrl + 'buggy/auth').subscribe({ //name of endpoint that we are trying to access that returns an observable and requires to subscribe.
      next: response => console.log(response),
      error: error => console.log(error)
    })
  }  

  get400ValidationError() {
    //name of endpoint that we are trying to access that returns an observable and requires to subscribe.
    //send up empty object just to test the validation errors there.
    this.http.post(this.baseUrl + 'account/register', {}).subscribe({ 
      next: response => console.log(response),
      error: error => {
        console.log(error);
        this.validationErrors = error; // set validation errors property to that array of validation errors.
      }
    })
  }  

}
