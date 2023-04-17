import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import {HttpClientModule, HTTP_INTERCEPTORS} from '@angular/common/http';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { NavComponent } from './nav/nav.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { HomeComponent } from './home/home.component';
import { RegisterComponent } from './register/register.component';
import { MemberListComponent } from './members/member-list/member-list.component';
import { MemberDetailComponent } from './members/member-detail/member-detail.component';
import { ListsComponent } from './lists/lists.component';
import { MessagesComponent } from './messages/messages.component';
import { ToastrModule } from 'ngx-toastr';
import { SharedModule } from './_modules/shared.module';
import { TestErrorComponent } from './errors/test-error/test-error.component';
import { ErrorsInterceptor } from './_interceptors/errors.interceptor';
import { NotFoundComponent } from './errors/not-found/not-found.component';
import { ServerErrorComponent } from './errors/server-error/server-error.component';
import { MemberCardComponent } from './members/member-card/member-card.component';
import { JwtInterceptor } from './_interceptors/jwt.interceptor';
import { MemberEditComponent } from './members/member-edit/member-edit.component';
import { LoadingInterceptor } from './_interceptors/loading.interceptor';
import { PhotoEditorComponent } from './members/photo-editor/photo-editor.component';
import { TextInputComponent } from './_forms/text-input/text-input.component';
import { DatePickerComponent } from './_forms/date-picker/date-picker.component';
import { MemberMessagesComponent } from './members/member-messages/member-messages.component';
import { AdminPanelComponent } from './admin/admin-panel/admin-panel.component';
import { HasRoleDirective } from './_directives/has-role.directive';
import { UserManagementComponent } from './admin/user-management/user-management.component';
import { PhotoManagementComponent } from './admin/photo-management/photo-management.component';
import { RolesModalComponent } from './modals/roles-modal/roles-modal.component';
import { ConfirmDialogComponent } from './modals/confirm-dialog/confirm-dialog.component';

//defines the root module of the application.
//may also contain other configuration options for the application, such as routes, authentication settings, and other global settings that are shared across the entire application.
// plays a critical role in configuring and initializing a .NET application, and is typically one of the first files that developers modify when starting a new project.
//modules are the building blocks of the application's structure and organization.
//is a logical unit that groups related components, services, directives, and pipes together
//It provides a way to organize the application into distinct, reusable parts, and to manage dependencies between them.
//Modules are used to define the boundaries of an Angular application and to manage the lifecycle of its components.

@NgModule({
  declarations: [ // for creating new component
    AppComponent, 
    NavComponent, 
    HomeComponent, 
    RegisterComponent, 
    MemberListComponent, 
    MemberDetailComponent, 
    ListsComponent, 
    MessagesComponent, 
    TestErrorComponent, 
    NotFoundComponent, 
    ServerErrorComponent, 
    MemberCardComponent, 
    MemberEditComponent, 
    PhotoEditorComponent, 
    TextInputComponent, 
    DatePickerComponent, 
    MemberMessagesComponent, 
    AdminPanelComponent, 
    HasRoleDirective, UserManagementComponent, PhotoManagementComponent, RolesModalComponent, ConfirmDialogComponent
  ],
  imports: [ // for creating new module
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    BrowserAnimationsModule,
    FormsModule, // 2-way binds between NAV framework component and html template to enable info sent to our component when users to complete in a form 
    ReactiveFormsModule,
    SharedModule

  ],
  providers: [
    {provide: HTTP_INTERCEPTORS, useClass: ErrorsInterceptor, multi: true},
    {provide: HTTP_INTERCEPTORS, useClass: JwtInterceptor, multi: true},
    {provide: HTTP_INTERCEPTORS, useClass: LoadingInterceptor, multi: true}
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
