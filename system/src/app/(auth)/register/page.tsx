// "use client";

// import { useState } from "react";
// import Logo from "@/components/ui/Logo";
// import Link from "next/link";
// import { useRouter } from "next/navigation";

// export default function Register() {
//   const router = useRouter();
//   const [name, setName] = useState("");
//   const [email, setEmail] = useState("");
//   const [password, setPassword] = useState("");
//   const [confirmPassword, setConfirmPassword] = useState("");
//   const [error, setError] = useState("");
//   const [loading, setLoading] = useState(false);

//   const handleSubmit = async (e: React.FormEvent) => {
//     e.preventDefault();
//     setError("");

//     if (!name || !email || !password || !confirmPassword) {
//       setError("Todos os campos são obrigatórios");
//       return;
//     }

//     if (password !== confirmPassword) {
//       setError("As senhas não coincidem");
//       return;
//     }

//     if (password.length < 6) {
//       setError("A senha deve ter no mínimo 6 caracteres");
//       return;
//     }

//     setLoading(true);

//     try {
//       const response = await fetch("http://localhost:3333/api/auth/register", {
//         method: "POST",
//         headers: {
//           "Content-Type": "application/json",
//         },
//         body: JSON.stringify({
//           name,
//           email,
//           password,
//           confirmPassword,
//         }),
//       });

//       const data = await response.json();

//       if (!response.ok) {
//         setError(data.error || "Erro ao cadastrar usuário");
//         setLoading(false);
//         return;
//       }

//       router.push("/login?registered=true");
//     } catch (err) {
//       setError("Erro ao conectar com o servidor. Verifique se a API está rodando.");
//       setLoading(false);
//     }
//   };

//   return (
//     <div className="h-screen w-screen flex flex-col items-center justify-center bg-charcoal-dark p-4">
            
//       <div className="flex justify-center mb-10">
//           <Link
//             href="/"
//             aria-label="Go to home"
//             className="incline-flex cursor-pointer">
//             <Logo height={200} width={200} />
//           </Link>
//       </div>
    
//       <div className="modal w-full max-w-md shadow-xl rounded-2xl p-8 bg-charcoal-gray">
//         <h1 className="text-2xl font-bold mb-6 text-center text-white">
//           Create Account
//         </h1>
//         <div className="space-y-4">
//           <input
//             type="text"
//             placeholder="Full Name"
//             value={name}
//             onChange={(e) => setName(e.target.value)}
//             disabled={loading}
//             className="w-full px-4 py-2 rounded-md bg-gray-300 text-black border-white border-2 focus:border-white outline-none focus:ring-white ring-1 disabled:opacity-50"
//           />

//           <input
//             type="email"
//             placeholder="Email"
//             value={email}
//             onChange={(e) => setEmail(e.target.value)}
//             disabled={loading}
//             className="w-full px-4 py-2 rounded-md bg-gray-300 text-black border-white border-2 focus:border-white outline-none focus:ring-white ring-1 disabled:opacity-50"/>

//           <input
//             type="password"
//             placeholder="Password"
//             value={password}
//             onChange={(e) => setPassword(e.target.value)}
//             disabled={loading}
//             className="w-full px-4 py-2 rounded-md bg-gray-300 text-black border-white border-2 focus:border-white outline-none focus:ring-white ring-1 disabled:opacity-50"/>

//           <input
//             type="password"
//             placeholder="Confirm Password"
//             value={confirmPassword}
//             onChange={(e) => setConfirmPassword(e.target.value)}
//             disabled={loading}
//             className="w-full px-4 py-2 rounded-md bg-gray-300 text-black border-white border-2 focus:border-white outline-none focus:ring-white ring-1 disabled:opacity-50"/>

//           <button 
//             type="submit"
//             disabled={loading}
//             className="w-full bg-amber-primary hover:bg-amber-secondary py-2 rounded-md font-semibold cursor-pointer">
//             {loading ? "Cadastrando..." : "Register"}
//           </button>

//           <div className="text-center mt-2 text-sm text-gray-300">
//             Already have an account?{" "}
//             <Link href="/login">
//               <span className="text-deep-orange hover:text-amber-500">
//                 Sign in
//               </span>
//             </Link>
//           </div>

//           <div className="text-center mt-2 text-sm text-gray-400">
//             <Link href="/" className="inline-flex items-center justify-center gap-0.5">
//               <span className="hover:text-white">Return to Home</span>
//             </Link>
//           </div>

//         </div>
//       </div>
//     </div>
//   )
// }

"use client";

import { useState } from "react";
import Logo from "@/components/ui/Logo";
import Link from "next/link";
import { useRouter } from "next/navigation";

type ApiErrorResponse = {
  code?: string;
  message?: string;
};

export default function Register() {
  const router = useRouter();

  const [name, setName] = useState("");
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [confirmPassword, setConfirmPassword] = useState("");

  const hasConfirmPassword = confirmPassword.length > 0;
  const passwordsMatch = hasConfirmPassword && password === confirmPassword;
  const passwordsDoNotMatch = hasConfirmPassword && password !== confirmPassword;

  const [errorMessage, setErrorMessage] = useState("");
  const [loading, setLoading] = useState(false);

  const handleSubmit = async (
    event: React.FormEvent<HTMLFormElement>
  ) => {
    event.preventDefault();

    if (loading) return;

    setErrorMessage("");

    if (!name.trim() || !email.trim() || !password || !confirmPassword) {
      setErrorMessage("All fields are required.");
      return;
    }

    if (password !== confirmPassword) {
      setErrorMessage("Passwords do not match.");
      return;
    }

    if (password.length < 8) {
      setErrorMessage("Password must contain at least 8 characters.");
      return;
    }

    setLoading(true);

    try {
      const response = await fetch("/api/auth/register", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        credentials: "same-origin",
        body: JSON.stringify({
          name: name.trim(),
          email: email.trim(),
          password,
        }),
      });

      if (!response.ok) {
        const data: ApiErrorResponse = await response
          .json()
          .catch(() => ({}));

        setErrorMessage(
          data.message ?? "Unable to create account."
        );

        return;
      }

      router.replace("/login?registered=true");
    } catch {
      setErrorMessage(
        "Unable to connect to the server. Please try again."
      );
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="h-screen w-screen flex flex-col items-center justify-center bg-charcoal-dark p-4">
      <div className="flex justify-center mb-10">
        <Link
          href="/"
          aria-label="Go to home"
          className="inline-flex cursor-pointer"
        >
          <Logo height={200} width={200} />
        </Link>
      </div>

      <div className="w-full max-w-md shadow-xl rounded-2xl p-8 bg-charcoal-gray">
        <h1 className="text-2xl font-bold mb-6 text-center text-white">
          Create Account
        </h1>

        <form
          onSubmit={handleSubmit}
          className="space-y-4"
        >
          <input
            type="text"
            placeholder="Full Name"
            value={name}
            onChange={(e) => setName(e.target.value)}
            disabled={loading}
            autoComplete="name"
            className="w-full px-4 py-2 rounded-md bg-gray-300 text-black border-white border-2 focus:border-white outline-none focus:ring-white ring-1 disabled:opacity-50"
          />

          <input
            type="email"
            placeholder="Email"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            disabled={loading}
            autoComplete="email"
            className="w-full px-4 py-2 rounded-md bg-gray-300 text-black border-white border-2 focus:border-white outline-none focus:ring-white ring-1 disabled:opacity-50"
          />

          <input
            type="password"
            placeholder="Password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            disabled={loading}
            autoComplete="new-password"
            className="w-full px-4 py-2 rounded-md bg-gray-300 text-black border-white border-2 focus:border-white outline-none focus:ring-white ring-1 disabled:opacity-50"
          />

          <div>
            <input
              type="password"
              placeholder="Confirm Password"
              value={confirmPassword}
              onChange={(e) => setConfirmPassword(e.target.value)}
              disabled={loading}
              autoComplete="new-password"
              aria-invalid={passwordsDoNotMatch}
              className={`
                w-full px-4 py-2 rounded-md
                bg-gray-300 text-black
                border-2
                outline-none
                ring-1
                disabled:opacity-50
                ${
                  passwordsDoNotMatch
                    ? "border-red-500 ring-red-500"
                    : passwordsMatch
                      ? "border-green-500 ring-green-500"
                      : "border-white ring-white"
                }
              `}
            />
          
            {passwordsDoNotMatch && (
              <p className="mt-1 text-sm text-red-400">
                Passwords do not match.
              </p>
            )}
          
            {passwordsMatch && (
              <p className="mt-1 text-sm text-green-400">
                Passwords match.
              </p>
            )}
          </div>

          {errorMessage && (
            <div
              role="alert"
              className="rounded-md border border-red-500/30 bg-red-500/10 px-4 py-3 text-center text-sm text-red-400"
            >
              {errorMessage}
            </div>
          )}

          <button
            type="submit"
            disabled={loading}
            className="flex h-10 w-full items-center justify-center rounded-md bg-amber-primary font-semibold cursor-pointer hover:bg-amber-secondary disabled:cursor-not-allowed disabled:opacity-60"
          >
            {loading ? (
              <span className="h-5 w-5 animate-spin rounded-full border-2 border-white/30 border-t-white" />
            ) : (
              "Register"
            )}
          </button>

          <div className="text-center mt-2 text-sm text-gray-300">
            Already have an account?{" "}

            <Link
              href="/login"
              className="text-deep-orange hover:!text-amber-500"
            >
              Sign in
            </Link>
          </div>

          <div className="text-center mt-2 text-sm text-gray-400">
            <Link
              href="/"
              className="inline-flex items-center justify-center hover:!text-white"
            >
              Return to Home
            </Link>
          </div>
        </form>
      </div>
    </div>
  );
}