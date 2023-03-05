import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { map, of } from 'rxjs';
import { environment } from 'src/environments/environment.development';
import { Member } from '../_models/member';

@Injectable({
  providedIn: 'root'
})
export class MembersService {
  baseUrl = environment.apiUrl;
  members: Member[] = []

  constructor(private http: HttpClient) { }

  getMembers() {
    if (this.members.length > 0) {
      return of(this.members);
    }
    return this.http.get<Member[]>(this.baseUrl + 'users').pipe(
      map(members => {
        this.members = members;
        return members;
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