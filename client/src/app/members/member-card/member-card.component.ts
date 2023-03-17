import { Component, Input, OnInit } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { Member } from 'src/app/_models/member';
import { MembersService } from 'src/app/_services/members.service';

@Component({
  selector: 'app-member-card',
  templateUrl: './member-card.component.html',
  styleUrls: ['./member-card.component.css']
})
export class MemberCardComponent implements OnInit {
  @Input() member: Member;
  @Input() addedToFriends: Partial<Member[]>;
  @Input() friendRequests: Partial<Member[]>;
  @Input() mutualFriends: Partial<Member[]>;
  shortenedMemberFullName: string;
  shortenedMemberCity: string;

  constructor(private memberService: MembersService, private toastr: ToastrService) { }

  ngOnInit(): void {
    this.shortenFullName();
    this.shortenCity();
  }

  isMemberInAddedToFriends() {
    return this.addedToFriends?.findIndex(m => m.id === this.member.id) !== -1;
  }

  isMemberSentAFriendRequest() {
    return this.friendRequests?.findIndex(m => m.id === this.member.id) !== -1;
  }

  isMemberAMutualFriend() {
    return this.mutualFriends?.findIndex(m => m.id === this.member.id) !== -1;
  }

  addFriend(member: Member) {
    this.memberService.addFriend(member.username).subscribe({
      next: () => {
        if (this.friendRequests?.findIndex(m => m.id === this.member.id) === -1) {
          this.toastr.success('You have sent a friend request to ' + member.fullName);

          this.addedToFriends.push(member);
        }

        if (this.friendRequests?.findIndex(m => m.id === this.member.id) !== -1) {
          this.toastr.success('You have added ' + member.fullName + ' to friends');

          const index = this.friendRequests.findIndex(m => m.id === this.member.id);
          if (index !== -1) {
            this.friendRequests.splice(index, 1);
          }

          this.mutualFriends.push(member);
        }
      }
    })
  }

  deleteFriend(member: Member) {
    this.memberService.deleteFriend(member.username).subscribe({
      next: () => {
        if (this.mutualFriends?.findIndex(m => m.id === this.member.id) !== -1) {
          this.toastr.warning('You have deleted ' + member.fullName + ' from friends');

          const index1 = this.mutualFriends.findIndex(m => m.id === this.member.id);
          if (index1 !== -1) {
            this.mutualFriends.splice(index1, 1);
          }

          const index2 = this.addedToFriends.findIndex(m => m.id === this.member.id);
          if (index2 !== -1) {
            this.addedToFriends.splice(index2, 1);
          }

          this.friendRequests.push(member);
        } else if (this.mutualFriends?.findIndex(m => m.id === this.member.id) === -1) {
          this.toastr.warning('You have recalled a friend request to ' + member.fullName);

          const index = this.addedToFriends.findIndex(m => m.id === this.member.id);
          if (index !== -1) {
            this.addedToFriends.splice(index, 1);
          }
        }
      }
    })
  }

  shortenFullName() {
    if (this.member.fullName.length > 14) {
      this.shortenedMemberFullName = this.member.fullName.substring(0, 14) + "...";
    } else {
      this.shortenedMemberFullName = this.member.fullName;
    }
  }

  shortenCity() {
    if (this.member.city.length > 14) {
      this.shortenedMemberCity = this.member.city.substring(0, 14) + "...";
    } else {
      this.shortenedMemberCity = this.member.city;
    }
  }
}
