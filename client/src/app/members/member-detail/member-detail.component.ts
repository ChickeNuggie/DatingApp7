import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { NgxGalleryAnimation, NgxGalleryImage, NgxGalleryOptions } from '@kolkov/ngx-gallery';
import { Member } from 'src/app/_models/member';
import { MembersService } from 'src/app/_services/members.service';

@Component({
  selector: 'app-member-detail',
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css']
})
export class MemberDetailComponent implements OnInit {
  member: Member | undefined; // forced to check if member's exist before attemp to access and use members' method it.
  galleryOptions: NgxGalleryOptions[] = [];
  galleryImages: NgxGalleryImage[] = [];
  //Activate this route which take the user to the member detail component and access the route parameter of the username upon clicking onto the link.
  constructor(private memberService: MembersService, private route: ActivatedRoute) { }

  ngOnInit(): void {
    this.loadMember();

    this.galleryOptions = [
      {
        width: '500px',
        height: '500px',
        imagePercent: 100,
        thumbnailsColumns: 4,
        imageAnimation: NgxGalleryAnimation.Slide,
        preview: false
      }
    ]

  }

  getImages () {
  if (!this.member) return []; // return empty array to ensure that the API returns this particular empty array method if user does not exist.
  const imageUrls = []; // loop over member's photo if user exist
  for (const photo of this.member.photos) {
    imageUrls.push({ // standard property name small, medium and big.
      small: photo.url,
      medium: photo.url,
      big: photo.url
    })
  }
  return imageUrls;
  }

  loadMember() {
    //get access to snapshot of currently activated routes snapshots.
    //paramap = map/array of root parameters inside root snapshots
    var username = this.route.snapshot.paramMap.get('username')
    if (!username) return; // stops the execution of the method to prevent unauthorized users from accessing the member information..
    this.memberService.getMember(username).subscribe({ // getmethod from memberservice and access to observable object and get back from API
      next: member => {
        this.member = member;
        this.galleryImages = this.getImages(); 
      }
      // nex tto specify sets the value of the member property on the current class instance to the emitted value.
    })
   //gets the username parameter from the current activated route snapshot and uses it to request a member object from an API service.
    //If the username value is not present in the snapshot, the method does not proceed with the API request. 
    //When the member object is retrieved, the method sets the member property on the current class instance to the retrieved value.

  }
}
