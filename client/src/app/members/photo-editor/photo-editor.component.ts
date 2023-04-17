import { Component, Input, OnInit } from '@angular/core';
import { FileUploader } from 'ng2-file-upload';
import { ToastrService } from 'ngx-toastr';
import { take } from 'rxjs';
import { AppEvent } from 'src/app/_models/appEvent';
import { Member } from 'src/app/_models/member';
import { User } from 'src/app/_models/user';
import { AccountService } from 'src/app/_services/account.service';
import { environment } from 'src/environments/environment.development';

@Component({
  selector: 'app-photo-editor',
  templateUrl: './photo-editor.component.html',
  styleUrls: ['./photo-editor.component.css']
})
export class PhotoEditorComponent implements OnInit {
  @Input() member?: Member;
  @Input() appEvent?: AppEvent;
  @Input() param?: String;
  uploader: FileUploader;
  hasBaseDropZoneOver = false;
  baseUrl = environment.apiUrl;
  user: User;

  constructor(private accountService: AccountService, private toastr: ToastrService) {
    this.accountService.currentUser$.pipe(take(1)).subscribe(user => this.user = user);
  }

  ngOnInit(): void {
    this.initializeUploader();
  }

  fileOverBase(e: any) {
    this.hasBaseDropZoneOver = e;
  }

  initializeUploader() {
    if (this.param === "userphoto") {
      this.uploader = new FileUploader({
        url: this.baseUrl + 'users/add-photo',
        authToken: 'Bearer ' + this.user.token,
        isHTML5: true,
        allowedFileType: ['image'],
        removeAfterUpload: true,
        autoUpload: false,
        maxFileSize: 10 * 1024 * 1024
      });
    }
    if (this.param === "eventphoto") {
      this.uploader = new FileUploader({
        url: this.baseUrl + 'events/add-photo/' + this.appEvent?.id,
        authToken: 'Bearer ' + this.user.token,
        isHTML5: true,
        allowedFileType: ['image'],
        removeAfterUpload: true,
        autoUpload: false,
        maxFileSize: 10 * 1920 * 1080
      });
    }

    this.uploader.onAfterAddingFile = (file) => {
      file.withCredentials = false;
    }

    this.uploader.onSuccessItem = (item, response, status, headers) => {
      if (response) {
        const photo = JSON.parse(response);

        if (this.param === "userphoto") {
          if (this.member?.userPhoto) {
            this.member.userPhoto = photo;
          }
          if (this.user?.photoUrl) {
            this.user.photoUrl = photo.photoUrl;
            this.accountService.setCurrentUser(this.user);
          }
          this.accountService.setCurrentUser(this.user);
        }

        if (this.param === "eventphoto") {
          if (this.appEvent?.eventPhoto) {
            this.appEvent.eventPhoto = photo;
          }
          if (this.appEvent?.eventPhotoUrl) {
            this.appEvent.eventPhotoUrl = photo.photoUrl;
          }
        }

        this.toastr.success("Photo uploaded");
      }
    }
  }
}
