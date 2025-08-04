import { axiosInstance } from "@/config/axiosInstance";
import { IDistrictType, IProvinceType, IWardType } from "@/types/profile";

export const getProvinces = async () => {
  const res = await axiosInstance.get<IProvinceType[]>(
    `${process.env.NEXT_PUBLIC_API_ENDPOINT}/api/v1/provinces`
  );
  return res.data;
};
export const getDistricts = async (code: number) => {
  const res = await axiosInstance.get<IDistrictType[]>(
    `${process.env.NEXT_PUBLIC_API_ENDPOINT}/api/v1/districts/${code}`
  );
  return res.data;
};

export const getWards = async (code: number) => {
  const res = await axiosInstance.get<IWardType[]>(
    `${process.env.NEXT_PUBLIC_API_ENDPOINT}/api/v1/wards/${code}`
  );
  return res.data;
};
