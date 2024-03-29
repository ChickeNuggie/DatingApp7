import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { NotFoundComponent } from './errors/not-found/not-found.component';
import { ServerErrorComponent } from './errors/server-error/server-error.component';
import { TestErrorComponent } from './errors/test-error/test-error.component';
import { HomeComponent } from './home/home.component';
import { ListsComponent } from './lists/lists.component';
import { MemberDetailComponent } from './members/member-detail/member-detail.component';
import { MemberEditComponent } from './members/member-edit/member-edit.component';
import { MemberListComponent } from './members/member-list/member-list.component';
import { MessagesComponent } from './messages/messages.component';
import { AuthGuard } from './_guards/auth.guard';
import { PreventUnsavedChangesGuard } from './_guards/prevent-unsaved-changes.guard';
import { MemberDetailedResolver } from './_resolvers/member-detailed.resolver';
import { AdminPanelComponent } from './admin/admin-panel/admin-panel.component';
import { AdminGuard } from './_guards/admin.guard';

const routes: Routes = [
{path: '', component: HomeComponent}, // empty route to home component
{path: '', 
runGuardsAndResolvers: 'always',
canActivate: [AuthGuard],  // authenticate guard on following list of child routes.
children: [ 
    {path: 'members', component: MemberListComponent, canActivate: [AuthGuard]}, 
    {path: 'members/:username', component: MemberDetailComponent, resolve: {member: MemberDetailedResolver}}, // get member from root/route than member service.
    {path: 'member/edit', component: MemberEditComponent, canDeactivate: [PreventUnsavedChangesGuard]}, // allows individual member to edit own profile upon authentication
    {path: 'lists', component: ListsComponent},
    {path: 'messages', component: MessagesComponent},
    //In order to get to admin access, user need to authenticate and get past off auth guard, and needs to be an admin or moderator.
    {path: 'admin', component: AdminPanelComponent, canActivate: [AdminGuard]},
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
