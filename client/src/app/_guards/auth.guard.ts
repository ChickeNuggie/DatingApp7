import { Injectable } from '@angular/core';
import { CanActivate} from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { map, Observable } from 'rxjs';
import { AccountService } from '../_services/account.service';

//security comes from API but not client side, authguard only can hide content and avoid user trying to modify by using tools such as root guards.
//auto subscribe and unsubscribe.
@Injectable({
  providedIn: 'root' // inform that this going to instatiated when application starts,
  // ensure that authentication guard is available all the time whileist application is alive. 
})

 // inject service into guards as it contains observable of current user object information.
export class AuthGuard implements CanActivate {
  constructor(private accountService: AccountService, private toastr: ToastrService) {}


   canActivate(): Observable<boolean> {
    // return something that is not a user (need to project out of the current user to be able to return a boolean value)
    return this.accountService.currentUser$.pipe(
      map(user =>{
        if (user) return true;
        else {
          this.toastr.error('You shall not pass!');
          return false;
        }
      }) // project boolean value out of currentUser 
    )
  }
  
}
