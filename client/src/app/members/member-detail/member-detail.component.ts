import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { FriendshipStatus } from 'src/app/_models/friendshipStatus';
import { Member } from 'src/app/_models/member';
import { MembersService } from 'src/app/_services/members.service';
import { PresenceService } from 'src/app/_services/presence.service';

@Component({
  selector: 'app-member-detail',
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css']
})
export class MemberDetailComponent implements OnInit {
  member: Member;
  friendshipStatus: FriendshipStatus;
  FriendshipStatus = FriendshipStatus;

  constructor(private memberService: MembersService, private route: ActivatedRoute, public presence: PresenceService) {}

  ngOnInit(): void {
    this.loadMember();
  }

  loadMember() {
    this.memberService.getMember(this.route.snapshot.paramMap.get('username')).subscribe({
      next: member => {
        this.member = member;
        this.getUsersFriendship(this.member.username);
      }
    })
  }

  addFriend(member: Member) {
    this.memberService.addFriend(member.username).subscribe({
      next: () => {
        if (this.friendshipStatus === FriendshipStatus.NoFriendship) {
          this.friendshipStatus = FriendshipStatus.AddingToFriends;
        }

        if (this.friendshipStatus === FriendshipStatus.AddedToFriends) {
          this.friendshipStatus = FriendshipStatus.AreFriends;
        }
      }
    })
  }

  deleteFriend(member: Member) {
    this.memberService.deleteFriend(member.username).subscribe({
      next: () => {
        if (this.friendshipStatus === FriendshipStatus.AddingToFriends) {
          this.friendshipStatus = FriendshipStatus.NoFriendship;
        }

        if (this.friendshipStatus === FriendshipStatus.AreFriends) {
          this.friendshipStatus = FriendshipStatus.AddedToFriends;
        }
      }
    })
  }

  getUsersFriendship(secondUserUsername: string) {
    this.memberService.getUsersFriendship(secondUserUsername).subscribe({
      next: response => {
        this.friendshipStatus = response;
      }
    })
  }
}
