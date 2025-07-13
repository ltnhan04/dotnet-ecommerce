import React from "react";
import Link from "next/link";
import { ChevronRight, Home } from "lucide-react";
import { usePathname } from "next/navigation";

const Breadcrumb: React.FC = (): React.ReactElement => {
  const pathname = usePathname();
  const paths = pathname.split("/").filter(Boolean);

  const breadcrumbItems = [
    {
      label: (
        <div className="flex items-center text-gray-600 hover:text-blue duration-500 ease-out font-semibold text-sm">
          <Home className="w-5 h-5" />
        </div>
      ),
      href: "/",
    },
    ...paths.map((segment, index) => {
      const href = "/" + paths.slice(0, index + 1).join("/");
      const label = segment
        .split("-")
        .map((word) => word.charAt(0).toUpperCase() + word.slice(1))
        .join(" ");

      return {
        label,
        href,
      };
    }),
  ];

  return (
    <nav className="flex items-center pb-4 overflow-x-auto whitespace-nowrap">
      {breadcrumbItems.map((item, index) => {
        const isLast = index === breadcrumbItems.length - 1;

        return (
          <React.Fragment key={item.href}>
            {index > 0 && (
              <ChevronRight className="w-3 h-3 mx-2 text-gray-600 flex-shrink-0 font-semibold" />
            )}
            {isLast ? (
              <span className="text-gray-800 font-semibold text-sm uppercase hover:text-blue duration-500 ease-out cursor-pointer">
                {item.label}
              </span>
            ) : (
              <Link
                href={item.href}
                className="text-gray-800 font-semibold text-sm uppercase hover:text-blue duration-500 ease-out cursor-pointer"
              >
                {item.label}
              </Link>
            )}
          </React.Fragment>
        );
      })}
    </nav>
  );
};

export default Breadcrumb;
