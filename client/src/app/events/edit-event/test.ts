// export class EditEventComponent {
//   @ViewChild('editTagsForm') editTagsForm: NgForm;
//   eventEditForm: FormGroup;
//   validationErrors: string[] = [];
//   appEvent: AppEvent;
//   eventTags: EventTag[] = [];
//   tags: string;

//   constructor(private eventsService: EventsService, private fb: FormBuilder, private route: ActivatedRoute,
//     private toastr: ToastrService) { }

//   ngOnInit(): void {
//     this.loadEvent();
//     this.initializeForm();
//   }

//   initializeForm() {
//     this.eventEditForm = this.fb.group({
//       eventName: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(64)]],
//       description: ['', [Validators.required, Validators.minLength(100), Validators.maxLength(1000)]],
//       country: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(56)]],
//       region: ['', [Validators.required, Validators.minLength(1), Validators.maxLength(35)]],
//       city: ['', [Validators.required, Validators.minLength(1), Validators.maxLength(58)]],
//       address: ['', [Validators.required, Validators.minLength(1), Validators.maxLength(100)]],
//       eventDates: this.fb.array([
//         this.fb.group({
//           startDate: ['', [Validators.required]],
//           endDate: ['', [Validators.required]]
//         })
//       ])
//     });
//   }

//   addEventDate() {
//     const eventDateGroup = this.fb.group({
//       startDate: ['', [Validators.required]],
//       endDate: ['', [Validators.required]]
//     });
//     this.eventDates.push(eventDateGroup);
//   }

//   removeEventDate(i: number) {
//     if (i > 0) {
//       this.eventDates.removeAt(i);
//     }
//   }

//   get eventDates() {
//     return this.eventEditForm.get('eventDates') as FormArray;
//   }

//   loadEvent() {
//     this.eventsService.getEvent(this.route.snapshot.paramMap.get('eventId')).subscribe({
//       next: appEvent => {
//         this.appEvent = appEvent;
//         this.eventTags = this.appEvent.eventTags;
//         this.eventEditForm.patchValue({
//           eventName: this.appEvent.eventName,
//           description: this.appEvent.description,
//           country: this.appEvent.country,
//           region: this.appEvent.region,
//           city: this.appEvent.city,
//           address: this.appEvent.address
//         });

//         this.eventDates.clear();

//         this.appEvent.eventDates.forEach(eventDate => {
//           const eventDateGroup = this.fb.group({
//             startDate: [eventDate.startDate, [Validators.required]],
//             endDate: [eventDate.endDate, [Validators.required]]
//           });
//           this.eventDates.push(eventDateGroup);
//         });
//       }
//     })
//   }
// }

// <div class="row">
//     <div class="container col-6">
//         <form [formGroup]="eventEditForm" (ngSubmit)="eventEditForm.valid && updateEvent()" autocomplete="off">
//             <h2 class="text-center">Update event</h2>
//             <hr>
//             <app-text-input-events [formControl]='eventEditForm.controls["eventName"]' label='Event name'>
//             </app-text-input-events>
//             <app-text-input-events [formControl]='eventEditForm.controls["description"]' label='Description'>
//             </app-text-input-events>
//             <app-text-input-events [formControl]='eventEditForm.controls["country"]' label='Country'>
//             </app-text-input-events>
//             <app-text-input-events [formControl]='eventEditForm.controls["region"]' label='Region'>
//             </app-text-input-events>
//             <app-text-input-events [formControl]='eventEditForm.controls["city"]' label='City'>
//             </app-text-input-events>
//             <app-text-input-events [formControl]='eventEditForm.controls["address"]' label='Address'>
//             </app-text-input-events>
            
//             <h4 class="text-center mt-4 mb-3">Dates</h4>
//             <div formArrayName="eventDates">
//                 <div *ngFor="let eventDate of eventDates.controls; let i=index" [formGroupName]="i">
//                     <div class="row">
//                         <div class="col text-center">
//                             <app-text-input-events [formControlName]="'startDate'" label="Start date"></app-text-input-events>
//                         </div>
//                         <div class="col text-center">
//                             <app-text-input-events [formControlName]="'endDate'" label="End date"></app-text-input-events>
//                         </div>
//                         <div class="col-auto">
//                             <button type="button" class="btn btn-danger" (click)="removeEventDate(i)">
//                                 <i class="fa fa-times" aria-hidden="true"></i>
//                             </button>
//                         </div>
//                     </div>
//                 </div>
//             </div>
//         </form>
//     </div>
// </div>

// <input *ngSwitchCase="'Start date'"
//         [class.is-invalid]="ngControl.touched && ngControl.invalid"
//         type="datetime-local"
//         class="form-control"
//         [formControl]="ngControl.control"
//         timezone="UTC">

//       <input *ngSwitchCase="'End date'"
//         [class.is-invalid]="ngControl.touched && ngControl.invalid"
//         type="datetime-local"
//         class="form-control"
//         [formControl]="ngControl.control"
//         timezone="UTC">


