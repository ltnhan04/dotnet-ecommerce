import { useQuery, useQueryClient, useMutation } from "@tanstack/react-query";
import {
  getNotifications,
  maskNotificationAsRead,
} from "@/services/notifications/notificationApi";
import {
  IResponseNotification,
  IResponseMarkAsRead,
} from "@/types/notifications";

export const useNotifications = (type: string = "all") => {
  return useQuery<IResponseNotification>({
    queryKey: ["notifications", type],
    queryFn: () => getNotifications(type),
    staleTime: 1000 * 30,
    refetchInterval: 1000 * 30,
    retry: 1,
  });
};

export const useMarkNotificationAsRead = () => {
  const queryClient = useQueryClient();

  return useMutation<IResponseMarkAsRead, unknown, string>({
    mutationFn: (notificationId) => maskNotificationAsRead(notificationId),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["notifications"] });
    },
  });
};
