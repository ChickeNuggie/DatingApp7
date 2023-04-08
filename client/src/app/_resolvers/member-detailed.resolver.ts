import { Injectable } from '@angular/core';
import {
  Router, Resolve,
  RouterStateSnapshot,
  ActivatedRouteSnapshot
} from '@angular/router';
import { Observable, of } from 'rxjs';
import { MembersService } from '../_services/members.service';
import { Member } from '../_models/member';

//initialize when app first started
// It gets the data before the root is activated, having data before component is constructed
// i.e. member is going to be available before the view is even available.
@Injectable({
  providedIn: 'root'
})

export class MemberDetailedResolver implements Resolve<Member> {
  constructor(private memberService: MembersService) {

  }
  
  // a route function, requires to update in app-routing.model
  resolve(route: ActivatedRouteSnapshot): Observable<Member> {
    return this.memberService.getMember(route.paramMap.get('username')!)
  }
}
