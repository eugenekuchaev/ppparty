import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
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
import { AuthGuard } from './_guards/auth.guard';
import { EventDetailComponent } from './events/event-detail/event-detail.component';
import { EventInvitesFromMembersComponent } from './events/event-invites-from-members/event-invites-from-members.component';
import { CreateEventComponent } from './events/create-event/create-event.component';
import { PhotoEditorComponent } from './members/photo-editor/photo-editor.component';
import { EditEventComponent } from './events/edit-event/edit-event.component';
import { PreventUnsavedChangesEditEventGuard } from './_guards/prevent-unsaved-changes-edit-event.guard';
import { PreventUnsavedChangesEventCreationGuard } from './_guards/prevent-usaved-changes-event-creation.guard';

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
      {path: 'events/:eventId', component: EventDetailComponent},
      {path: 'events/invitefrommembers/:username', component: EventInvitesFromMembersComponent},
      {path: 'createevent', component: CreateEventComponent, canDeactivate: [PreventUnsavedChangesEventCreationGuard]},
      {path: 'photoeditor', component: PhotoEditorComponent},
      {path: 'editevent/:eventId', component: EditEventComponent, canDeactivate: [PreventUnsavedChangesEditEventGuard]}
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
