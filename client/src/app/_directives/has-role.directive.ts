import { Directive, Input, OnInit, TemplateRef, ViewContainerRef } from '@angular/core';
import { User } from '../_models/user';
import { AccountService } from '../_services/account.service';
import { take } from 'rxjs';

@Directive({
  //similar to ngIf where it allows certain roles to display its elements.
  //*appHasRole='["Admin", "Thing"]'
  selector: '[appHasRole]' 
})
export class HasRoleDirective implements OnInit {
  @Input() appHasRole: string[] = [];
  user: User = {} as User;

  constructor(private viewContainerRef: ViewContainerRef, private templateRef: TemplateRef<any>,
    private accountService: AccountService) {
      this.accountService.currentUser$.pipe(take(1)).subscribe({
        next: user => {
          if (user) this.user = user
        }
      })
     }
     
  ngOnInit(): void {
    // To check if users' roles at least matches roles inside appHasRole, display contain.
    if (this.user.roles.some(r => this.appHasRole.includes(r))) {
      this.viewContainerRef.createEmbeddedView(this.templateRef);
    } else {
      this.viewContainerRef.clear() //removing element from the DOM (i.e. admin link remove from DOM is users roles does not match against appHasRole)
    }
  }

}
