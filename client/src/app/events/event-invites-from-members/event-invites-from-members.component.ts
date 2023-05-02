import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { AppEvent } from 'src/app/_models/appEvent';
import { EventsService } from 'src/app/_services/events.service';

@Component({
  selector: 'app-event-invites-from-members',
  templateUrl: './event-invites-from-members.component.html',
  styleUrls: ['./event-invites-from-members.component.css']
})
export class EventInvitesFromMembersComponent implements OnInit {
  eventsForInvitations: AppEvent[] = [];
  friendUsername: String;

  constructor(private eventsService: EventsService, private route: ActivatedRoute) {}

  ngOnInit(): void {
    this.getEventsForInvitations();
  }

  getEventsForInvitations() {
    this.friendUsername = this.route.snapshot.paramMap.get('username');
    this.eventsService.getEventsForInvitations(this.friendUsername).subscribe({
      next: response => {
        this.eventsForInvitations = response;
      }
    })
  }
}
