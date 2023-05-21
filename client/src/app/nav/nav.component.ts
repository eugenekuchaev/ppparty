import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Member } from '../_models/member';
import { User } from '../_models/user';
import { AccountService } from '../_services/account.service';
import { MembersService } from '../_services/members.service';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {
  model: any = {}
  member: Member;
  user: User;

  constructor(public accountService: AccountService, private router: Router,
    private memberService: MembersService) { }

  ngOnInit(): void {
    this.accountService.currentUser$.subscribe({
      next: user => {
        if (user) {
          this.user = user;
          this.loadMember();
        }
      }
    });
  }

  login() {
    this.accountService.login(this.model).subscribe({
      next: () => {
        this.router.navigateByUrl('/events');
      }
    })
  }

  logout() {
    this.accountService.logout();
    this.router.navigateByUrl('/');
  }

  loadMember() {
    this.memberService.getMember(this.user.username).subscribe({
      next: member => {
        this.member = member;
        this.user.fullName = this.member.fullName;
      }
    })
  }
}
