import React from "react";
import Link from "next/link";
import LoginForm from "@/app/(auth)/login/login-form";
import Image from "next/image";

export default function Login() {
  return (
    <div className="h-screen flex items-center justify-center bg-[#f3f4f6]">
      <div className="max-w-4xl w-full mx-auto p-8 md:p-12 bg-white border-[#e6e6e6] rounded-xl shadow-md shadow-[#e6e6e6]">
        <div className="grid grid-cols-1 md:grid-cols-2 gap-8 items-center">
          <div className="flex items-center justify-center">
            <Image
              src={"/assets/images/login-image.jpg"}
              width={400}
              height={400}
              alt="Image"
              quality={100}
              priority={true}
            />
          </div>
          <div>
            <h1 className="text-2xl font-bold text-center font-sans text-primary mb-3">
              ĐĂNG NHẬP
            </h1>
            <p className="text-center text-gray-500 font-sans font-normal mb-6">
              Khám phá sản phẩm yêu thích của bạn ngay!
            </p>
            <div>
              <LoginForm />
              <h1 className="text-sm text-center mt-5 font-semibold text-gray-600 font-sans">
                Bạn chưa có tài khoản?
                <Link
                  className="text-primary ml-1 hover:underline transition-colors duration-200 ease-out"
                  href="/register"
                >
                  Đăng ký
                </Link>
              </h1>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}
