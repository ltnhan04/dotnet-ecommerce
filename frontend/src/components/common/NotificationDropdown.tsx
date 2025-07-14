import React, { useState, useMemo } from "react";
import { Bell, Filter } from "lucide-react";
import {
  DropdownMenu,
  DropdownMenuTrigger,
  DropdownMenuContent,
  DropdownMenuItem,
} from "@/components/ui/dropdown-menu";
import { Badge } from "@/components/ui/badge";
import { ScrollArea } from "@/components/ui/scroll-area";
import { Button } from "@/components/ui/button";
import {
  useNotifications,
  useMarkNotificationAsRead,
} from "@/hooks/useNotifications";
import { INotification } from "@/types/notifications";
import { ITEMS_PER_PAGE, NOTIFICATION_TYPES } from "@/constants/page";
import { getTimeAgo } from "@/utils/time-ago";

export const NotificationDropdown = () => {
  const [type, setType] = useState("all");
  const [page, setPage] = useState(1);
  const { data, isLoading, error } = useNotifications(type);
  const { mutate: markAsRead } = useMarkNotificationAsRead();
  const notifications = data?.data || [];
  const total = notifications.length;
  const totalPages = Math.max(1, Math.ceil(total / ITEMS_PER_PAGE));
  const paginatedNotifications = useMemo(() => {
    const start = (page - 1) * ITEMS_PER_PAGE;
    return notifications.slice(start, start + ITEMS_PER_PAGE);
  }, [notifications, page]);
  const unreadCount = notifications.filter((n) => !n.isRead).length;
  const handleNotificationClick = (notification: INotification) => {
    if (!notification.isRead) markAsRead(notification._id);
    if (notification.redirectUrl) {
      if (notification.redirectUrl.startsWith("/")) {
        window.location.href = notification.redirectUrl;
      } else if (notification.redirectUrl.startsWith("http")) {
        window.open(notification.redirectUrl, "_blank");
      } else {
        window.location.href = notification.redirectUrl;
      }
    }
  };
  const currentType =
    NOTIFICATION_TYPES.find((t) => t.value === type)?.label || "Tất cả";
  return (
    <DropdownMenu>
      <DropdownMenuTrigger asChild>
        <button className="relative flex items-center justify-center text-white focus:outline-none">
          <Bell className="w-5 h-5 sm:w-6 sm:h-6" />
          {unreadCount > 0 && (
            <span className="absolute -top-2 -right-2 bg-red-500 text-white text-[11px] font-semibold rounded-full w-4 h-4 flex items-center justify-center">
              {unreadCount > 99 ? "99+" : unreadCount}
            </span>
          )}
        </button>
      </DropdownMenuTrigger>
      <DropdownMenuContent align="end" className="w-[380px] p-0 rounded-lg max-h-[400px] flex flex-col">
        <div className="px-4 pt-3 pb-1 border-b flex items-center justify-between gap-2">
          <span className="font-medium text-base">Thông báo</span>
          <DropdownMenu>
            <DropdownMenuTrigger asChild>
              <Button
                variant="outline"
                size="sm"
                className="flex items-center gap-1 px-2 py-1 h-7 text-xs"
              >
                <Filter className="w-4 h-4 mr-1" />
                <span className="truncate max-w-[80px]">{currentType}</span>
              </Button>
            </DropdownMenuTrigger>
            <DropdownMenuContent align="end" className="w-36">
              {NOTIFICATION_TYPES.map((t) => (
                <DropdownMenuItem
                  key={t.value}
                  onClick={() => {
                    setType(t.value);
                    setPage(1);
                  }}
                  className={type === t.value ? "font-semibold bg-muted" : ""}
                >
                  {t.label}
                </DropdownMenuItem>
              ))}
            </DropdownMenuContent>
          </DropdownMenu>
        </div>
        <ScrollArea className="flex-1 min-h-0 px-0 overflow-auto">
          {isLoading ? (
            <div className="py-8 text-muted-foreground text-center text-sm">
              Đang tải...
            </div>
          ) : error ? (
            <div className="py-8 text-destructive text-center text-sm">
              Lỗi tải thông báo
            </div>
          ) : paginatedNotifications.length === 0 ? (
            <div className="py-8 text-muted-foreground text-center text-sm">
              Không có thông báo
            </div>
          ) : (
            paginatedNotifications.map((notification) => {
              const hasRedirect =
                notification.redirectUrl &&
                notification.redirectUrl.trim() !== "";
              const truncatedMessage =
                notification.message.length > 100
                  ? notification.message.substring(0, 100) + "..."
                  : notification.message;
              return (
                <div
                  key={notification._id}
                  className={`flex gap-3 px-4 py-2 border-b last:border-b-0 transition-colors items-start ${
                    !notification.isRead ? "bg-blue-50/60" : ""
                  } ${
                    hasRedirect
                      ? "cursor-pointer hover:bg-muted"
                      : "cursor-default"
                  }`}
                  onClick={() =>
                    hasRedirect && handleNotificationClick(notification)
                  }
                  tabIndex={hasRedirect ? 0 : -1}
                  aria-label={notification.title}
                >
                  <span
                    className={`w-2 h-2 rounded-full mt-1 border ${
                      !notification.isRead ? "bg-blue-500" : "bg-gray-200"
                    }`}
                    aria-label={notification.isRead ? "Read" : "Unread"}
                  ></span>
                  <div className="flex-1 min-w-0">
                    <div className="flex items-center justify-between gap-2">
                      <span className="text-sm font-medium truncate text-foreground max-w-[180px]">
                        {notification.title}
                      </span>
                      {!notification.isRead && (
                        <Button
                          variant="link"
                          size="sm"
                          className="ml-2 px-0 text-xs min-w-fit"
                          onClick={(e) => {
                            e.stopPropagation();
                            markAsRead(notification._id);
                          }}
                          aria-label="Mark as read"
                        >
                          Đã đọc
                        </Button>
                      )}
                    </div>
                    <div className="text-xs text-muted-foreground mt-0.5 line-clamp-2">
                      {truncatedMessage}
                    </div>
                    <div className="flex items-center justify-between mt-1">
                      <span className="text-xs text-gray-400">
                        {getTimeAgo(notification.createdAt)}
                      </span>
                      <Badge
                        variant="secondary"
                        className="text-xs px-2 py-0.5 capitalize min-w-fit"
                      >
                        {notification.type}
                      </Badge>
                    </div>
                  </div>
                </div>
              );
            })
          )}
        </ScrollArea>
        {totalPages > 1 && (
          <div className="flex items-center justify-between px-4 py-2 border-t bg-background text-xs gap-2">
            <Button
              variant="ghost"
              size="sm"
              disabled={page === 1}
              onClick={() => setPage((p) => Math.max(1, p - 1))}
              aria-label="Previous page"
              className="px-2"
            >
              Previous
            </Button>
            <span className="mx-2">
              Page {page} of {totalPages}
            </span>
            <Button
              variant="ghost"
              size="sm"
              disabled={page === totalPages}
              onClick={() => setPage((p) => Math.min(totalPages, p + 1))}
              aria-label="Next page"
              className="px-2"
            >
              Next
            </Button>
          </div>
        )}
      </DropdownMenuContent>
    </DropdownMenu>
  );
};

export default NotificationDropdown;
