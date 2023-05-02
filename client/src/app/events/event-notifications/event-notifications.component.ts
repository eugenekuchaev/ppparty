import { Component, Input, OnInit } from '@angular/core';
import { EventNotification } from 'src/app/_models/eventNotification';
import { EventsService } from 'src/app/_services/events.service';

@Component({
  selector: 'app-event-notifications',
  templateUrl: './event-notifications.component.html',
  styleUrls: ['./event-notifications.component.css']
})
export class EventNotificationsComponent implements OnInit{
  @Input() notification: EventNotification;

  constructor(private eventsService: EventsService) {}

  ngOnInit(): void {
    this.readEventNotification();
  }

  readEventNotification() {
    this.eventsService.readEventNotification(this.notification.id).subscribe({
      next: () => {
      }
    })
  }
}
