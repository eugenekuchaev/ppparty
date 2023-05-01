export interface EventNotification {
    id: number;
    timeStamp: Date;
    notificationMessage: string;
    eventId: number;
    eventName: string;
    read: boolean;
  }