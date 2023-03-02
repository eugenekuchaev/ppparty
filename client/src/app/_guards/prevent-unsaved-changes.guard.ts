import { Injectable } from '@angular/core';
import { CanDeactivate } from '@angular/router';
import { MemberEditComponent } from '../members/member-edit/member-edit.component';

@Injectable({
  providedIn: 'root'
})
export class PreventUnsavedChangesGuard implements CanDeactivate<unknown> {
  canDeactivate(component: MemberEditComponent): boolean {
    if (component.editNameAndUsernameForm.dirty || component.editLocationForm.dirty || component.editInterestsForm.dirty) {
      return confirm('Are you sure? Any unnsaved changes will be lost.')
    }
    return true;
  }
}
