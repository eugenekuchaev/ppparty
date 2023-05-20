import { Injectable } from '@angular/core';
import { CanDeactivate } from '@angular/router';
import { CreateEventComponent } from '../events/create-event/create-event.component';

@Injectable({
  providedIn: 'root'
})
export class PreventUnsavedChangesEventCreationGuard implements CanDeactivate<unknown> {
  canDeactivate(component: CreateEventComponent): boolean {
    if (component.eventCreationForm.dirty) {
      return confirm('Are you sure? Any unnsaved changes will be lost.')
    }
    return true;
  }
}
