"use client";

import { useState } from "react";
import Logo from "@/components/ui/Logo";
import Link from "next/link";
import { useRouter } from "next/navigation";

export default function Register() {
  const router = useRouter();
  const [name, setName] = useState("");
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [confirmPassword, setConfirmPassword] = useState("");
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(false);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError("");

    // Validação básica no frontend
    if (!name || !email || !password || !confirmPassword) {
      setError("Todos os campos são obrigatórios");
      return;
    }

    if (password !== confirmPassword) {
      setError("As senhas não coincidem");
      return;
    }

    if (password.length < 6) {
      setError("A senha deve ter no mínimo 6 caracteres");
      return;
    }

    setLoading(true);

    try {
      const response = await fetch("http://localhost:3333/api/auth/register", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify({
          name,
          email,
          password,
          confirmPassword,
        }),
      });

      const data = await response.json();

      if (!response.ok) {
        setError(data.error || "Erro ao cadastrar usuário");
        setLoading(false);
        return;
      }

      // Sucesso - redirecionar para login
      router.push("/login?registered=true");
    } catch (err) {
      setError("Erro ao conectar com o servidor. Verifique se a API está rodando.");
      setLoading(false);
    }
  };

  return (
    <div className="min-h-screen flex flex-col items-center justify-center bg-gray-950 p-4">
        <div className="flex justify-center mb-10">
          <Logo height={200} width={200}/>
        </div>
        <div className="modal w-full max-w-md shadow-xl rounded-2xl p-8 bg-gray-900">
        <h1 className="text-2xl font-bold mb-6 text-center text-white">Create Account</h1>
            <form onSubmit={handleSubmit} className="space-y-4">
              {error && (
                <div className="bg-red-500 text-white px-4 py-2 rounded-md text-sm">
                  {error}
                </div>
              )}

              <input
                type="text"
                placeholder="Full Name"
                value={name}
                onChange={(e) => setName(e.target.value)}
                disabled={loading}
                className="w-full px-4 py-2 rounded-md bg-gray-300 text-black border-white border-2 focus:border-white outline-none focus:ring-white ring-1 disabled:opacity-50"/>
              
              <input
                type="email"
                placeholder="Email"
                value={email}
                onChange={(e) => setEmail(e.target.value)}
                disabled={loading}
                className="w-full px-4 py-2 rounded-md bg-gray-300 text-black border-white border-2 focus:border-white outline-none focus:ring-white ring-1 disabled:opacity-50"/>

              <input
                type="password"
                placeholder="Password"
                value={password}
                onChange={(e) => setPassword(e.target.value)}
                disabled={loading}
                className="w-full px-4 py-2 rounded-md bg-gray-300 text-black border-white border-2 focus:border-white outline-none focus:ring-white ring-1 disabled:opacity-50"/>

              <input
                type="password"
                placeholder="Confirm Password"
                value={confirmPassword}
                onChange={(e) => setConfirmPassword(e.target.value)}
                disabled={loading}
                className="w-full px-4 py-2 rounded-md bg-gray-300 text-black border-white border-2 focus:border-white outline-none focus:ring-white ring-1 disabled:opacity-50"/>

              <button 
                type="submit"
                disabled={loading}
                className="w-full bg-blue-600 hover:bg-blue-700 py-2 rounded-md font-semibold disabled:opacity-50 disabled:cursor-not-allowed">
                {loading ? "Cadastrando..." : "Register"}
              </button>
            </form>

            <div className="text-center mt-2 text-sm text-gray-300">
              Already have an account?{" "}
              <Link href="/login" className="text-blue-400 hover:underline">
                Sign in
              </Link>
            </div>
        </div>
    </div>
  );
}