import { Injectable } from '@angular/core';
import { CanDeactivate } from '@angular/router';
import { MemberEditComponent } from '../members/member-edit/member-edit.component';

@Injectable({
  providedIn: 'root'
})
export class PreventUnsavedChangesGuard implements CanDeactivate<MemberEditComponent> {
  canDeactivate(
    component: MemberEditComponent): boolean { //true/false if they can move forward to other template or stay where they are now 
    if (component.editForm?.dirty) {
      return confirm('Are you sure you want to continue? Any unsaved changes will be lost'); //'confirm' function returns true/false depending on user's selection
    }
    return true;
  }
  
}
