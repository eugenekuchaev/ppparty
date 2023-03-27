import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Conversation } from '../_models/conversation';
import { MessageService } from '../_services/message.service';
import { PresenceService } from '../_services/presence.service';

@Component({
  selector: 'app-messages',
  templateUrl: './messages.component.html',
  styleUrls: ['./messages.component.css']
})
export class MessagesComponent implements OnInit {
  conversations: Conversation[] = [];

  constructor(private messageService: MessageService, private router: Router, public presence: PresenceService) {}

  ngOnInit(): void {
    this.loadConversations();
  }

  loadConversations() {
    this.messageService.getConversations().subscribe({
      next: response => {
        this.conversations = response;
      }
    })
  }

  onRowClicked(username: string) {
    this.router.navigate(['/messages', username]);
  }

  shortenMessage(message: string) {
    if (message.length > 50) {
      return message.substring(0, 50) + "...";
    } else {
      return message;
    }
  }
}
