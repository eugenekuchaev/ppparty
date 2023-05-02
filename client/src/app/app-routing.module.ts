import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AdminPanelComponent } from './admin/admin-panel/admin-panel.component';
import { NotFoundComponent } from './errors/not-found/not-found.component';
import { ServerErrorComponent } from './errors/server-error/server-error.component';
import { TestErrorsComponent } from './errors/test-errors/test-errors.component';
import { EventListComponent } from './events/event-list/event-list.component';
import { HomeComponent } from './home/home.component';
import { MemberDetailComponent } from './members/member-detail/member-detail.component';
import { MemberEditComponent } from './members/member-edit/member-edit.component';
import { MemberListComponent } from './members/member-list/member-list.component';
import { MemberSecurityComponent } from './members/member-security/member-security.component';
import { MemberMessagesComponent } from './messages/member-messages/member-messages.component';
import { MessagesComponent } from './messages/messages.component';
import { AdminGuard } from './_guards/admin.guard';
import { AuthGuard } from './_guards/auth.guard';
import { EventDetailComponent } from './events/event-detail/event-detail.component';
import { EventInvitesFromMembersComponent } from './events/event-invites-from-members/event-invites-from-members.component';

const routes: Routes = [
  {path: '', component: HomeComponent},
  {
    path: '',
    runGuardsAndResolvers: 'always',
    canActivate: [AuthGuard],
    children: [
      {path: 'events', component: EventListComponent},
      {path: 'members', component: MemberListComponent},
      {path: 'members/:username', component: MemberDetailComponent},
      {path: 'member/edit', component: MemberEditComponent},
      {path: 'messages', component: MessagesComponent},
      {path: 'member/security', component: MemberSecurityComponent},
      {path: 'messages/:username', component: MemberMessagesComponent},
      {path: 'admin', component: AdminPanelComponent, canActivate: [AdminGuard]},
      {path: 'events/:eventId', component: EventDetailComponent},
      {path: 'events/invitefrommembers/:username', component: EventInvitesFromMembersComponent}
    ]
  },
  {path: 'errors', component: TestErrorsComponent}, 
  {path: 'not-found', component: NotFoundComponent},
  {path: 'server-error', component: ServerErrorComponent},
  {path: '**', component: NotFoundComponent, pathMatch: 'full'},
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
