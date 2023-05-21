import { Component, OnInit } from '@angular/core';
import { AppEvent } from 'src/app/_models/appEvent';
import { EventNotification } from 'src/app/_models/eventNotification';
import { EventParams } from 'src/app/_models/eventParams';
import { Pagination } from 'src/app/_models/pagination';
import { User } from 'src/app/_models/user';
import { EventsService } from 'src/app/_services/events.service';

@Component({
  selector: 'app-event-list',
  templateUrl: './event-list.component.html',
  styleUrls: ['./event-list.component.css']
})
export class EventListComponent implements OnInit {
  recommendedEvents: AppEvent[] = [];
  allEvents: AppEvent[];
  participatedEvents: AppEvent[] = [];
  ownedEvents: AppEvent[] = [];
  friendsEvents: AppEvent[] = [];
  invites: AppEvent[] = [];
  eventNotifications: EventNotification[] = [];
  pagination: Pagination;
  eventParams: EventParams;
  user: User;
  recommendedEventsIsActivated = true;
  allEventsIsActivated = false;
  participatedEventsIsActivated = false;
  ownedEventsIsActivated = false;
  friendsEventsIsActivated = false;
  eventInvitesIsActivated = false;
  eventNotificationsIsActivated = false;
  searchOpen = false;
  showEndedParticipatedEvents = false;
  showEndedOwnedEvents = false;
  hideNotificationBadges = false;

  ngOnInit(): void {
    this.loadRecommendedEvents();
    this.loadParticipatedEvents();
    this.loadOwnedEvents();
    this.loadInvites();
    this.loadEventNotifications();
  }

  constructor(private eventsService: EventsService) {
    this.eventParams = this.eventsService.getEventParams();
  }

  loadRecommendedEvents() {
    this.eventsService.getRecommendedEvents().subscribe({
      next: response => {
        this.recommendedEvents = response;
      }
    })
  }

  loadAllEvents() {
    this.eventsService.setEventParams(this.eventParams);
    this.eventsService.getEvents(this.eventParams).subscribe({
      next: response => {
        this.allEvents = response.result;
        this.pagination = response.pagination;
      }
    })
  }

  loadParticipatedEvents() {
    this.eventsService.getParticipatedEvents().subscribe({
      next: response => {
        this.participatedEvents = response;
      }
    })
  }

  loadOwnedEvents() {
    this.eventsService.getOwnedEvents().subscribe({
      next: response => {
        this.ownedEvents = response;
      }
    })
  }

  loadFriendsEvents() {
    this.eventsService.getFriendsEvents().subscribe({
      next: response => {
        this.friendsEvents = response;
      }
    })
  }

  loadInvites() {
    this.eventsService.getInvites().subscribe({
      next: response => {
        this.invites = response;
      }
    })
  }

  loadEventNotifications() {
    this.eventsService.getEventNotifications().subscribe({
      next: response => {
        this.eventNotifications = response;
      }
    })
  }

  getNumberOfUnreadNotifications() {
    return this.eventNotifications.filter(notification => !notification.read).length;
  }

  clickOnEventNotificationsWhenUnreadNotifications() {
    this.clickOnEventNotifications();
    this.hideNotificationBadges = true;
  }

  clickOnSearch() {
    this.searchOpen = !this.searchOpen;
  }

  resetFilters() {
    this.eventParams = this.eventsService.resetEventParams();
    this.loadAllEvents();
  }

  pageChanged(event: any) {
    this.eventParams.pageNumber = event.page;
    this.eventsService.setEventParams(this.eventParams);
    this.loadAllEvents();
  }

  checkIfParticipatedHasEndedEvents() {
    return this.participatedEvents.some(obj => obj.isEnded === true);
  }

  checkIfOwnedHasEndedEvents() {
    return this.ownedEvents.some(obj => obj.isEnded === true);
  }

  clickOnRecommendedEvents() {
    this.recommendedEventsIsActivated = true;
    this.allEventsIsActivated = false;
    this.participatedEventsIsActivated = false;
    this.ownedEventsIsActivated = false;
    this.friendsEventsIsActivated = false;
    this.eventInvitesIsActivated = false;
    this.eventNotificationsIsActivated = false;
  }

  clickOnAllEvents() {
    this.loadAllEvents();
    this.recommendedEventsIsActivated = false;
    this.allEventsIsActivated = true;
    this.participatedEventsIsActivated = false;
    this.ownedEventsIsActivated = false;
    this.friendsEventsIsActivated = false;
    this.eventInvitesIsActivated = false;
    this.eventNotificationsIsActivated = false;
  }

  clickOnParticipatedEvents() {
    this.recommendedEventsIsActivated = false;
    this.allEventsIsActivated = false;
    this.participatedEventsIsActivated = true;
    this.ownedEventsIsActivated = false;
    this.friendsEventsIsActivated = false;
    this.eventInvitesIsActivated = false;
    this.eventNotificationsIsActivated = false;
  }

  clickOnOwnedEvents() {
    this.recommendedEventsIsActivated = false;
    this.allEventsIsActivated = false;
    this.participatedEventsIsActivated = false;
    this.ownedEventsIsActivated = true;
    this.friendsEventsIsActivated = false;
    this.eventInvitesIsActivated = false;
    this.eventNotificationsIsActivated = false;
  }

  clickOnFriendsEvents() {
    this.loadFriendsEvents();
    this.recommendedEventsIsActivated = false;
    this.allEventsIsActivated = false;
    this.participatedEventsIsActivated = false;
    this.ownedEventsIsActivated = false;
    this.friendsEventsIsActivated = true;
    this.eventInvitesIsActivated = false;
    this.eventNotificationsIsActivated = false;
  }

  clickOnEventInvites() {
    this.recommendedEventsIsActivated = false;
    this.allEventsIsActivated = false;
    this.participatedEventsIsActivated = false;
    this.ownedEventsIsActivated = false;
    this.friendsEventsIsActivated = false;
    this.eventInvitesIsActivated = true;
    this.eventNotificationsIsActivated = false;
  }

  clickOnEventNotifications() {
    this.recommendedEventsIsActivated = false;
    this.allEventsIsActivated = false;
    this.participatedEventsIsActivated = false;
    this.ownedEventsIsActivated = false;
    this.friendsEventsIsActivated = false;
    this.eventInvitesIsActivated = false;
    this.eventNotificationsIsActivated = true;
  }
}
