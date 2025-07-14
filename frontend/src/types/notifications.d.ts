export interface IResponseMarkAsRead {
  message: string;
  data: string | null;
}

export interface IResponseNotification {
  message: string;
  data: INotification[];
}

export interface INotification {
  _id: string;
  userId: string;
  targetRole: string;
  type: string;
  title: string;
  message: string;
  isRead: boolean;
  createdAt: string;
  updatedAt: string;
  redirectUrl: string;
}
