"use client";

import { useState } from "react";
import Logo from "@/components/ui/Logo";
import Link from "next/link";

export default function Login() {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");

  return (
    <div className="min-h-screen flex flex-col items-center justify-center bg-gray-950 p-4">
        <div className="flex justify-center mb-10">
          <Logo height={200} width={200}/>
        </div>
        <div className="modal w-full max-w-md shadow-xl rounded-2xl p-8 bg-gray-900">
        <h1 className="text-2xl font-bold mb-6 text-center text-white">Sign In</h1>
            <div className="space-y-4">
              <input
                type="email"
                placeholder="Email"
                value={email}
                onChange={(e) => setEmail(e.target.value)}
                className="w-full px-4 py-2 rounded-md bg-gray-300 text-black border-white border-2 focus:border-white outline-none focus:ring-white ring-1"/>

              <input
                type="password"
                placeholder="Password"
                value={password}
                onChange={(e) => setPassword(e.target.value)}
                className="w-full px-4 py-2 rounded-md bg-gray-300 text-blackw-full px-4 py-2 rounded-md bg-gray-300 text-black border-white border-2 focus:border-white outline-none focus:ring-white ring-1"/>

              <button className="w-full bg-blue-600 hover:bg-blue-700 py-2 rounded-md font-semibold">
                Sign In
              </button>
            </div>

            <div className="text-center mt-4 text-sm">
              <Link href="/forgotpass" className="text-blue-400 hover:underline">
                Forgot your password?
              </Link>
            </div>

            <div className="text-center mt-2 text-sm text-gray-300">
              Don't have an account?{" "}
              <Link href="/register" className="text-blue-400 hover:underline">
                Create one
              </Link>
            </div>
        </div>
    </div>
  );
}