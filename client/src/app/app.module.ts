import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import {HttpClientModule, HTTP_INTERCEPTORS} from '@angular/common/http';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { NavComponent } from './nav/nav.component';
import { FormsModule } from '@angular/forms';
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

//modules are the building blocks of the application's structure and organization.
//is a logical unit that groups related components, services, directives, and pipes together
//It provides a way to organize the application into distinct, reusable parts, and to manage dependencies between them.
//Modules are used to define the boundaries of an Angular application and to manage the lifecycle of its components.

@NgModule({
  declarations: [ // for creating new component
    AppComponent, NavComponent, HomeComponent, RegisterComponent, MemberListComponent, MemberDetailComponent, ListsComponent, MessagesComponent, TestErrorComponent, NotFoundComponent, ServerErrorComponent
  ],
  imports: [ // for creating new module
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    BrowserAnimationsModule,
    FormsModule, // 2-way binds between NAV framework component and html template to enable info sent to our component when users to complete in a form 
    SharedModule
  ],
  providers: [
    {provide: HTTP_INTERCEPTORS, useClass: ErrorsInterceptor, multi: true}
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
