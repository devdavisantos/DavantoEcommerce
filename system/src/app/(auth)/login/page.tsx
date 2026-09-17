import Link from "next/link";
import Logo from "@/components/ui/Logo";
import LoginForm from "@/components/auth/LoginForm";

type LoginPageProps = {
  searchParams: Promise<{
    registered?: string;
  }>;
};

export default async function Login({
  searchParams,
}: LoginPageProps) {
  const params = await searchParams;

  const accountCreated = params.registered === "true";
  return (
    <div className="h-screen w-screen flex flex-col items-center justify-center bg-charcoal-dark p-4">
      <div className="flex justify-center mb-10">
          <Link href="/" className="contents">
            <Logo height={200} width={200} />
          </Link>
      </div>

      <div className="w-full max-w-md shadow-xl rounded-2xl p-8 bg-charcoal-gray">
        <LoginForm
        showReturnHome
        successMessage={
            accountCreated
              ? "Account created successfully. You can now sign in."
              : undefined
          }/>
      </div>
    </div>
  );
}