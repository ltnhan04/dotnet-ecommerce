import { axiosInstance } from "@/config/axiosInstance";
import { IResponseReview } from "@/types/review";

export const createReview = async ({
  variant,
  rating,
  comment,
}: {
  variant: string;
  rating: number;
  comment: string;
}): Promise<IResponseReview> => {
  const res = await axiosInstance.post("/v1/reviews", {
    variant,
    rating,
    comment,
  });
  return res.data;
};

export const updateReview = async (
  id: string,
  {
    variant,
    rating,
    comment,
  }: {
    variant: string;
    rating?: number;
    comment?: string;
  }
): Promise<IResponseReview> => {
  const res = await axiosInstance.put(`/v1/reviews/${id}`, {
    variant,
    rating,
    comment,
  });
  return res.data;
};
export const deleteReview = async (id: string): Promise<IResponseReview> => {
  const res = await axiosInstance.delete(`/v1/reviews/${id}`);
  return res.data;
};
