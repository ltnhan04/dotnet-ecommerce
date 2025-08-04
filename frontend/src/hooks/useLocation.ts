import { useQuery } from "@tanstack/react-query";
import {
  getProvinces,
  getDistricts,
  getWards,
} from "@/services/profile/profileApi";
import { IProvinceType, IDistrictType, IWardType } from "@/types/profile";

export const useProvinces = () => {
  return useQuery<IProvinceType[]>({
    queryKey: ["provinces"],
    queryFn: getProvinces,
  });
};

export const useDistricts = (provinceCode?: number) => {
  return useQuery<IDistrictType[]>({
    queryKey: ["districts", provinceCode],
    queryFn: () =>
      provinceCode ? getDistricts(provinceCode) : Promise.resolve([]),
    enabled: !!provinceCode,
  });
};

export const useWards = (districtCode?: number) => {
  return useQuery<IWardType[]>({
    queryKey: ["wards", districtCode],
    queryFn: () =>
      districtCode ? getWards(districtCode) : Promise.resolve([]),
    enabled: !!districtCode,
  });
};
