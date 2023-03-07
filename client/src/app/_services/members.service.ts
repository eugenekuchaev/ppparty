import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { map, of } from 'rxjs';
import { environment } from 'src/environments/environment.development';
import { Member } from '../_models/member';
import { PaginatedResult } from '../_models/pagination';

@Injectable({
  providedIn: 'root'
})
export class MembersService {
  baseUrl = environment.apiUrl;
  members: Member[] = [];
  paginatedResult: PaginatedResult<Member[]> = new PaginatedResult<Member[]>();

  constructor(private http: HttpClient) { }

  getMembers(page?: number, itemsPerPage?: number) {
    let params = new HttpParams();

    if (page !== null && itemsPerPage !== null) {
      params = params.append('pageNumber', page.toString());
      params = params.append('pageSize', itemsPerPage.toString());
    }

    return this.http.get<Member[]>(this.baseUrl + 'users', { observe: 'response', params }).pipe(
      map(response => {
        this.paginatedResult.result = response.body;

        if (response.headers.get('Pagination') !== null) {
          this.paginatedResult.pagination = JSON.parse(response.headers.get('Pagination'));
        }

        return this.paginatedResult;
      })
    )
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
} 
