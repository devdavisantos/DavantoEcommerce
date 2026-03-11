"use client";

import { useState } from "react";
import Logo from "@/components/ui/Logo";

export default function ResetPassword() {
  const [password, setPassword] = useState("");
  const [confirmPassword, setConfirmPassword] = useState("");

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();

    if (password !== confirmPassword) {
      alert("Passwords do not match!");
      return;
    }

    // Aqui você chamaria sua API para atualizar a senha
    console.log("Password updated:", password);
  };

  return (
    <div className="min-h-screen flex flex-col items-center justify-center bg-gray-950 p-4">
      <div className="flex justify-center mb-10">
        <Logo className="scale-100" />
      </div>
      <div className="modal w-full max-w-md shadow-xl rounded-2xl p-8 bg-gray-900">
        <h1 className="text-2xl font-bold mb-6 text-center text-white">Reset Password</h1>

        <form className="space-y-4" onSubmit={handleSubmit}>
          <input
            type="password"
            placeholder="New Password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            className="w-full border px-4 py-2 rounded-md bg-gray-400 text-black placeholder-black
            focus:outline-1 focus:outline-white focus:border-white"
          />

          <input
            type="password"
            placeholder="Confirm Password"
            value={confirmPassword}
            onChange={(e) => setConfirmPassword(e.target.value)}
            className="w-full border px-4 py-2 rounded-md bg-gray-400 text-black placeholder-black
            focus:outline-1 focus:outline-white focus:border-white"
          />

          <button
            type="submit"
            className="w-full bg-blue-600 hover:bg-blue-700 text-white py-2 rounded-md font-semibold"
          >
            Save Password
          </button>
        </form>
      </div>
    </div>
  );
}