import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { ToastrModule } from 'ngx-toastr';
import { FileUploadModule } from 'ng2-file-upload';
import { PaginationModule } from 'ngx-bootstrap/pagination'
import { TabsModule } from 'ngx-bootstrap/tabs'
import { ModalModule } from 'ngx-bootstrap/modal'

@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    BsDropdownModule.forRoot(),
    ToastrModule.forRoot({
      positionClass: 'toast-bottom-right'
    }),
    FileUploadModule,
    PaginationModule.forRoot(),
    TabsModule.forRoot(),
    ModalModule.forRoot()
  ],
  exports: [
    BsDropdownModule,
    ToastrModule,
    FileUploadModule,
    PaginationModule,
    TabsModule, 
    ModalModule
  ]
})
export class SharedModule { }
