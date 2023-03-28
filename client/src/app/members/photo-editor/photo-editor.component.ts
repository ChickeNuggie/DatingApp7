import { Component, Input, OnInit } from '@angular/core';
import { FileUploader } from 'ng2-file-upload';
import { take } from 'rxjs';
import { Member } from 'src/app/_models/member';
import { Photo } from 'src/app/_models/photo';
import { User } from 'src/app/_models/user';
import { AccountService } from 'src/app/_services/account.service';
import { MembersService } from 'src/app/_services/members.service';
import { environment } from 'src/environments/environment';

// child class of member-edit component
@Component({
  selector: 'app-photo-editor',
  templateUrl: './photo-editor.component.html',
  styleUrls: ['./photo-editor.component.css']
})
export class PhotoEditorComponent implements OnInit {
  // Pass down from parent (MemberEdits) component
  @Input() member: Member | undefined;
  uploader: FileUploader | undefined;
  hasBaseDropZoneOver =  false;
  baseUrl = environment.apiUrl;
  user: User | undefined; // require to inject AccountService to access and retrieve User object.

  constructor(private accountService: AccountService, private memberService: MembersService) {
    this.accountService.currentUser$.pipe(take(1)).subscribe({
      next: user => {
        if (user) this.user = user
      }
    })
    }

  ngOnInit(): void {
    this.initializeUploader();
  }

  // To get drop-zone functionality
  fileOverBase(e: any) {
    this.hasBaseDropZoneOver = e;
  }

  setMainPhoto(photo: Photo) {
    this.memberService.setMainPhoto(photo.id).subscribe({
      next: () => {
        if (this.user && this.member) {
          this.user.photoUrl = photo.url;
          //Current user observable and other components are subscribing to that current user object (nav bar)
          // Updating main photo here will also update any other component that's displaying user's main photo. (nav bar)
          this.accountService.setCurrentUser(this.user);
          //using photourl to display main image,user's detailed page and photo cards as well
          this.member.photoUrl = photo.url;
          this.member.photos.forEach(p => {
            if (p.isMain) p.isMain = false;
            if (p.id == photo.id) p.isMain = true;
          }) 
        }
      }
    })
  }

  deletePhoto(photoId: number) {
    this.memberService.deletePhoto(photoId).subscribe({
      next: _ => {
        if (this.member) {
          this.member.photos = this.member.photos.filter(x => x.id != photoId); // return all photos except for the one that matches this ID
        }
      }
    })
  }

  //Initialize the file upload
  initializeUploader() {
    this.uploader = new FileUploader({
      url: this.baseUrl + 'users/add-photo',// url address to send the image to.
      authToken: 'Bearer ' + this.user?.token,
      isHTML5: true,
      allowedFileType: ['image'], // allow any image format to be uploaded (jpg, png, wbs, etc.)
      removeAfterUpload: true, // prevent using too much memory space
      autoUpload: false, // have to manually on click action to upload a photo instead of automatic upload after insert images.
      maxFileSize: 10 * 1024 * 1024
    });

    // to prevent adjust to configuration after uploading file.
    this.uploader.onAfterAddingFile = (file) => {
      file.withCredentials = false
    }

    // Task or Action to do after file has been successfully uploaded.
    this.uploader.onSuccessItem = (item, response, status, headers) => {
      if (response) {
        const photo = JSON.parse(response);
        this.member?.photos.push(photo);
        //check if uploaded photo is the first photo and will be automatically set as main photo by the API on user and main page/component.
        if (photo.isMain && this.user && this.member) {
          this.user.photoUrl = photo.url;
          this.member.photoUrl = photo.url;
          this.accountService.setCurrentUser(this.user);
        }
      }
    }
  }



}
