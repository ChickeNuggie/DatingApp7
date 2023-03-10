import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { NotFoundComponent } from './errors/not-found/not-found.component';
import { ServerErrorComponent } from './errors/server-error/server-error.component';
import { TestErrorComponent } from './errors/test-error/test-error.component';
import { HomeComponent } from './home/home.component';
import { ListsComponent } from './lists/lists.component';
import { MemberDetailComponent } from './members/member-detail/member-detail.component';
import { MemberListComponent } from './members/member-list/member-list.component';
import { MessagesComponent } from './messages/messages.component';
import { AuthGuard } from './_guards/auth.guard';

const routes: Routes = [
{path: '', component: HomeComponent}, // empty route to home component
{path: '', 
runGuardsAndResolvers: 'always',
canActivate: [AuthGuard],  // authenticate guard on following list of child routes.
children: [ 
    {path: 'members', component: MemberListComponent, canActivate: [AuthGuard]}, 
    {path: 'members/:id', component: MemberDetailComponent},
    {path: 'lists', component: ListsComponent},
    {path: 'messages', component: MessagesComponent},
    ]
  },  

  {path: 'errors', component: TestErrorComponent}, 
  {path: 'not-found', component: NotFoundComponent}, 
  {path: 'server-error', component: ServerErrorComponent}, 
  {path: '**', component: NotFoundComponent, pathMatch: 'full'},// all other routes no in the lists of array above.
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
