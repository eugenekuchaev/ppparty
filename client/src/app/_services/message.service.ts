import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { ToastrService } from 'ngx-toastr';
import { BehaviorSubject, take } from 'rxjs';
import { environment } from 'src/environments/environment.development';
import { Conversation } from '../_models/conversation';
import { Group } from '../_models/group';
import { Message } from '../_models/message';
import { User } from '../_models/user';
import { UserParams } from '../_models/userParams';
import { AccountService } from './account.service';

@Injectable({
  providedIn: 'root'
})
export class MessageService {
  baseUrl = environment.apiUrl;
  userParams: UserParams;
  user: User;
  hubUrl = environment.hubUrl;
  private hubConnection: HubConnection;
  private messageThreadSource = new BehaviorSubject<Message[]>([]);
  messageThread$ = this.messageThreadSource.asObservable();

  constructor(private http: HttpClient, private accountService: AccountService, private toastr: ToastrService) { 
    this.accountService.currentUser$.pipe(take(1)).subscribe({
      next: user => {
        this.user = user;
        this.userParams = new UserParams(user);
      }
    })
  }

  createHubConnection(user: User, otherUsername: string) {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(this.hubUrl + 'message?user=' + otherUsername, {
        accessTokenFactory: () => user.token
      })
      .withAutomaticReconnect()
      .build();
    
    this.hubConnection.start().catch(error => console.log(error));

    this.hubConnection.on("ReceiveMessageThread", messages => {
      this.messageThreadSource.next(messages);
    })

    this.hubConnection.on("NewMessage", message => {
      this.messageThread$.pipe(take(1)).subscribe({
        next: messages => {
          this.messageThreadSource.next([message, ...messages]);
        }
      })
    })

    this.hubConnection.on("UpdatedGroup", (group: Group) => {
      if (group.connections.some(x => x.username === otherUsername)) {
        this.messageThread$.pipe(take(1)).subscribe(messages => {
          messages.forEach(message => {
            if (!message.dateRead) {
              message.dateRead = new Date(Date.now())
            }
          })
          this.messageThreadSource.next([...messages]);
        })
      }
    })
  }

  stopHubConnection() {
    this.hubConnection.stop();
  }

  getConversations() {
    return this.http.get<Conversation[]>(this.baseUrl + 'messages/conversations');
  }

  async sendMessage(username: string, content: string) {
    return this.hubConnection.invoke("SendMessage", {recipientUsername: username, content})
      .catch(error => {
        console.log(error);
        this.toastr.error("The message is too long or you are not friends with this user");
      });
  }
}
