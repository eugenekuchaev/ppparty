import { Component, Input, OnInit } from '@angular/core';
import { Member } from 'src/app/_models/member';

@Component({
  selector: 'app-member-card',
  templateUrl: './member-card.component.html',
  styleUrls: ['./member-card.component.css']
})
export class MemberCardComponent implements OnInit {
  @Input() member: Member;
  shortenedMemberFullName: string;
  shortenedMemberCity: string;

  ngOnInit(): void {
    this.shortenFullName();
    this.shortenCity();
  }

  shortenFullName() {
    if (this.member.fullName.length > 19) {
      this.shortenedMemberFullName = this.member.fullName.substring(0, 19) + "...";
    } else {
      this.shortenedMemberFullName = this.member.fullName;
    }
  }

  shortenCity() {
    if (this.member.city.length > 17) {
      this.shortenedMemberCity = this.member.city.substring(0, 17) + "...";
    } else {
      this.shortenedMemberCity = this.member.city;
    }
  }
}
