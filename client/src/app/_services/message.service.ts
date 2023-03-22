import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { take } from 'rxjs';
import { environment } from 'src/environments/environment.development';
import { Conversation } from '../_models/conversation';
import { Message } from '../_models/message';
import { User } from '../_models/user';
import { UserParams } from '../_models/userParams';
import { AccountService } from './account.service';
import { getPaginatedResult, getPaginationHeaders } from './paginationHelper';

@Injectable({
  providedIn: 'root'
})
export class MessageService {
  baseUrl = environment.apiUrl;
  userParams: UserParams;
  user: User;

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

  getMessages(pageNumber, pageSize, container) {
    let params = getPaginationHeaders(pageNumber, pageSize);
    params = params.append("Container", container);

    return getPaginatedResult<Message[]>(this.baseUrl + 'messages', params, this.http);
  }

  getMessageThread(userParams: UserParams, username: string) {
    let params = getPaginationHeaders(userParams.pageNumber, userParams.pageSize);

    return getPaginatedResult<Message[]>(this.baseUrl + 'messages/thread/' + username, params, this.http);
  }

  getConversations() {
    return this.http.get<Conversation[]>(this.baseUrl + 'messages/conversations');
  }

  sendMessage(username: string, content: string) {
    return this.http.post<Message>(this.baseUrl + 'messages', { recipientUsername: username, content });
  }
}
