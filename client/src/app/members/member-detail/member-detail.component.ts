import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Member } from 'src/app/_models/member';
import { User } from 'src/app/_models/user';
import { MembersService } from 'src/app/_services/members.service';
import { PresenceService } from 'src/app/_services/presence.service';

@Component({
  selector: 'app-member-detail',
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css']
})
export class MemberDetailComponent implements OnInit {
  member: Member;
  user: User;
  mutualFriends: Partial<Member[]>;
  friendRequests: Partial<Member[]>;
  addedToFriends: Partial<Member[]>;

  constructor(private memberService: MembersService, private route: ActivatedRoute, public presence: PresenceService) {}

  ngOnInit(): void {
    this.loadMember();
  }

  loadMember() {
    this.memberService.getMember(this.route.snapshot.paramMap.get('username')).subscribe({
      next: member => {
        this.member = member;
      }
    })
  }
}
