import { Component, Input, OnInit } from '@angular/core';
import { AppEvent } from 'src/app/_models/appEvent';
import { EventsService } from 'src/app/_services/events.service';

@Component({
  selector: 'app-event-invites-from-members-card',
  templateUrl: './event-invites-from-members-card.component.html',
  styleUrls: ['./event-invites-from-members-card.component.css']
})
export class EventInvitesFromMembersCardComponent implements OnInit{
  @Input() appEvent: AppEvent;
  @Input() username: String;
  hasBeenInvited: Boolean;

  constructor(private eventsService: EventsService) {}

  ngOnInit(): void {
    this.checkIfUserHasBeenInvited();
  }

  checkIfUserHasBeenInvited() {
    this.eventsService.checkIfUserHasBeenInvited(this.username, this.appEvent.id).subscribe({
      next: response => {
        this.hasBeenInvited = response;
      }
    })
  }

  inviteToEvent() {
    this.eventsService.inviteToEvent(this.username, this.appEvent.id).subscribe({
      next: () => {}
    })
    
    this.hasBeenInvited = true;
  }
}
