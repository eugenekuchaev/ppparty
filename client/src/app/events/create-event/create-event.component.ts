import { Component, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NavigationExtras, Router } from '@angular/router';
import { AppEvent } from 'src/app/_models/appEvent';
import { EventsService } from 'src/app/_services/events.service';

@Component({
  selector: 'app-create-event',
  templateUrl: './create-event.component.html',
  styleUrls: ['./create-event.component.css']
})
export class CreateEventComponent implements OnInit {
  eventCreationForm: FormGroup;
  validationErrors: string[] = [];

  constructor(private eventsService: EventsService, private fb: FormBuilder, private router: Router) { }

  ngOnInit(): void {
    this.initializeForm();
  }

  initializeForm() {
    this.eventCreationForm = this.fb.group({
      eventName: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(64)]],
      description: ['', [Validators.required, Validators.minLength(100), Validators.maxLength(1000)]],
      country: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(56)]],
      region: ['', [Validators.required, Validators.minLength(1), Validators.maxLength(35)]],
      city: ['', [Validators.required, Validators.minLength(1), Validators.maxLength(58)]],
      address: ['', [Validators.required, Validators.minLength(1), Validators.maxLength(100)]],
      eventTagsString: ['', [Validators.required, Validators.minLength(1), Validators.maxLength(300)]],
      eventDates: this.fb.array([
        this.fb.group({
          startDate: ['', [Validators.required]],
          endDate: ['', [Validators.required]]
        })
      ])
    });
  }
  
  addEventDate() {
    const eventDateGroup = this.fb.group({
      startDate: ['', [Validators.required]],
      endDate: ['', [Validators.required]]
    });
    this.eventDates.push(eventDateGroup);
  }

  removeEventDate(i: number) {
    if (i > 0) {
      this.eventDates.removeAt(i);
    }
  }
  
  get eventDates() {
    return this.eventCreationForm.get('eventDates') as FormArray;
  }

  createEvent() {
    this.eventsService.createEvent(this.eventCreationForm.value).subscribe({
      next: (response: AppEvent) => {
        const navigationExtras: NavigationExtras = {
          state: {
            appEvent: response,
            param: 'eventphoto'
          }
        };
        this.router.navigate(['/photoeditor'], navigationExtras);
      },
      error: error => {
        this.validationErrors = error;
      }
    });
  }
  
}