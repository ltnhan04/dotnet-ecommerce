"use client";
import React, { useState, useEffect } from "react";
import Breadcrumb from "../../../../components/common/breadcrumb";
import { useGetProductBySlug } from "@/hooks/useProducts";
import Loading from "@/app/loading";
import { useAuth } from "@/hooks/useAuth";
import ProductImages from "./components/ProductImages";
import ProductInfo from "./components/ProductInfo";
import ProductVariants from "./components/ProductVariants";
import ProductActions from "./components/ProductActions";
import ProductReviews from "./components/ProductReviews";
import ProductNotFound from "@/components/common/product-not-found";
import { ProductVariant } from "@/types/product";

export default function ProductDetail({
  params,
}: {
  params: { slug: string };
}) {
  const { data: productData, isLoading } = useGetProductBySlug(params.slug);
  const [selectedVariant, setSelectedVariant] = useState<ProductVariant | null>(
    null
  );
  const { user } = useAuth();

  useEffect(() => {
    if (productData?.data?.variants?.length > 0) {
      setSelectedVariant(productData.data.variants[0]);
    }
  }, [productData]);

  if (isLoading) {
    return <Loading />;
  }

  if (!productData?.data || !selectedVariant) {
    return <ProductNotFound />;
  }

  const variants: ProductVariant[] = productData.data.variants;

  const handleVariantSelect = (colorName: string, storage: string) => {
    const variant = variants.find(
      (v: ProductVariant) =>
        v.color.colorName === colorName && v.storage === storage
    );
    if (variant) {
      setSelectedVariant(variant);
    }
  };

  return (
    <div className="min-h-screen bg-white">
      <div className="container mx-auto px-4 py-6">
        <Breadcrumb />
        <div className="grid grid-cols-1 md:grid-cols-2 gap-8">
          <ProductImages
            images={selectedVariant.images}
            name={productData.data.name}
          />

          <div className="space-y-6">
            <ProductInfo
              product={productData.data!}
              variant={selectedVariant}
            />
            <ProductVariants
              variants={variants}
              selectedVariant={selectedVariant}
              onVariantSelect={handleVariantSelect}
            />
            <ProductActions
              product={productData?.data}
              variant={selectedVariant}
            />
          </div>
        </div>

        <div className="mt-12">
          <ProductReviews
            reviews={selectedVariant.reviews || []}
            currentUser={user || null}
          />
        </div>
      </div>
    </div>
  );
}
