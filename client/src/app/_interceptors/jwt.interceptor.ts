import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor
} from '@angular/common/http';
import { Observable, take } from 'rxjs';
import { AccountService } from '../_services/account.service';

// Adds authorization header along with bearer token so that user can get authenticated by API when request list of members/single member information (than inside indivvidual method level services).
// Generally, application is loaded when using interceptors and thus, able to get token from account service
// than getting if from local storage as it is done in the root component and update account service
@Injectable()
export class JwtInterceptor implements HttpInterceptor {

  constructor(private accountService: AccountService) {}

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    //only subscribe onces and wont consume further resources in our app. (do not need to manually unsubscribed from this observable)
    this.accountService.currentUser$.pipe(take(1)) .subscribe({
      next: user => {
        if (user) {
          request = request.clone({
            setHeaders: {
              Authorization: `Bearer ${user.token}`
            }
          })
        }
      }
    })
    return next.handle(request);
  }
}
