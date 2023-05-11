import { Component, ViewChild } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, NgForm, ValidatorFn, Validators } from '@angular/forms';
import { Router } from '@angular/router';
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
  editEmailForm: FormGroup;
  emailPattern = '^[a-z0-9._%+-]+@[a-z0-9.-]+\\.[a-z]{2,7}$';
  emailValidationErrors: string[] = [];
  changePasswordForm: FormGroup;
  passwordValidationErrors: string[] = [];
  member: Member;
  user: User;

  constructor(private memberService: MembersService, private accountService: AccountService,
    private toastr: ToastrService, private fb: FormBuilder, private router: Router) {
    this.accountService.currentUser$.pipe(take(1)).subscribe({
      next: user => this.user = user
    })
  }

  ngOnInit(): void {
    this.loadMember();
    this.initializePasswordForm();
  }
  
  initializePasswordForm() {
    this.changePasswordForm = this.fb.group({
      oldPassword: ['', [Validators.required, Validators.minLength(8), Validators.maxLength(64)]],
      newPassword: ['', [Validators.required, Validators.minLength(8), Validators.maxLength(64)]],
      confirmNewPassword: ['', [Validators.required, this.matchValues('newPassword')]]
    })
    this.changePasswordForm.controls.newPassword.valueChanges.subscribe({
      next: () => {
        this.changePasswordForm.controls.confirmNewPassword.updateValueAndValidity();
      }
    })
  }

  loadMember() {
    this.memberService.getMember(this.user.username).subscribe({
      next: member => {
        this.member = member;
        this.editEmailForm = this.fb.group({
          email: [this.member.email, [Validators.required, Validators.pattern(this.emailPattern)]]
        });
      }
    });
  }
  

  updateEmail() {
    this.accountService.updateEmail(this.editEmailForm.value).subscribe({
      next: response => {
        this.toastr.success("Email updated");
      },
      error: error => {
        this.emailValidationErrors = error;
      }
    })
  }

  changePassword() {
    this.accountService.changePassword(this.changePasswordForm.value).subscribe({
      next: response => {
        this.toastr.success("Password updated");
        this.logout();
      },
      error: error => {
        this.emailValidationErrors = error;
      }
    })
  }

  logout() {
    this.accountService.logout();
    this.router.navigateByUrl('/');
  }

  matchValues(matchTo: string): ValidatorFn {
    return (control: AbstractControl) => {
      return control?.value === control?.parent?.controls[matchTo].value ? null : { isMatching: true }
    }
  }
}
