import { Component, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { Message } from 'src/app/_models/message';
import { Pagination } from 'src/app/_models/pagination';
import { UserParams } from 'src/app/_models/userParams';
import { MembersService } from 'src/app/_services/members.service';
import { MessageService } from 'src/app/_services/message.service';
import { PresenceService } from 'src/app/_services/presence.service';

@Component({
  selector: 'app-member-messages',
  templateUrl: './member-messages.component.html',
  styleUrls: ['./member-messages.component.css']
})
export class MemberMessagesComponent implements OnInit{
  @ViewChild('messageForm') messageForm: NgForm;
  username: string;
  messages: Message[];
  messageContent: string;
  pagination: Pagination;
  userParams: UserParams;
  pageNumber = 1;
  pageSize = 5;

  constructor(private messageService: MessageService, private route: ActivatedRoute,
    private membersService: MembersService, public presence: PresenceService) {
      this.userParams = this.membersService.getUserParams();
    }

  ngOnInit(): void {
    this.route.paramMap.subscribe(params => {
      this.username = params.get('username');
      this.loadMessages();
    });
  }
  

  loadMessages() {
    this.membersService.setUserParams(this.userParams);
    this.messageService.getMessageThread(this.userParams, this.route.snapshot.paramMap.get('username')).subscribe({
      next: messages => {
        this.messages = messages.result;
        this.pagination = messages.pagination;
      }
    })
  }

  sendMessage() {
    this.messageService.sendMessage(this.username, this.messageContent).subscribe({
      next: message => {
        if (this.pagination.currentPage === 1){
          this.messages.unshift(message);
        }
        this.messageForm.reset();
      }
    })
  }

  pageChanged(event: any) {
    this.userParams.pageNumber = event.page;
    this.membersService.setUserParams(this.userParams);
    this.loadMessages();
  }
}
