import { Component, HostListener, OnInit, ViewChild } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, NgForm, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { AppEvent } from 'src/app/_models/appEvent';
import { EventTag } from 'src/app/_models/eventTag';
import { EventsService } from 'src/app/_services/events.service';

@Component({
  selector: 'app-edit-event',
  templateUrl: './edit-event.component.html',
  styleUrls: ['./edit-event.component.css']
})
export class EditEventComponent implements OnInit {
  @ViewChild('editTagsForm') editTagsForm: NgForm;
  eventEditForm: FormGroup;
  validationErrors: string[] = [];
  appEvent: AppEvent;
  eventTags: EventTag[] = [];
  tags: string;
  eventId: string;
  initialFormValues: any;
  @HostListener('window:beforeunload', ['$event']) unloadNotification($event: any) {
    if (this.eventEditForm.dirty || this.editTagsForm.value !== null) {
      $event.returnValue = true;
    }
  }

  constructor(private eventsService: EventsService, private fb: FormBuilder, private route: ActivatedRoute,
    private toastr: ToastrService) {
    this.eventId = this.route.snapshot.paramMap.get('eventId');
  }

  ngOnInit(): void {
    this.loadEvent();
    this.initializeForm();
  }

  initializeForm() {
    this.eventEditForm = this.fb.group({
      eventName: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(64)]],
      description: ['', [Validators.required, Validators.minLength(100), Validators.maxLength(1000)]],
      country: ['', [Validators.required, Validators.minLength(2), Validators.maxLength(56)]],
      region: ['', [Validators.required, Validators.minLength(1), Validators.maxLength(35)]],
      city: ['', [Validators.required, Validators.minLength(1), Validators.maxLength(58)]],
      address: ['', [Validators.required, Validators.minLength(1), Validators.maxLength(100)]],
      eventDates: this.fb.array([
        this.fb.group({
          startDate: ['', [Validators.required]],
          endDate: ['', [Validators.required]]
        })
      ])
    });
  }

  resetForm() {
    this.eventEditForm.setValue(this.initialFormValues);
    this.eventEditForm.markAsPristine();
    this.validationErrors = [];
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
      this.eventEditForm.markAsDirty();
    }
  }

  get eventDates() {
    return this.eventEditForm.get('eventDates') as FormArray;
  }

  loadEvent() {
    this.eventsService.getEvent(this.route.snapshot.paramMap.get('eventId')).subscribe({
      next: appEvent => {
        this.appEvent = appEvent;
        this.eventTags = this.appEvent.eventTags;
        this.eventEditForm.patchValue({
          eventName: this.appEvent.eventName,
          description: this.appEvent.description,
          country: this.appEvent.country,
          region: this.appEvent.region,
          city: this.appEvent.city,
          address: this.appEvent.address
        });

        while (this.eventDates.length) {
          this.eventDates.removeAt(0);
        }

        this.appEvent.eventDates.forEach(eventDate => {
          const eventDateGroup = this.fb.group({
            startDate: [new Date(eventDate.startDate).toISOString().slice(0, 16), [Validators.required]],
            endDate: [new Date(eventDate.endDate).toISOString().slice(0, 16), [Validators.required]]
          });
          this.eventDates.push(eventDateGroup);
        });

        this.initialFormValues = this.eventEditForm.value;
      }
    })
  }

  updateEvent() {
    this.eventsService.updateEvent(this.eventEditForm.value, this.appEvent.id).subscribe({
      next: () => {
        this.toastr.success('Event updated');
        this.initialFormValues = this.eventEditForm.value;
        this.eventEditForm.markAsPristine();
      }
    })
  }

  addTags(tags: string) {
    this.eventsService.addTags(tags, this.appEvent.id).subscribe({
      next: () => {
        const tagsArray: string[] = tags.split(',').map((tag: string) => 
          tag.replace(/(^-+)|(-+$)|[^a-zA-Z0-9-]/g, '').replace(/(-){2,}/g, '-').trim());
        tagsArray.forEach((tag: string) => {
          if (tag !== "" && this.eventTags.find(x => x.eventTagName === tag) == null) {
            const newTag: EventTag = {
              id: 0,
              eventTagName: tag
            };
            this.eventTags.push(newTag);
            this.editTagsForm.reset();
            this.toastr.success('Tags added');
          }
        })
      }
    })
  }

  deleteTag(tag: string) {
    this.eventsService.removeTag(tag, this.appEvent.id).subscribe({
      next: () => {
        this.toastr.success('Tag deleted');
        this.eventTags = this.eventTags.filter(ui => ui.eventTagName !== tag);
      }
    })
  }
}
