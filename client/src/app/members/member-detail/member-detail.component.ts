import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { NgxGalleryAnimation, NgxGalleryImage, NgxGalleryOptions } from '@kolkov/ngx-gallery';
import { TabDirective, TabsetComponent } from 'ngx-bootstrap/tabs';
import { take } from 'rxjs';
import { Member } from 'src/app/_models/member';
import { Message } from 'src/app/_models/message';
import { User } from 'src/app/_models/user';
import { AccountService } from 'src/app/_services/account.service';
import { MembersService } from 'src/app/_services/members.service';
import { MessageService } from 'src/app/_services/message.service';
import { PresenceService } from 'src/app/_services/presence.service';

@Component({
  selector: 'app-member-detail',
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css']
})
// Note: parent class also loads child dependency methods and properties as well.
// OnDestory = do something when this component is destroyed.
export class MemberDetailComponent implements OnInit, OnDestroy {
  @ViewChild('memberTabs', {static: true}) memberTabs?: TabsetComponent;
  member: Member = {} as Member; // forced to check if member's exist before attemp to access and use members' method it. Since member comes from route, database it will always.
  galleryOptions: NgxGalleryOptions[] = [];
  galleryImages: NgxGalleryImage[] = [];
  //Activate this route which take the user to the member detail component and access the route parameter of the username upon clicking onto the link.
  activeTab?: TabDirective;
  messages: Message[] = [];
  user?: User;
  
  
  constructor(private accountService: AccountService, private route: ActivatedRoute,
     private messageService: MessageService, public presenceService: PresenceService,
      private router: Router) {
        this.accountService.currentUser$.pipe(take(1)).subscribe({ // subscribe to current use observable and populate user.
          next: user => {
            if (user) this.user = user;
          }
        });
        //force the component to reload in order to load messages or other informations.
        this.router.routeReuseStrategy.shouldReuseRoute = () => false;
      }


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
  
  //stop hub connection from receiving messages when move away from application.
  ngOnDestroy(): void {
    this.messageService.stopHubConnection();
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
    // Only if messages tab is activated and the current user exists, then messages will be shown
    if (this.activeTab?.heading === 'Messages' && this.user) {
      //load from signalR connection hub instead
      // make conenction to messagehub when access message threads in client browser.
      this.messageService.createHubConnection(this.user, this.member.userName);
    } else {
      this.messageService.stopHubConnection();
    }
  }
}
