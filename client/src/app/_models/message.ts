export interface Message {
    id: number;
    senderId: number;
    senderUsername: string;
    senderFullName: string;
    senderPhotoUrl: string;
    recipientId: number;
    recipientUsername: string;
    recipientFullName: string;
    recipientPhotoUrl: string;
    content: string;
    dateRead?: Date;
    messageSent: Date;
  }