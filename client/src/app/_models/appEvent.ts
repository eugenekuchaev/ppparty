import { EventDate } from "./eventDate"
import { EventPhoto } from "./eventPhoto"
import { EventTag } from "./eventTag"

export interface AppEvent {
    id: number;
    eventName: string;
    description: string;
    eventPhotoUrl: string;
    isEnded: boolean;
    country: string;
    region: string;
    city: string;
    address: string;
    currency: string;
    price: number;
    eventTags: EventTag[];
    eventTagsString: String;
    eventOwnerUsername: string;
    eventDates: EventDate[];
    eventPhoto: EventPhoto;
  }