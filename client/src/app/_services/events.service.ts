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

    return getPaginatedResult<AppEvent[]>(this.baseUrl + 'events/allevents', params, this.http).pipe(
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
    return this.http.get<Partial<AppEvent[]>>(this.baseUrl + 'events/ownedevents');
  }

  getParticipatedEvents() {
    return this.http.get<Partial<AppEvent[]>>(this.baseUrl + 'events/participatedevents');
  }

  getFriendsEvents() {
    return this.http.get<Partial<AppEvent[]>>(this.baseUrl + 'events/friendsevents');
  }

  getRecommendedEvents() {
    return this.http.get<Partial<AppEvent[]>>(this.baseUrl + 'events/recommendedevents');
  }

  getEventNotifications() {
    return this.http.get<Partial<EventNotification[]>>(this.baseUrl + 'events/eventnotifications');
  }

  createEvent(appEvent: AppEvent) {
    return this.http.post(this.baseUrl + 'events', appEvent);
  }

  participateInEvent(eventId: Number) {
    return this.http.put(this.baseUrl + 'events/participateinevent/' + eventId, {});
  }

  stopParticipatingInEvent(eventId: Number) {
    return this.http.delete(this.baseUrl + 'events/stopparticipatinginevent/' + eventId);
  }

  updateEvent(appEvent: AppEvent, eventId: Number) {
    return this.http.put(this.baseUrl + 'events/updateevent/' + eventId, appEvent);
  }

  addTags(tags: string, eventId: Number) {
    const options = { headers: { 'Content-Type': 'application/json' } };
    return this.http.post(this.baseUrl + 'events/addtags' + eventId, JSON.stringify(tags), options);
  }

  deleteTag(tag: string, eventId: Number) {
    return this.http.delete(this.baseUrl + 'events/deletetag/' + eventId, { params: { eventTagName: tag } });
  }

  deleteEvent(eventId: Number) {
    return this.http.delete(this.baseUrl + 'events/deleteevent/' + eventId);
  }

  inviteToEvent(username: String, eventId: Number) {
    return this.http.put(this.baseUrl + 'events/invitetoevent/' + eventId + '?username=' + username, {});
  }

  getInvites() {
    return this.http.get<Partial<AppEvent[]>>(this.baseUrl + 'events/invites');
  }

  declineInvitation(eventId: Number) {
    return this.http.delete(this.baseUrl + 'events/declineinvitation/' + eventId);
  }

  cancelEvent(eventId: Number) {
    return this.http.put(this.baseUrl + 'events/cancelevent/' + eventId, {});
  }

  readEventNotification(notificationId: Number) {
    return this.http.put(this.baseUrl + 'events/readeventnotification/' + notificationId, {});
  }

  getNumberOfOwnedEvents(username: String) {
    return this.http.get<Number>(this.baseUrl + 'events/numberofownedevents/' + username);
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
