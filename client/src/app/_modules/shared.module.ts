import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ToastrModule } from 'ngx-toastr';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { TabsModule } from 'ngx-bootstrap/tabs';
import { NgxGalleryModule } from '@kolkov/ngx-gallery';
import { NgxSpinnerModule } from 'ngx-spinner';
import { FileUploadModule } from 'ng2-file-upload';
import {BsDatepickerModule } from 'ngx-bootstrap/datepicker';

//create 3rd party component imports and separate from angular's component
//used to declare and export common components, directives (i.e. ngIf, ngFor), and pipes that are used across multiple modules within the application
//organize and modularize the common features, and avoid repeating the same code across the application.
@NgModule({
  declarations: [], // any components to declare in this array
  imports: [
    CommonModule,
    BsDropdownModule.forRoot(), 
    TabsModule.forRoot(),
    ToastrModule.forRoot({
      positionClass: 'toast-bottom-right'
    }),
    NgxGalleryModule,
    NgxSpinnerModule.forRoot({
      type: 'pacman'
    }),
    FileUploadModule,
    BsDatepickerModule.forRoot()
  ],

  //required to export name of modules only  in order for shared modules to work in app.modules.
  exports: [
    BsDropdownModule,
    ToastrModule,
    TabsModule,
    NgxGalleryModule,
    NgxSpinnerModule,
    FileUploadModule,
    BsDatepickerModule
  ]
})
export class SharedModule { }
