import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { take } from 'rxjs';
import { AppEvent } from 'src/app/_models/appEvent';
import { AppEventDetail } from 'src/app/_models/appEventDetail';
import { Member } from 'src/app/_models/member';
import { User } from 'src/app/_models/user';
import { AccountService } from 'src/app/_services/account.service';
import { EventsService } from 'src/app/_services/events.service';
import { MembersService } from 'src/app/_services/members.service';

@Component({
  selector: 'app-event-detail',
  templateUrl: './event-detail.component.html',
  styleUrls: ['./event-detail.component.css']
})
export class EventDetailComponent implements OnInit {
  appEvent: AppEventDetail;
  numberOfOwnedEvents: Number;
  user: User;
  member: Member;
  eventLoaded: boolean;
  showParticipateButton: boolean;
  showCancelParticipationButton: boolean;
  showEventOwnerButtons: boolean;
  showEventOwnerCancelledButtons: boolean;
  showCancelledButton: boolean;

  constructor(private eventsService: EventsService, private route: ActivatedRoute, private accountService: AccountService,
    private membersService: MembersService) { 
    this.accountService.currentUser$.pipe(take(1)).subscribe({
      next: user => this.user = user
    })
  }

  ngOnInit(): void {
    this.loadEvent();
    this.getNumberOfOwnedEvents();
    this.membersService.getMember(this.user.username).subscribe({
      next: response => {
        this.member = response;
      }
    })
  }

  loadEvent() {
    this.eventsService.getEvent(this.route.snapshot.paramMap.get('eventId')).subscribe({
      next: appEvent => {
        this.appEvent = appEvent;
        this.eventLoaded = true; 
        this.getNumberOfOwnedEvents();
      }
    })
  }

  participateInEvent(appEvent: AppEvent) {
    this.eventsService.participateInEvent(appEvent.id).subscribe({
      next: () => {
        this.appEvent.participants.push(this.member);
      }
    });
  }

  stopParticipatingInEvent(appEvent: AppEvent) {
    this.eventsService.stopParticipatingInEvent(appEvent.id).subscribe({
      next: () => {
        if (this.appEvent.participants?.findIndex(m => m.id === this.member.id) !== -1) {
          const index = this.appEvent.participants.findIndex(m => m.id === this.member.id);
          if (index !== -1) {
            this.appEvent.participants.splice(index, 1);
          }
        }
      }
    });
  }

  cancelEvent(appEvent: AppEvent) {
    const confirmation = confirm("Are you sure you want to delete this event?");
    if (confirmation) {
      this.eventsService.cancelEvent(appEvent.id).subscribe({
        next: () => {
          this.appEvent.isCancelled = true;
        }
      });
    } else {
    }
  }

  getDate(eventDate: any): string {
    const start = new Date(eventDate.startDate);
    const startStr = this.getDateString(start);
    return startStr;
  }

  getTime(eventDate: any): string {
    const start = new Date(eventDate.startDate);
    const end = new Date(eventDate.endDate);
    const timeStr = this.getTimeString(start, end);
    return timeStr;
  }

  getDateString(date: Date): string {
    let dateString = date.toLocaleString('en-US', {
      timeZone: 'UTC',
      month: 'long',
      day: 'numeric'
    });

    if (date.getUTCFullYear() > new Date().getUTCFullYear()) {
      dateString += ', ' + date.getUTCFullYear();
    }
    
    return dateString;
  }

  getTimeString(start: Date, end: Date): string {
    const startStr = start.toLocaleTimeString('en-US', {
      timeZone: 'UTC',
      hour: 'numeric',
      minute: 'numeric'
    });

    const endStr = end.toLocaleTimeString('en-US', {
      timeZone: 'UTC',
      hour: 'numeric',
      minute: 'numeric'
    });

    return startStr + ' — ' + endStr;
  }

  getTenParticipants() {
    return this.appEvent.participants.slice(0, 10);
  }

  getTenFriendsParticipants() {
    return this.appEvent.friendsParticipants.slice(0, 10);
  }

  getNumberOfOwnedEvents() {
    if (this.eventLoaded) { 
      return this.eventsService.getNumberOfOwnedEvents(this.appEvent.eventOwner.username).subscribe({
        next: response => {
          this.numberOfOwnedEvents = response;
        }
      })
    }
  }

  checkIfUserIsOwner() {
    return this.appEvent.eventOwner.username === this.user.username;
  }

  checkIfUserIsParticipating() {
    return this.appEvent.participants.some(x => x.username === this.user.username);
  }
}
