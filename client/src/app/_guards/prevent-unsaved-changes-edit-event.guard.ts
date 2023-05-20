import { Injectable } from '@angular/core';
import { CanDeactivate } from '@angular/router';
import { EditEventComponent } from '../events/edit-event/edit-event.component';

@Injectable({
  providedIn: 'root'
})
export class PreventUnsavedChangesEditEventGuard implements CanDeactivate<unknown> {
  canDeactivate(component: EditEventComponent): boolean {
    if (component.eventEditForm.dirty || component.editTagsForm.dirty) {
      return confirm('Are you sure? Any unnsaved changes will be lost.')
    }
    return true; 
  }
}
