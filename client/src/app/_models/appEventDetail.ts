import { EventDate } from "./eventDate";
import { EventPhoto } from "./eventPhoto";
import { EventTag } from "./eventTag";
import { MemberInEvent } from "./memberInEvent";

export interface AppEventDetail {
    id: number;
    eventName: string;
    description: string;
    eventPhotoUrl: string;
    isEnded: boolean;
    isCancelled: boolean;
    country: string;
    region: string;
    city: string;
    address: string;
    currency: string;
    price: number;
    eventPhoto: EventPhoto;
    eventTags: EventTag[];
    eventOwner: MemberInEvent;
    participants: MemberInEvent[];
    friendsParticipants: MemberInEvent[];
    eventDates: EventDate[];
  }