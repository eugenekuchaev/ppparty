import { Component, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { take } from 'rxjs';
import { Member } from 'src/app/_models/member';
import { User } from 'src/app/_models/user';
import { AccountService } from 'src/app/_services/account.service';
import { MembersService } from 'src/app/_services/members.service';

@Component({
  selector: 'app-member-security',
  templateUrl: './member-security.component.html',
  styleUrls: ['./member-security.component.css']
})
export class MemberSecurityComponent {
  @ViewChild('editEmail') editEmail: NgForm;
  @ViewChild('editPassword') editPassword: NgForm;
  member: Member;
  user: User;

  constructor(private memberService: MembersService, private accountService: AccountService,
    private toastr: ToastrService) {
    this.accountService.currentUser$.pipe(take(1)).subscribe({
      next: user => this.user = user
    })
  }

  ngOnInit(): void {
    this.loadMember();
  }

  loadMember() {
    this.memberService.getMember(this.user.username).subscribe({
      next: member => this.member = member
    })
  }

  updateEmail() {
    console.log(this.member);
    this.toastr.success('Email updated');
    this.editEmail.reset(this.member);
  }

  updatePassword() {
    console.log(this.member);
    this.toastr.success('Password updated');
    this.editPassword.reset(this.member);
  }
}
