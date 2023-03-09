import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { take } from 'rxjs';
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
    private memberService: MembersService) {
      this.accountService.currentUser$.pipe(take(1)).subscribe({
        next: user => this.user = user
      })
    }

  ngOnInit(): void {
    this.loadMember();
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
      }
    })
  }
}
