"use client";

import { useState } from "react";
import Logo from "@/components/ui/Logo";
import Link from "next/link";

export default function Login() {
  const [email, setEmail] = useState("");

  return (
    <div className="min-h-screen flex flex-col items-center justify-center bg-gray-950 px-4">

      <div className="mb-10">
        <Logo />
      </div>

      <div className="w-full max-w-md bg-gray-900 rounded-2xl shadow-xl p-8">
        <h1 className="text-2xl font-bold text-center text-white mb-6">
          Reset Password
        </h1>
    
        <div className="space-y-4">
          <input
            type="email"
            placeholder="Your email"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            className="w-full px-4 py-2 rounded-md bg-gray-200 text-black border border-gray-300 focus:outline-none focus:ring-2 focus:ring-blue-500"/>
          <button className="w-full bg-blue-600 hover:bg-blue-700 text-white py-2 rounded-md font-semibold transition">
            Reset Password
          </button>
          <p className="text-center text-sm text-gray-400 cursor-pointer hover:text-white">
            Back to login
          </p>
        </div>

      </div>
    </div>
  );
}