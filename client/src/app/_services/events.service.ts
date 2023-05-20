import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment.development';
import { AppEvent } from '../_models/appEvent';
import { User } from '../_models/user';
import { EventParams } from '../_models/eventParams';
import { HttpClient } from '@angular/common/http';
import { AccountService } from './account.service';
import { map, of, take } from 'rxjs';
import { getPaginatedResult, getPaginationHeaders } from './paginationHelper';
import { EventNotification } from '../_models/eventNotification';
import { AppEventDetail } from '../_models/appEventDetail';

@Injectable({
  providedIn: 'root'
})
export class EventsService {
  baseUrl = environment.apiUrl;
  events: AppEvent[] = [];
  eventCache = new Map();
  user: User;
  eventParams: EventParams;

  constructor(private http: HttpClient, private accountService: AccountService) {
    this.accountService.currentUser$.pipe(take(1)).subscribe({
      next: user => {
        this.user = user;
        this.eventParams = new EventParams(user);
      }
    })
  }

  getEvents(eventParams: EventParams) {
    var response = this.eventCache.get(Object.values(eventParams).join('-'));
    if (response) {
      return of(response);
    }

    let params = getPaginationHeaders(eventParams.pageNumber, eventParams.pageSize);

    if (eventParams.eventName != null) {
      params = params.append('eventName', eventParams.eventName);
    }
    if (eventParams.fromDate != null) {
      params = params.append('fromDate', eventParams.fromDate);
    }
    if (eventParams.toDate != null) {
      params = params.append('toDate', eventParams.toDate);
    }
    if (eventParams.country != null) {
      params = params.append('country', eventParams.country);
    }
    if (eventParams.region != null) {
      params = params.append('region', eventParams.region);
    }
    if (eventParams.city != null) {
      params = params.append('city', eventParams.city);
    }
    if (eventParams.eventTag != null) {
      params = params.append('userInterest', eventParams.eventTag);
    }

    return getPaginatedResult<AppEvent[]>(this.baseUrl + 'events/all-events', params, this.http).pipe(
      map(response => {
        this.eventCache.set(Object.values(eventParams).join('-'), response);
        return response;
      })
    )
  }

  getEvent(eventId: string) {
    return this.http.get<AppEventDetail>(this.baseUrl + 'events/' + eventId);
  }

  getOwnedEvents() {
    return this.http.get<Partial<AppEvent[]>>(this.baseUrl + 'events/owned-events');
  }

  getParticipatedEvents() {
    return this.http.get<Partial<AppEvent[]>>(this.baseUrl + 'events/participated-events');
  }

  getFriendsEvents() {
    return this.http.get<Partial<AppEvent[]>>(this.baseUrl + 'events/friends-events');
  }

  getRecommendedEvents() {
    return this.http.get<Partial<AppEvent[]>>(this.baseUrl + 'events/recommended-events');
  }

  getEventNotifications() {
    return this.http.get<Partial<EventNotification[]>>(this.baseUrl + 'events/event-notifications');
  }

  createEvent(appEvent: AppEvent) {
    return this.http.post(this.baseUrl + 'events', appEvent);
  }

  participateInEvent(eventId: Number) {
    return this.http.patch(this.baseUrl + 'events/participate-in-event/' + eventId, {});
  }

  stopParticipatingInEvent(eventId: Number) {
    return this.http.patch(this.baseUrl + 'events/stop-participating-in-event/' + eventId, {});
  }

  updateEvent(appEvent: AppEvent, eventId: Number) {
    return this.http.put(this.baseUrl + 'events/update-event/' + eventId, appEvent);
  }

  addTags(tags: string, eventId: Number) {
    const options = { headers: { 'Content-Type': 'application/json' } };
    return this.http.post(this.baseUrl + 'events/add-tags/' + eventId, JSON.stringify(tags), options);
  }

  removeTag(tag: string, eventId: Number) {
    const options = { headers: { 'Content-Type': 'application/json' } };
    return this.http.patch(this.baseUrl + 'events/remove-tag/' + eventId, JSON.stringify(tag), options);
  }

  deleteEvent(eventId: Number) {
    return this.http.delete(this.baseUrl + 'events/delete-event/' + eventId);
  }

  inviteToEvent(username: String, eventId: Number) {
    return this.http.patch(this.baseUrl + 'events/invite-to-event/' + eventId + '?username=' + username, {});
  }

  getInvites() {
    return this.http.get<Partial<AppEvent[]>>(this.baseUrl + 'events/invites');
  }

  declineInvitation(eventId: Number) {
    return this.http.patch(this.baseUrl + 'events/decline-invitation/' + eventId, {});
  }

  cancelEvent(eventId: Number) {
    return this.http.patch(this.baseUrl + 'events/cancel-event/' + eventId, {});
  }

  readEventNotification(notificationId: Number) {
    return this.http.patch(this.baseUrl + 'events/read-event-notification/' + notificationId, {});
  }

  getNumberOfOwnedEvents(username: String) {
    return this.http.get<Number>(this.baseUrl + 'events/number-of-owned-events/' + username);
  }

  getEventsForInvitations(friendUsername: String) {
    return this.http.get<AppEvent[]>(this.baseUrl + 'events/events-for-invitations' + '?friendUsername=' + friendUsername);
  }

  checkIfUserHasBeenInvited(username: String, eventId: Number) {
    return this.http.get<Boolean>(this.baseUrl + 'events/has-user-been-invited-to-event' + '?username=' + username + "&eventId=" + eventId);
  }

  getEventParams() {
    return this.eventParams;
  }

  setEventParams(params: EventParams) {
    this.eventParams = params;
  }

  resetEventParams() {
    this.eventParams = new EventParams(this.user);
    return this.eventParams;
  }
}
