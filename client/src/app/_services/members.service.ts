import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map, of } from 'rxjs';
import { environment } from 'src/environments/environment.development';
import { Member } from '../_models/member';
import { PaginatedResult } from '../_models/pagination';
import { UserParams } from '../_models/userParams';

@Injectable({
  providedIn: 'root'
})
export class MembersService {
  baseUrl = environment.apiUrl;
  members: Member[] = [];

  constructor(private http: HttpClient) { }

  getMembers(userParams: UserParams) {
    let params = this.getPaginationHeaders(userParams.pageNumber, userParams.pageSize);

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
    
    return this.getPaginatedResult<Member[]>(this.baseUrl + 'users', params);
  }

  getMember(username: string) {
    const member = this.members.find(x => x.username === username);

    if (member !== undefined) {
      return of(member);
    }

    return this.http.get<Member>(this.baseUrl + 'users/' + username);
  }

  updateName(member: Member) {
    return this.http.put(this.baseUrl + 'users/updatename', member);
  }

  updateLocation(member: Member) {
    return this.http.put(this.baseUrl + 'users/updatelocation', member);
  }

  updateAbout(member: Member) {
    return this.http.put(this.baseUrl + 'users/updateabout', member);
  }

  deleteInterest(interest: string) {
    return this.http.delete(this.baseUrl + 'users/deleteinterest', { params: { interestName: interest } });
  }

  addInterests(interests: string) {
    const options = { headers: { 'Content-Type': 'application/json' } };
    return this.http.post(this.baseUrl + 'users/addinterests', JSON.stringify(interests), options);
  }

  updateContacts(member: Member) {
    return this.http.put(this.baseUrl + 'users/updatecontacts', member);
  }

  private getPaginatedResult<T>(url, params) {
    const paginatedResult: PaginatedResult<T> = new PaginatedResult<T>();
    return this.http.get<T>(url, { observe: 'response', params }).pipe(
      map(response => {
        paginatedResult.result = response.body;

        if (response.headers.get('Pagination') !== null) {
          paginatedResult.pagination = JSON.parse(response.headers.get('Pagination'));
        }

        return paginatedResult;
      })
    );
  }

  private getPaginationHeaders(pageNumber: number, pageSize: number) {
    let params = new HttpParams();

    params = params.append('pageNumber', pageNumber.toString());
    params = params.append('pageSize', pageSize.toString());

    return params;
  }
} 
