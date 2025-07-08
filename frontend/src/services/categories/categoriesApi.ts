import { axiosInstance } from "@/config/axiosInstance";
import { Category } from "@/types/category";

export const getCategories = async (): Promise<Category[]> => {
  const res = await axiosInstance.get<{ data: Category[] }>(
    `/api/v1/admin/categories/`
  );
  return res.data.data.filter((category) => !category.parent_category);
};

export const getSubCategories = async (parentCategoryId: string) => {
  const res = await axiosInstance.get(
    `/api/v1/admin/categories/sub/${parentCategoryId}`
  );
  return res.data;
};
