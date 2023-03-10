import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor,
  HttpErrorResponse
} from '@angular/common/http';
import { catchError, Observable } from 'rxjs';
import { NavigationExtras, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';

// Intercept to handle appropriate response based on type of errors
@Injectable()
export class ErrorsInterceptor implements HttpInterceptor {

  // Inject router to redirect user if neded depending on the error  got back from API.
  constructor(private router: Router, private toastr: ToastrService) {} 

  //request http and what happen next from http handler
  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    return next.handle(request).pipe( // modify obervable by using pipe to modify error message
      catchError((error: HttpErrorResponse) => {
        if (error) {
          switch (error.status) {
            case 400: 
              if(error.error.errors) {
                const modelStateErrors = [];
                  for (const key in error.error.errors) {
                    if (error.error.errors[key]) { // check if errors based on key exsit
                      modelStateErrors.push(error.error.errors[key]) // build and store array of errors from validation errors
                    }
                }
                throw modelStateErrors.flat();
              } else {
                this.toastr.error(error.error, error.status.toString())
              }
              break;
            case 401:
              this.toastr.error('Unauthorized', error.status.toString());
              break;
            case 404:
              this.router.navigateByUrl('/not-found');
              break;
            //access error response and display error information on the page
            case 500:
              const navigationExtras: NavigationExtras =  {state: {error: error.error}}; // router is capbale of receiving API exception state
              this.router.navigateByUrl('/server-error', navigationExtras);// use navigationExtras's information to display on component template.
              break;
            default:
              this.toastr.error('Something unexpected went wrong');
              console.log(error)
              break;
          }
        }
        throw error;
      })
    ) 
  }
}
