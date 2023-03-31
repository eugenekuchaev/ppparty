import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { take } from 'rxjs/operators';
import { Conversation } from '../_models/conversation';
import { User } from '../_models/user';
import { AccountService } from '../_services/account.service';
import { MessageService } from '../_services/message.service';
import { PresenceService } from '../_services/presence.service';

@Component({
  selector: 'app-messages',
  templateUrl: './messages.component.html',
  styleUrls: ['./messages.component.css']
})
export class MessagesComponent implements OnInit {
  conversations: Conversation[] = [];
  user: User;

  constructor(
    private router: Router,
    public presence: PresenceService,
    private accountService: AccountService,
    private messageService: MessageService
  ) {
    this.accountService.currentUser$.pipe(take(1)).subscribe({
      next: user => {
        this.user = user;
      }
    });
  }

  ngOnInit(): void {
    this.loadConversations();
    this.presence.newMessageReceived$.subscribe(() => {
      this.loadConversations();
    });
  }

  private loadConversations(): void {
    this.messageService.getConversations().subscribe({
      next: conversations => {
        this.conversations = conversations;
      }
    });
  }

  onRowClicked(username: string) {
    this.router.navigate(['/messages', username]);
  }

  shortenMessage(message: string) {
    if (message.length > 50) {
      return message.substring(0, 50) + '...';
    } else {
      return message;
    }
  }
}



