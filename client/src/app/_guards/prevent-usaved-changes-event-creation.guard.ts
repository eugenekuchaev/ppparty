import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanDeactivate, RouterStateSnapshot } from '@angular/router';
import { CreateEventComponent } from '../events/create-event/create-event.component';

@Injectable({
  providedIn: 'root'
})
export class PreventUnsavedChangesEventCreationGuard implements CanDeactivate<unknown> {
  canDeactivate(component: unknown, currentRoute: ActivatedRouteSnapshot, currentState: RouterStateSnapshot, nextState?: RouterStateSnapshot): boolean {
    if (component instanceof CreateEventComponent) {
      const createEventComponent = component as CreateEventComponent;
      if (createEventComponent.eventCreationForm.dirty && nextState && nextState.url !== '/photoeditor') {
        return confirm('Are you sure? Any unsaved changes will be lost.');
      }
    }
    return true;
  }
}
