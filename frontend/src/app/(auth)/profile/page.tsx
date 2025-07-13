/* eslint-disable @typescript-eslint/no-explicit-any */
"use client";

import { useEffect, useState } from "react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import {
  Card,
  CardContent,
  CardDescription,
  CardFooter,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import { Label } from "@/components/ui/label";

import { useToast } from "@/hooks/use-toast";
import { useProfile } from "@/hooks/useProfile";
import { validatePhoneNumber } from "@/utils/validate-phoneNumber";
import { Avatar, AvatarFallback, AvatarImage } from "@/components/ui/avatar";
import AddressSection from "@/app/(auth)/profile/components/address";
import { EditedProfile } from "@/types/profile";
import { ProfileType } from "@/types/auth";

const UserProfile = () => {
  const { toast } = useToast();
  const { profile, isLoading, error, updateProfile } = useProfile();
  const [editedProfile, setEditedProfile] = useState<EditedProfile>({
    isEdited: false,
    editedName: "",
    editedAddress: {
      street: "",
      ward: "",
      district: "",
      city: "",
      country: "Vietnam",
    },
    editedPhoneNumber: "",
  });

  useEffect(() => {
    if (profile) {
      setEditedProfile({
        isEdited: false,
        editedName: profile.name || "",
        editedAddress: {
          street: profile.address?.street || "",
          ward: profile.address?.ward || "",
          district: profile.address?.district || "",
          city: profile.address?.city || "",
          country: profile.address?.country || "Vietnam",
        },
        editedPhoneNumber: profile.phoneNumber || "",
      });
    }
  }, [profile]);

  const handleUpdateProfile = async () => {
    if (!validatePhoneNumber(editedProfile.editedPhoneNumber)) {
      toast({
        title: "Đã xảy ra lỗi!",
        description: "Số điện thoại không hợp lệ!",
        variant: "destructive",
      });
      return;
    }

    setEditedProfile((prev) => ({ ...prev, isEdited: true }));
    try {
      const res = await updateProfile({
        name: editedProfile.editedName,
        phoneNumber: editedProfile.editedPhoneNumber,
        address: editedProfile.editedAddress,
      });
      if (res.status === 200) {
        toast({
          title: "Thay đổi thành công!",
          description: res.data.message,
          variant: "default",
        });
      }
      setEditedProfile((prev) => ({ ...prev, isEdited: false }));
    } catch (error: any) {
      toast({
        title: "Đã xảy ra lỗi!",
        description: error.response?.data?.message || "Something went wrong!",
        variant: "destructive",
      });
    }
  };

  if (isLoading) {
    return <div>Loading...</div>;
  }

  if (error) {
    return <div>Error: {error.message}</div>;
  }

  return (
    <div className="min-h-screen py-8 px-4 sm:px-6 lg:px-8">
      <Card className="w-full max-w-5xl mx-auto shadow-2xl rounded-[8px] overflow-hidden border transition-all duration-300 hover:shadow-3xl">
        <CardHeader className="p-8 border-b">
          <p className="text-2xl font-medium text-blue-800 pb-6">Hồ sơ của tôi </p>
          <div className="flex items-center space-x-6">

            <Avatar className="size-24 shadow-xl transition-transform duration-300 hover:scale-105">
              <AvatarImage
                src={`https://api.dicebear.com/6.x/initials/svg?seed=${profile?.name}`}
                className="object-cover"
              />
              <AvatarFallback className="bg-gradient-to-br from-blue-500 to-blue-600 text-xl font-bold">
                {profile?.name?.charAt(0) || "U"}
              </AvatarFallback>
            </Avatar>
            <div className="space-y-2">
              <CardTitle className="text-xl font-bold tracking-tight">
                {profile?.name}
              </CardTitle>
              <CardDescription className="flex items-center space-x-2 text-blue-800">
                <span className="font-medium">{profile?.role && profile.role.charAt(0).toUpperCase() + profile.role.slice(1)}</span>
              </CardDescription>
              <CardDescription className="flex items-center space-x-2">
                <span className="font-medium">
                  <AddressSection
                    userData={profile as ProfileType}
                    setEditedProfile={setEditedProfile}
                    editedProfile={editedProfile}
                  />
                </span>
              </CardDescription>
            </div>
          </div>
        </CardHeader>
        <CardContent className="p-6 border-b">
          <p className="text-2xl font-medium pb-6 text-blue-800">Thông tin cá nhân</p>
          <div className="grid grid-cols-2 gap-y-8 gap-x-6">
            <div className="space-y-2">
              <Label className="text-sm font-semibold text-gray-300 flex items-center">
                Họ và tên
              </Label>
              {editedProfile.isEdited ? (
                <Input
                  value={editedProfile.editedName}
                  maxLength={50}
                  onChange={(e) =>
                    setEditedProfile((prev) => ({
                      ...prev,
                      editedName: e.target.value,
                    }))
                  }
                  className="border-gray-200 focus:border-blue-500 focus:ring-2 focus:ring-blue-200 transition-all duration-200 h-12 font-medium text-md"
                  placeholder="Nhập họ và tên"
                />
              ) : (
                <div className="font-medium text-gray-900 rounded-md">
                  {editedProfile.editedName || "Chưa cập nhật"}
                </div>
              )}
            </div>
            <div className="space-y-2">
              <Label className="text-sm font-semibold text-gray-300 flex items-center">
                Số điện thoại
              </Label>
              {editedProfile.isEdited ? (
                <Input
                  value={editedProfile.editedPhoneNumber}
                  onChange={(e) =>
                    setEditedProfile((prev) => ({
                      ...prev,
                      editedPhoneNumber: e.target.value,
                    }))
                  }
                  className="focus:border-blue-500 focus:ring-2 focus:ring-blue-200 transition-all duration-200 h-12 font-medium text-md"
                  placeholder="Nhập số điện thoại"
                />
              ) : (
                <div className="text-gray-900 rounded-md font-medium">
                  {profile?.phoneNumber || "Chưa cập nhật số điện thoại"}
                </div>
              )}
            </div>
            <div className="space-y-2">
              <Label className="text-sm font-semibold text-gray-300 flex items-center no">
                Email
              </Label>
              <div className="text-gray-900 rounded-md font-medium">
                {profile?.email || "Chưa cập nhật email"}
              </div>
            </div>
          </div>
        </CardContent>
        <CardContent>
          <p className="text-2xl font-medium text-blue-800 py-6">Địa chỉ</p>
          <div className="grid grid-cols-2 gap-y-8">
            <div className="space-y-2">
              <Label className="text-sm font-semibold text-gray-300 flex items-center">
                Số nhà, đường
              </Label>
              <div className="text-gray-900 rounded-md font-medium">
                {profile?.address?.street || "Chưa cập nhật email"}
              </div>
            </div>
            <div className="space-y-2">
              <Label className="text-sm font-semibold text-gray-300 flex items-center">
                Phường
              </Label>
              <div className="text-gray-900 rounded-md font-medium">
                {profile?.address?.ward || "Chưa cập nhật email"}
              </div>
            </div>
            <div className="space-y-2">
              <Label className="text-sm font-semibold text-gray-300 flex items-center">
                Quận
              </Label>
              <div className="text-gray-900 rounded-md font-medium">
                {profile?.address?.district || "Chưa cập nhật email"}
              </div>
            </div>
            <div className="space-y-2">
              <Label className="text-sm font-semibold text-gray-300 flex items-center">
                Tỉnh / Thành phố
              </Label>
              <div className="text-gray-900 rounded-md font-medium">
                {profile?.address?.city || "Chưa cập nhật email"}
              </div>
            </div>
          </div>

        </CardContent>
        <CardFooter className="flex justify-end gap-4 p-6">
          {editedProfile.isEdited ? (
            <>
              <Button
                onClick={handleUpdateProfile}
                className="bg-primary text-white px-8 py-6 font-semibold shadow-lg transition-all duration-300"
              >
                Lưu thay đổi
              </Button>
              <Button
                variant="outline"
                onClick={() =>
                  setEditedProfile((prev) => ({ ...prev, isEdited: false }))
                }
                className="border-gray-300 hover:bg-gray-100 px-8 py-6 font-semibold transition-all duration-300"
              >
                Hủy
              </Button>
            </>
          ) : (
            <Button
              onClick={() =>
                setEditedProfile((prev) => ({ ...prev, isEdited: true }))
              }
              className=" text-white px-8 py-6 font-semibold shadow-lg hover:shadow-xl bg-primary transition-all duration-300"
            >
              Chỉnh sửa thông tin
            </Button>
          )}
        </CardFooter>
      </Card>
    </div>
  );
};

export default UserProfile;
