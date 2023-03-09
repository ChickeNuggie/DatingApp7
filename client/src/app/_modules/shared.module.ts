import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ToastrModule } from 'ngx-toastr';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';

//create 3rd party component imports and separate from angular's component
@NgModule({
  declarations: [], // any components to declare in this array
  imports: [
    CommonModule,
    BsDropdownModule.forRoot(), 
    ToastrModule.forRoot({
      positionClass: 'toast-bottom-right'
    })
  ],

  //required to export name of modules only  in order for shared modules to work in app.modules.
  exports: [
    BsDropdownModule,
    ToastrModule
  ]

})
export class SharedModule { }
