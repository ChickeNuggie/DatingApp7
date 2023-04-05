import { Component, OnInit } from '@angular/core';
import { take } from 'rxjs';
import { Member } from 'src/app/_models/member';
import { Pagination } from 'src/app/_models/pagination';
import { User } from 'src/app/_models/user';
import { UserParams } from 'src/app/_models/userParams';
import { AccountService } from 'src/app/_services/account.service';
import { MembersService } from 'src/app/_services/members.service';

@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.css']
})
export class MemberListComponent implements OnInit {
  // members$: Observable<Member[]> | undefined;
  members: Member[] = [];
  pagination: Pagination | undefined;
  userParams: UserParams | undefined;
  genderList = [{value: 'male', display: 'Males'}, {value: 'female', display: 'Females'}] // display on dropdown list

  constructor(private memberService: MembersService) {
    this.userParams = this.memberService.getUserParams();
   }


  ngOnInit(): void {
    // this.members$ = this.memberService.getMembers();
    this.loadMembers();
  }

  loadMembers() {
    if (this.userParams) {
      this.memberService.setUserParams(this.userParams); // set to whatever user selected
      this.memberService.getMembers(this.userParams).subscribe({ // get members
        next: response => {
          if (response.result && response.pagination) {
            this.members = response.result;
            this.pagination = response.pagination;
          }
          console.log(response.result)
          console.log(response.pagination)
        }
      }) 
    }

  }


  resetFilters() {
      this.userParams = this.memberService.resetUserParams(); // back to default first page gender and age.
      this.loadMembers(); // get resetted list of users basesd on default parameter
    }


  // take an event of any type
  pageChanged(event: any) {
    // check if userparams and page number not the same as page on click (event) and prevent any funky behaviour caused by multiple request.
    if (this.userParams && this.userParams?.pageNumber !== event.page) {
      this.userParams.pageNumber = event.page;
      this,this.memberService.setUserParams(this.userParams); // updated memberservice as well
      this.loadMembers(); // retrieve updated content of member page
  
    }
  }

}
