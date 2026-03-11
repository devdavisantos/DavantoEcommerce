"use client";

import Image from "next/image";
import logo from "../../../public/lastore.png";

export default function Logo({ height = 40, width = 120 }: { height?: number, width?: number }) {
  return (
    <Image 
      src={logo}
      alt="Logo"
      width={width}
      height={height}
    />
  );
}