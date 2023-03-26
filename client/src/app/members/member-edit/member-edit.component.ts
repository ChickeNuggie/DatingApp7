import { Component, HostListener, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { take } from 'rxjs';
import { Member } from 'src/app/_models/member';
import { User } from 'src/app/_models/user';
import { AccountService } from 'src/app/_services/account.service';
import { MembersService } from 'src/app/_services/members.service';

@Component({
  selector: 'app-member-edit',
  templateUrl: './member-edit.component.html',
  styleUrls: ['./member-edit.component.css']
})
export class MemberEditComponent implements OnInit {
  @ViewChild('editForm') editForm: NgForm | undefined; // looks for template reference variable that matches the name input here
  //gets access to browser than angular application 
  // browser (host) listens to browser event before reload and provide simialr warnings of unsaved changes.
  // Note: Anuglar does not know what's happening with the browser as a whole but knows if users moving around to different angular components
  @HostListener('window:beforeunload', ["$event"]) unloadNotification($event:any) {
    if (this.editForm?.dirty) { //
      $event.returnValue = true;
    }
  } 
  
  member: Member | undefined; //initialize variable as at the time this component is constructed. it will be undefined and require to get from some other services.
  // it will contain updated information for that member or user when form is submitted.
  user: User | null = null;

  // access current user observable of account and members service to populate user and member object abbove that was initiated.
  constructor(private accountService: AccountService, private memberService: MembersService, 
    private toastr: ToastrService) {
    this.accountService.currentUser$.pipe(take(1)).subscribe({ // only take one request from user which is then completed and thus, need not unsubscribe
      next: user => this.user = user 
    })
   }

  ngOnInit(): void {
    this.loadMember();
  }
  

  loadMember() {
    if (!this.user) return;
    this.memberService.getMember(this.user.userName).subscribe({
      next: member => this.member = member
    })
  }

  updateMember() {
    this.memberService.updateMember(this.editForm?.value).subscribe({
      next: _ => {
        this.toastr.success('Profile updated successfully');
        this.editForm?.reset(this.member);//resets to current member's profile updated
      }
    })

  }

}
