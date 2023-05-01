import { Component, Input, OnInit } from '@angular/core';
import { AppEvent } from 'src/app/_models/appEvent';
import { EventDate } from 'src/app/_models/eventDate';
import { EventsService } from 'src/app/_services/events.service';

@Component({
  selector: 'app-event-card',
  templateUrl: './event-card.component.html',
  styleUrls: ['./event-card.component.css']
})
export class EventCardComponent implements OnInit {
  @Input() appEvent: AppEvent;
  @Input() recommendedEvents: AppEvent[];
  @Input() allEvents: AppEvent[];
  @Input() participatedEvents: AppEvent[];
  @Input() ownedEvents: AppEvent[];
  @Input() friendsEvents: AppEvent[];
  @Input() invites: AppEvent[] = [];
  @Input() showInviteButtons: boolean = false;
  @Input() showParticipateButtonInRecommended: boolean = false;
  @Input() showButtonsInAll: boolean = false;
  @Input() showCancelButtonInParticipated: boolean = false;
  @Input() showFriendsEventsButtons: boolean = false;
  @Input() showButtonsInOwned: boolean = false;
  shortenedEventDescription: string;

  constructor(private eventsService: EventsService) { }

  ngOnInit(): void {
    this.shortenEventDescription();
    this.checkIfEventIsInParticipated(this.appEvent);
  }

  participateInEvent(appEvent: AppEvent) {
    this.eventsService.participateInEvent(appEvent.id).subscribe({
      next: () => {
        if (this.invites?.findIndex(m => m.id === appEvent.id) !== -1) {
          const index = this.invites.findIndex(m => m.id === appEvent.id);
          if (index !== -1) {
            this.invites.splice(index, 1);
          }
        }

        if (this.recommendedEvents?.findIndex(m => m.id === appEvent.id) !== -1) {
          const index = this.recommendedEvents.findIndex(m => m.id === appEvent.id);
          if (index !== -1) {
            this.recommendedEvents.splice(index, 1);
            this.participatedEvents.push(appEvent);
          }
        }

        if (this.participatedEvents?.findIndex(m => m.id === appEvent.id) === -1) {
          this.participatedEvents.push(appEvent);
        }
      }
    });
  }

  stopParticipatingInEvent(appEvent: AppEvent) {
    this.eventsService.stopParticipatingInEvent(appEvent.id).subscribe({
      next: () => {
        if (this.participatedEvents?.findIndex(m => m.id === appEvent.id) !== -1) {
          const index = this.participatedEvents.findIndex(m => m.id === appEvent.id);
          if (index !== -1) {
            this.participatedEvents.splice(index, 1);
          }
        }
      }
    });
  }

  declineInvitation(appEvent: AppEvent) {
    this.eventsService.declineInvitation(appEvent.id).subscribe({
      next: () => {
        if (this.invites?.findIndex(m => m.id === appEvent.id) !== -1) {
          const index = this.invites.findIndex(m => m.id === appEvent.id);
          if (index !== -1) {
            this.invites.splice(index, 1);
          }
        }
      }
    })
  }

  cancelEvent(appEvent: AppEvent) {
    const confirmation = confirm("Are you sure you want to delete this event?");
    if (confirmation) {
      this.eventsService.cancelEvent(appEvent.id).subscribe({
        next: () => {
          appEvent.isEnded = true;
        }
      });
    } else {
    }
  }

  checkIfEventIsInParticipated(appEvent: AppEvent) {
    return this.participatedEvents.some(obj => obj.id === appEvent.id);
  }

  checkIfEventIsInOwned(appEvent: AppEvent) {
    return this.ownedEvents.some(obj => obj.id === appEvent.id);
  }

  formatEarliestDate(appEvent: AppEvent): string {
    const earliestDate: EventDate = appEvent.eventDates.reduce((earliest, current) => {
      if (current.startDate < earliest.startDate) {
        return current;
      }
      return earliest;
    });

    const formattedEarliestDate = new Date(earliestDate.startDate).toLocaleString('en-US', {
      timeZone: 'UTC',
      month: 'long',
      day: 'numeric',
      hour: 'numeric',
      minute: 'numeric',
    });

    return formattedEarliestDate;
  }

  shortenEventDescription() {
    if (this.appEvent.description.length > 90) {
      this.shortenedEventDescription = this.appEvent.description.substring(0, 90) + "...";
    } else {
      this.shortenedEventDescription = this.appEvent.description;
    }
  }
}
