export interface Conversation {
    friendFullName: string;
    friendUsername: string;
    friendPhotoUrl: string;
    lastMessageAuthorName: string;
    lastMessageContent: string;
    lastMessageSent: Date;
    lastMessageRead?: Date;
  }
  