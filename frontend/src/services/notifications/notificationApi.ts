import { axiosInstance } from "@/config/axiosInstance";
import {
  IResponseMarkAsRead,
  IResponseNotification,
} from "@/types/notifications";

export const getNotifications = async (
  type: string = "all"
): Promise<IResponseNotification> => {
  const res = await axiosInstance.get(`/v1/notifications?type=${type}`);
  return res.data;
};

export const maskNotificationAsRead = async (
  notificationId: string
): Promise<IResponseMarkAsRead> => {
  const res = await axiosInstance.put(
    `/v1/notifications/${notificationId}/read`
  );
  return res.data;
};
