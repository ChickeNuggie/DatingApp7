import { Component, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { NgxGalleryAnimation, NgxGalleryImage, NgxGalleryOptions } from '@kolkov/ngx-gallery';
import { TabDirective, TabsetComponent } from 'ngx-bootstrap/tabs';
import { Member } from 'src/app/_models/member';
import { Message } from 'src/app/_models/message';
import { MembersService } from 'src/app/_services/members.service';
import { MessageService } from 'src/app/_services/message.service';

@Component({
  selector: 'app-member-detail',
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css']
})
// Note: parent class also loads child dependency methods and properties as well.
export class MemberDetailComponent implements OnInit {
  @ViewChild('memberTabs', {static: true}) memberTabs?: TabsetComponent;
  member: Member = {} as Member; // forced to check if member's exist before attemp to access and use members' method it. Since member comes from route, database it will always.
  galleryOptions: NgxGalleryOptions[] = [];
  galleryImages: NgxGalleryImage[] = [];
  //Activate this route which take the user to the member detail component and access the route parameter of the username upon clicking onto the link.
  activeTab?: TabDirective;
  messages: Message[] = [];
  
  
  constructor(private memberService: MembersService, private route: ActivatedRoute,
     private messageService: MessageService) { }

  ngOnInit(): void {
   //get member data from app route instead
   this.route.data.subscribe({
    next: data => {
      this.member = data['member'] // access member property inside the route
    }
   })

    this.route.queryParams.subscribe({
      next: params => {
        //ensure have parameters before attempt to use the parameters
        params['tab'] && this.selectTab(params['tab'])
      }
    })

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
    this.galleryImages = this.getImages(); 

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


  selectTab(heading: string) {
    if (this.memberTabs) {
      //find parameter in array of tabs and set active property to true.
      //! turnss off typescript safety.
      this.memberTabs.tabs.find(x => x.heading === heading)!.active = true;
    }
  }


  loadMessages() {
    if (this.member) {
      this.messageService.getMessageThread(this.member.userName).subscribe({
        next: messages =>
        this.messages = messages
      })
    }
  }

  //do something when this method is is activated and check if user are on message tab
  onTabActivated(data: TabDirective)
  {
    this.activeTab = data;
    // Only if messages tab is activated, then messages will be shown
    if (this.activeTab?.heading === 'Messages') {
      this.loadMessages();
    }
  }
}
