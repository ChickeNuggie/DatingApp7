import { Component, OnInit } from '@angular/core';
import { Member } from '../_models/member';
import { MembersService } from '../_services/members.service';
import { Pagination } from '../_models/pagination';

@Component({
  selector: 'app-lists',
  templateUrl: './lists.component.html',
  styleUrls: ['./lists.component.css']
})
export class ListsComponent implements OnInit {
  members: Member[] | undefined;
  predicate = 'liked';
  pageNumber = 1;
  pageSize = 5;
  pagination: Pagination | undefined;

  constructor(private memberService: MembersService) { }

  ngOnInit(): void {
    this.loadLikes();
  }

  loadLikes() {
    this.memberService.getLikes(this.predicate, this.pageNumber, this.pageSize).subscribe({
      next: response => {
        this.members = response.result; // returns pagination results
        this.pagination = response.pagination;
      }

    })
  }

    // take an event of any type
    pageChanged(event: any) {
      // check if userparams and page number not the same as page on click (event) and prevent any funky behaviour caused by multiple request.
      if ( this.pageNumber !== event.page) {
        this.pageNumber = event.page;
        this.loadLikes(); // retrieve updated content of member page
    
      }
    }
  
  

}
