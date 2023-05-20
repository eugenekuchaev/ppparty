import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map, of, take } from 'rxjs';
import { environment } from 'src/environments/environment.development';
import { Member } from '../_models/member';
import { User } from '../_models/user';
import { UserParams } from '../_models/userParams';
import { AccountService } from './account.service';
import { getPaginatedResult, getPaginationHeaders } from './paginationHelper';

@Injectable({
  providedIn: 'root'
})
export class MembersService {
  baseUrl = environment.apiUrl;
  members: Member[] = [];
  memberCache = new Map();
  user: User;
  userParams: UserParams;

  constructor(private http: HttpClient, private accountService: AccountService) {
    this.accountService.currentUser$.pipe(take(1)).subscribe({
      next: user => {
        this.user = user;
        this.userParams = new UserParams(user);
      }
    })
  }

  getUserParams() {
    return this.userParams;
  }

  setUserParams(params: UserParams) {
    this.userParams = params;
  }

  resetUserParams() {
    this.userParams = new UserParams(this.user);
    return this.userParams;
  }

  getMembers(userParams: UserParams) {
    var response = this.memberCache.get(Object.values(userParams).join('-'));
    if (response) {
      return of(response);
    }

    let params = getPaginationHeaders(userParams.pageNumber, userParams.pageSize);

    if (userParams.fullName != null) {
      params = params.append('fullName', userParams.fullName);
    }
    if (userParams.username != null) {
      params = params.append('username', userParams.username);
    }
    if (userParams.country != null) {
      params = params.append('country', userParams.country);
    }
    if (userParams.region != null) {
      params = params.append('region', userParams.region);
    }
    if (userParams.city != null) {
      params = params.append('city', userParams.city);
    }
    if (userParams.userInterest != null) {
      params = params.append('userInterest', userParams.userInterest);
    }

    return getPaginatedResult<Member[]>(this.baseUrl + 'users', params, this.http).pipe(
      map(response => {
        this.memberCache.set(Object.values(userParams).join('-'), response);
        return response;
      })
    )
  }

  getMember(username: string) {
    const member = [...this.memberCache.values()]
      .reduce((array, element) => array.concat(element.result), [])
      .find((member: Member) => member.username === username);

    if (member) {
      return of(member);
    }

    return this.http.get<Member>(this.baseUrl + 'users/' + username);
  }

  updateName(member: Member) {
    return this.http.patch(this.baseUrl + 'users/update-name', member);
  }

  updateLocation(member: Member) {
    return this.http.patch(this.baseUrl + 'users/update-location', member);
  }

  updateAbout(member: Member) {
    return this.http.patch(this.baseUrl + 'users/update-about', member);
  }

  removeInterest(interest: string) {
    const options = { headers: { 'Content-Type': 'application/json' } };
    return this.http.patch(this.baseUrl + 'users/remove-interest', JSON.stringify(interest), options);
  }

  addInterests(interests: string) {
    const options = { headers: { 'Content-Type': 'application/json' } };
    return this.http.post(this.baseUrl + 'users/add-interests', JSON.stringify(interests), options);
  }

  updateContacts(member: Member) {
    return this.http.patch(this.baseUrl + 'users/update-contacts', member);
  }

  addFriend(username: string) {
    return this.http.patch(this.baseUrl + 'friends/add-friend/' + username, {});
  }

  deleteFriend(username: string) {
    return this.http.patch(this.baseUrl + 'friends/delete-friend/' + username, {});
  }

  getFriends(predicate: string) {
    return this.http.get<Partial<Member[]>>(this.baseUrl + 'friends?predicate=' + predicate);
  }
} 
