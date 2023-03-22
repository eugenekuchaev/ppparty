import { UserInterest } from "./userInterest"
import { UserPhoto } from "./userPhoto"

export interface Member {
    id: number;
    fullName: string;
    username: string;
    email: string;
    country: string;
    region: string;
    city: string;
    about: string;
    userPhotoUrl: string;
    phoneNumber: string;
    facebookLink: string;
    instagramLink: string;
    twitterLink: string;
    linkedInLink: string;
    websiteLink: string;
    rating: number;
    created: Date;
    lastActive: Date;
    userPhoto: UserPhoto;
    userInterests: UserInterest[];
  }
  
