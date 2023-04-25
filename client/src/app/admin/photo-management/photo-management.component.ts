import { Component, OnInit } from '@angular/core';
import { take } from 'rxjs';
import { Photo } from 'src/app/_models/photo';
import { AdminService } from 'src/app/_services/admin.service';

@Component({
  selector: 'app-photo-management',
  templateUrl: './photo-management.component.html',
  styleUrls: ['./photo-management.component.css']
})
export class PhotoManagementComponent implements OnInit {
  photos: Photo[] =[];

  constructor(private adminService: AdminService) { }

  ngOnInit(): void {
    this.getPhotosForApproval();
  }

  getPhotosForApproval() 
  {
  //adminservice =  retrieve the photos, which likely communicates with a backend API. 11
  //The subscribe() method is called on the observable returned by adminService.getPhotosForApproval()
  //which allows the function to handle the asynchronous response when the photos are retrieved.
  //When the photos are successfully retrieved, the next callback function is executed, which sets the this.photos property to the array of photos that were returned by the service
  //This makes the photos available for use in the component where this function is called.
  //Overall, this function sets up an asynchronous request to retrieve photos for approval and sets the photos property on the component once the request is complete.
    this.adminService.getPhotosForApproval().subscribe({
      next: photos => this.photos = photos
    })
  }

  approvePhoto(photoId: number)
  {
    this.adminService.approvePhoto(photoId).subscribe({
      next: () => this.photos.splice(this.photos.findIndex(p => p.id === photoId), 1)
    })
  }

  rejectPhoto(photoId: number) 
  {
    //admin services = using an Angular service called adminService to communicate with a backend API and reject the photo.
    //subscribe() method is called on the observable returned by adminService.rejectPhoto(photoId), which allows the function to handle the asynchronous response when the rejection is complete.
    //When the rejection is successful, the next callback function is executed, which removes the rejected photo from the this.photos array.
    //This is done using the splice() method, which removes one element from the array at the index where the rejected photo is found. 
    //findIndex() method is used to find the index of the rejected photo in the array by searching for the photo with the matching photoId before removal.
    //Overall, this function sets up an asynchronous request to reject a photo and removes the rejected photo from the component's this.photos array once the request is complete.(which is to be reflected in front-end view)
    this.adminService.rejectPhoto(photoId).subscribe({
      next: () => this.photos.splice(this.photos.findIndex(p => p.id === photoId), 1)
    })
  }
  

}
