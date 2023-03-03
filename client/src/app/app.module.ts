import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import {HttpClientModule} from '@angular/common/http';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

//modules are the building blocks of the application's structure and organization.
//is a logical unit that groups related components, services, directives, and pipes together
//It provides a way to organize the application into distinct, reusable parts, and to manage dependencies between them.
//Modules are used to define the boundaries of an Angular application and to manage the lifecycle of its components.

@NgModule({
  declarations: [ // for creating new component
    AppComponent
  ],
  imports: [ // for creating new module
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    BrowserAnimationsModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
