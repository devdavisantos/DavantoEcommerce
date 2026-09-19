"use client";
import { authenticatedFetch } from "@/lib/auth-client";

import { useEffect, useState } from "react";
import { Camera, LockKeyhole, Mail, Save, UserRound, CircleCheck, CircleX } from "@/components/icons/icons";
import { useAuth } from "@/contexts/AuthContext";
import { useRouter } from "next/navigation";

export default function ProfilePage() {
  const { user, refreshSession } = useAuth();

  const [name, setName] = useState("");
  const [document, setDocument] = useState("");
  const [phone, setPhone] = useState("");
  const [email, setEmail] = useState("");
  const [currentPassword, setCurrentPassword] = useState("");
  const [newPassword, setNewPassword] = useState("");
  const [confirmPassword, setConfirmPassword] = useState("");
  const [isSavingProfile, setIsSavingProfile] = useState(false);
  const [profileMessage, setProfileMessage] = useState("");
  const [isChangingPassword, setIsChangingPassword] = useState(false);

  const router = useRouter();

  const [profileAlert, setProfileAlert] = useState<{
    type: "success" | "error";
    message: string;
  } | null>(null);

  useEffect(() => {
    if (!profileAlert) return;

    const timeout = setTimeout(() => {
      setProfileAlert(null);
    }, 4000);

    return () => clearTimeout(timeout);
  }, [profileAlert]);

  const handleSaveProfile = async () => {
    if (isSavingProfile) return;

    const payload: {
      name?: string;
      document?: string;
      phone?: string;
    } = {};

    if (name.trim()) {
      payload.name = name.trim();
    }

    if (document.trim()) {
      payload.document = document.trim();
    }

    if (phone.trim()) {
      payload.phone = phone.trim();
    }

    if (Object.keys(payload).length === 0) {
      setProfileMessage("Enter at least one field to update.");
      return;
    }

    setIsSavingProfile(true);
    setProfileAlert(null);

    try {
      const response = await authenticatedFetch("/api/customer/profile", {
        method: "PATCH",
        headers: {
          "Content-Type": "application/json",
        },
        credentials: "same-origin",
        body: JSON.stringify({
          name,
          document,
          phone,
        }),
      });

      if (!response.ok) {
        const error = await response.json().catch(() => null);

        setProfileAlert({
          type: "error",
          message:
            error?.message ?? "Unable to update profile."
        });

        return;
      }

      setProfileAlert({
        type: "success",
        message: "Profile updated successfully.",
      });

      await refreshSession();

      router.refresh();

    } catch {
      setProfileAlert({
        type: "error",
        message: "Unable to update profile.",
      });
    } finally {
      setIsSavingProfile(false);
    }
  }

  useEffect(() => {
    if (!user) return;

    setName(typeof user.name === "string" ? user.name : "");
    setEmail(typeof user.email === "string" ? user.email : "");
    setDocument(
      typeof user.document === "string" ? user.document : ""
    );
    setPhone(
      typeof user.phone === "string" ? user.phone : ""
    );
  }, [user]);

  const hasNewPassword = newPassword.length > 0;
  const isPasswordLongEnough = newPassword.length >= 8;
  const hasConfirmPassword = confirmPassword.length > 0;

  const passwordsMatch = hasConfirmPassword && newPassword === confirmPassword;
  const passwordsDoNotMatch = hasConfirmPassword && newPassword !== confirmPassword;

  const handleChangePassword = async () => {
    if (isChangingPassword) return;

    if (!currentPassword) {
      setProfileAlert({
        type: "error",
        message: "Current password is required."
      });

      return;
    }

    if (newPassword.length < 8) {
      setProfileAlert({
        type: "error",
        message: "Password must contain at least 8 characters."
      });

      return;
    }

    if (newPassword !== confirmPassword) {
      setProfileAlert({
        type: "error",
        message: "Password do not match."
      });

      return;
    }

    setIsChangingPassword(true);
    setProfileAlert(null);

    try {
      const response = await authenticatedFetch("/api/customer/change-password", {
        method: "PUT",
        headers: {"Content-Type": "application/json",},
        credentials: "same-origin",
        body: JSON.stringify({
          currentPassword,
          newPassword,
        }),
      });

      if (!response.ok) {
        const error = await response.json().catch(() => null);

        setProfileAlert({
          type: "error",
          message:
            error?.message ?? "Unable to change password.",
        });

        return;
      }

      setProfileAlert({
        type: "success",
        message: "Password changed successfully.",
      });

      setCurrentPassword("");
      setNewPassword("");
      setConfirmPassword("");

      await refreshSession();

      router.refresh();
    } catch {
      setProfileAlert({
        type: "error",
        message: "Unable to change password."
      });
    } finally {
      setIsChangingPassword(false);
    }
  };

  return (
        <>
          <div className="space-y-10">
            
            {/* <section className="flex flex-col items-center">
              <div className="relative">
                <div className="flex h-28 w-28 items-center justify-center rounded-full border border-white/10 bg-charcoal-gray">
                  <UserRound size={52} className="text-gray-400" />
                </div>

                <button
                  type="button"
                  className="absolute bottom-0 right-0 flex h-9 w-9 items-center justify-center rounded-full bg-deep-orange text-white transition hover:scale-105 cursor-pointer"
                  aria-label="Change profile photo"
                >
                  <Camera size={17} />
                </button>
              </div>

              <button
                type="button"
                className="mt-3 text-sm text-gray-400 transition hover:!text-deep-orange cursor-pointer"
              >
                Change photo
              </button>
            </section> */}

            <section>
              <div className="mb-5 flex items-center gap-2">
                <UserRound size={20} className="text-deep-orange" />
                <h2 className="text-lg font-semibold">
                  Personal information
                </h2>
              </div>

              <div className="grid grid-cols-1 gap-5 md:grid-cols-2">
                <div>
                  <label className="mb-2 block text-sm text-gray-400">
                    Name
                  </label>

                  <input
                    type="text"
                    value={name}
                    onChange={(e) => setName(e.target.value)}
                    className="w-full rounded-lg border border-white/10 bg-steel-gray px-4 py-3 outline-none transition focus:border-deep-orange"
                  />
                </div>

                <div>
                  <label className="mb-2 block text-sm text-gray-400">
                    Phone
                  </label>

                  <input
                    type="tel"
                    value={phone}
                    onChange={(e) => setPhone(e.target.value)}
                    className="w-full rounded-lg border border-white/10 bg-steel-gray px-4 py-3 outline-none transition focus:border-deep-orange"
                  />
                </div>

                <div>
                  <label className="mb-2 block text-sm text-gray-400">
                    Document
                  </label>

                  <input
                    type="text"
                    value={document}
                    onChange={(e) => setDocument(e.target.value)}
                    className="w-full rounded-lg border border-white/10 bg-steel-gray px-4 py-3 outline-none transition focus:border-deep-orange"
                  />
                </div>
              </div>

              <div className="mt-6 flex justify-end">
                <button
                  type="button"
                  onClick={handleSaveProfile}
                  disabled={isSavingProfile}
                  className="flex items-center gap-2 rounded-lg bg-deep-orange px-5 py-2.5 font-medium text-white transition hover:opacity-90 cursor-pointer"
                >
                  <Save size={18} />
                  <span>{isSavingProfile ? "Saving..." : "Save changes"}</span>
                </button>
              </div>
            </section>

            <div className="border-t border-white/10" />

            {/* EMAIL */}
            <section>
              <div className="mb-5 flex items-center gap-2">
                <Mail size={20} className="text-deep-orange" />
                <h2 className="text-lg font-semibold">
                  Email
                </h2>
              </div>

              <div className="max-w-xl">
                <label className="mb-2 block text-sm text-gray-400">
                  Email address
                </label>

                <input
                  type="email"
                  value={email}
                  onChange={(e) => setEmail(e.target.value)}
                  className="w-full rounded-lg border border-white/10 bg-steel-gray px-4 py-3 outline-none transition focus:border-deep-orange"
                />

                <p className="mt-2 text-sm text-gray-400">
                  A confirmation link will be sent to your new email
                  address. Your current email will remain active until
                  the change is confirmed.
                </p>
              </div>

              <div className="mt-6 flex justify-end">
                <button
                  type="button"
                  className="rounded-lg bg-deep-orange px-5 py-2.5 font-medium text-white transition hover:opacity-90 cursor-pointer"
                >
                  Change email
                </button>
              </div>
            </section>

            <div className="border-t border-white/10" />

            {/* PASSWORD */}
            <section>
              <div className="mb-5 flex items-center gap-2">
                <LockKeyhole size={20} className="text-deep-orange" />
                <h2 className="text-lg font-semibold">
                  Password
                </h2>
              </div>

              <div className="grid grid-cols-1 gap-5 md:grid-cols-2">
                <div className="md:col-span-2">
                  <label className="mb-2 block text-sm text-gray-400">
                    Current password
                  </label>

                  <input
                    type="password"
                    value={currentPassword}
                    onChange={(e) => setCurrentPassword(e.target.value)}
                    autoComplete="current-password"
                    className="w-full max-w-md rounded-lg border border-white/10 bg-steel-gray px-4 py-3 outline-none transition focus:border-deep-orange"
                  />
                </div>

                <div>
                  <label className="mb-2 block text-sm text-gray-400">
                    New password
                  </label>

                  <input
                    type="password"
                    value={newPassword}
                    onChange={(e) => setNewPassword(e.target.value)}
                    autoComplete="new-password"
                    className={`w-full rounded-lg border bg-steel-gray px-4 py-3 outline-none transition 
                      ${!hasNewPassword ? "border-white/10 focus:border-deep-orange"
                        : isPasswordLongEnough
                          ?"border-green-500"
                          : "border-red-500"
                      }`}
                  />
                  <p className={`mt-2 text-sm transition-colors
                    ${!hasNewPassword ? "text-gray-400"
                      : isPasswordLongEnough
                        ? "text-green-400"
                        : "text-red-400"
                    }`}>
                      <b>Password must contain at lelast 8 characters.</b>
                  </p>
                </div>

                <div>
                  <label className="mb-2 block text-sm text-gray-400">
                    Confirm new password
                  </label>

                  <input
                    type="password"
                    value={confirmPassword}
                    onChange={(e) => setConfirmPassword(e.target.value)}
                    autoComplete="new-password"
                    aria-invalid={passwordsDoNotMatch}
                    className={`w-full rounded-lg border bg-steel-gray px-4 py-3 outline-none transition
                      ${passwordsDoNotMatch ? "border-red-500"
                        : passwordsMatch
                          ? "border-green-500"
                          : "border-white/10 focus:border-deep-orange"
                      }`}
                  />

                  {passwordsDoNotMatch && (
                    <p className="mt-2 text-sm text-red-400">
                      Passwords do not match.
                    </p>
                  )}

                  {passwordsMatch && (
                    <p className="mt-2 text-sm text-green-400">
                      Passwords match.
                    </p>
                  )}
                </div>
              </div>

              <div className="mt-6 flex justify-end">
                <button
                  type="button"
                  onClick={handleChangePassword}
                  disabled={isChangingPassword || !currentPassword || !isPasswordLongEnough || !passwordsMatch}
                  className="rounded-lg bg-deep-orange px-5 py-2.5 font-medium text-white transition hover:opacity-90 cursor-pointer disabled:cursor-not-allowed disabled:opacity-50"
                >
                  {isChangingPassword ? "Changing..." : "Change password"}
                </button>
              </div>
            </section>
          </div>

          {profileAlert && (
            <div
              role="alert"
              className={`fixed bottom-6 right-6 z-[9999] flex min-w-[320px] max-w-[420px] items-center gap-3 rounded-lg border px-4 py-3 shadow-xl
                ${
                  profileAlert.type === "success"
                    ? "border-green-500/30 bg-green-950 text-green-400"
                    : "border-red-500/30 bg-red-950 text-red-400"
                }
              `}
            >
              {profileAlert.type === "success" ? (
                <CircleCheck size={20} className="shrink-0" />
              ) : (
                <CircleX size={20} className="shrink-0" />
              )}

              <span className="text-sm font-medium">
                {profileAlert.message}
              </span>
            </div>
          )}
        </>
      );
}