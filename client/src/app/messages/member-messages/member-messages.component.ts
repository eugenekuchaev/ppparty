import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { take } from 'rxjs';
import { Message } from 'src/app/_models/message';
import { Pagination } from 'src/app/_models/pagination';
import { User } from 'src/app/_models/user';
import { UserParams } from 'src/app/_models/userParams';
import { AccountService } from 'src/app/_services/account.service';
import { MembersService } from 'src/app/_services/members.service';
import { MessageService } from 'src/app/_services/message.service';
import { PresenceService } from 'src/app/_services/presence.service';

@Component({
  selector: 'app-member-messages',
  templateUrl: './member-messages.component.html',
  styleUrls: ['./member-messages.component.css']
})
export class MemberMessagesComponent implements OnInit, OnDestroy {
  @ViewChild('messageForm') messageForm: NgForm;
  username: string;
  messages: Message[];
  messageContent: string;
  pagination: Pagination;
  userParams: UserParams;
  pageNumber = 1;
  pageSize = 5;
  user: User;

  constructor(public messageService: MessageService, private route: ActivatedRoute,
    private membersService: MembersService, public presence: PresenceService, private accountService: AccountService) {
    this.userParams = this.membersService.getUserParams();
    this.accountService.currentUser$.pipe(take(1)).subscribe({
      next: user => this.user = user
    })
  }

  ngOnInit(): void {
    this.route.paramMap.subscribe(params => {
      this.username = params.get('username');
      this.messageService.createHubConnection(this.user, this.username);
    });
  }

  ngOnDestroy(): void {
    this.messageService.stopHubConnection();
  }

  sendMessage() {
    this.messageService.sendMessage(this.username, this.messageContent).then(() => {
      this.messageForm.reset();
    })
  }
}
