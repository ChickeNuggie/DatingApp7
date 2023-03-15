import { Component, Input, OnInit } from '@angular/core';
import { Member } from 'src/app/_models/member';

@Component({
  selector: 'app-member-card',
  templateUrl: './member-card.component.html',
  styleUrls: ['./member-card.component.css']
})
export class MemberCardComponent implements OnInit {
  @Input() member: Member | undefined; // initial value undefined before passing it down as input property. 
  //(force check that there's a member before attempt to use it from input property).
  constructor() { }

  ngOnInit(): void {
  }

}
