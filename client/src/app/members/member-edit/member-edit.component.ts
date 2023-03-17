import { Component, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { take } from 'rxjs';
import { Member } from 'src/app/_models/member';
import { User } from 'src/app/_models/user';
import { UserInterest } from 'src/app/_models/userInterest';
import { AccountService } from 'src/app/_services/account.service';
import { MembersService } from 'src/app/_services/members.service';

@Component({
  selector: 'app-member-edit',
  templateUrl: './member-edit.component.html',
  styleUrls: ['./member-edit.component.css']
})
export class MemberEditComponent implements OnInit {
  @ViewChild('editNameForm') editNameForm: NgForm;
  @ViewChild('editLocationForm') editLocationForm: NgForm;
  @ViewChild('editAboutForm') editAboutForm: NgForm;
  @ViewChild('editInterestsForm') editInterestsForm: NgForm;
  @ViewChild('editContactsForm') editContactsForm: NgForm;
  member: Member;
  user: User;
  userInterests: UserInterest[] = [];
  generalIsActivated = true;
  contactsIsActivated = false;
  changePhotoIsActivated = false;

  constructor(private memberService: MembersService, private accountService: AccountService,
    private toastr: ToastrService) {
      this.accountService.currentUser$.pipe(take(1)).subscribe({
        next: user => {
          this.user = user;
        }
      })
  }

  ngOnInit(): void {
    this.loadMember();
  }

  loadMember() {
    this.memberService.getMember(this.user.username).subscribe({
      next: member => {
        this.member = member;
        this.userInterests = this.member.userInterests;
      }
    })
  }

  updateName() {
    this.memberService.updateName(this.member).subscribe({
      next: () => {
        this.toastr.success('Name updated');
        this.editNameForm.reset(this.member);
      },
      error: error => {
        this.toastr.error("Full name must be from 3 to 32 characters");
      }
    })
  }

  updateLocation() {
    this.memberService.updateLocation(this.member).subscribe({
      next: () => {
        this.toastr.success('Location updated');
        this.editLocationForm.reset(this.member);
      },
      error: error => {
        this.toastr.error("One of the fields is too long")
      }
    })
  }

  updateAbout() {
    this.memberService.updateAbout(this.member).subscribe({
      next: () => {
        this.toastr.success('About updated');
        this.editAboutForm.reset(this.member);
      },
      error: error => {
        this.toastr.error("Your about is too long")
      }
    })
  }

  deleteInterest(interest: string) {
    this.memberService.deleteInterest(interest).subscribe({
      next: () => {
        this.toastr.success('Interest deleted');
        this.userInterests = this.userInterests.filter(ui => ui.interestName !== interest);
      }
    })
  }

  addInterests(interests: string) {
    this.memberService.addInterests(interests).subscribe({
      next: () => {
        this.toastr.success('Interests added');
        this.editInterestsForm.reset(this.member);

        // This array of interests is temporary, 
        // the property userInterests is reloaded from the API each time a user reloads a page 
        // Id is set to 0 because it's not used in any API requests
        const interestsArray: string[] = interests.split(',').map((interest: string) => interest.trim());
        interestsArray.forEach((interest: string) => {
          if (interest !== "" && this.userInterests.find(x => x.interestName === interest) == null) {
            const newInterest: UserInterest = {
              id: 0,
              interestName: interest
            };
            this.userInterests.push(newInterest);
          }
        })
      }
    })
  }

  updateContacts() {
    this.memberService.updateContacts(this.member).subscribe({
      next: () => {
        this.toastr.success('Contacts updated');
        this.editContactsForm.reset(this.member);
      },
      error: error => {
        this.toastr.error("One of the fields is too long")
      }
    })
  }

  clickOnGeneral() {
    this.generalIsActivated = true;
    this.contactsIsActivated = false;
    this.changePhotoIsActivated = false;
  }

  clickOnContacts() {
    this.generalIsActivated = false;
    this.contactsIsActivated = true;
    this.changePhotoIsActivated = false;
  }

  clickOnChangePhoto() {
    this.generalIsActivated = false;
    this.contactsIsActivated = false;
    this.changePhotoIsActivated = true;
  }
}
