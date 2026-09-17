// import Link from "next/link";
// import Logo from "@/components/ui/Logo";
// import SearchBar from "@/components/layout/SearchBar";
// import { User, Heart, ShoppingCart } from "@/components/icons/icons";
// import LanguageDropdown from "@/components/ui/LanguageDropdown";
// import LocationModal from "@/components/ui/CodeZip";

// export default function Header() {
//   return (
//       <header className="fixed w-full h-20 bg-charcoal-dark px-4 shadow-md border-b border-black flex justify-start">
//         <div className="max-w-7xl flex items-center justify-between gap-8 ml-14">
          
//           <Link href="/" className="contents">
//             <Logo width={170} />
//           </Link>
          
//           <div className="flex items-center gap-10 ml-1.5">
//             <div className="flex items-center">
//               <LocationModal/>
//             </div>

//             <SearchBar/>

//             <div className="ml-8 mr-8">
//               <nav className="flex items-center gap-10 text-sm font-medium whitespace-nowrap">
                
//                 <div className="hover:text-deep-orange">
//                   <a href="/login" className="flex items-center gap-1 transition flex-col">
//                     <User size={30}/>
//                     <span>Sign in</span>
//                   </a>
//                 </div>
                
//                 <div className="hover:text-deep-orange">
//                   <a href="/orders" className="flex items-center gap-1 hover:text-gray-300 transition flex-col">
//                     <Heart size={30}/>
//                     <span>Orders</span>
//                   </a>
//                 </div>

//                 <div className="hover:text-deep-orange">
//                   <a href="/cart" className="flex items-center gap-1 hover:text-gray-300 transition flex-col">
//                     <ShoppingCart size={30}/>
//                     <span>Cart</span>
//                   </a>
                
//                 </div>
//               </nav>
//             </div>

//             {/* <div className="ml-8 mr-8">
//               <LanguageDropdown />
//             </div> */}

//           </div>
//         </div>
//       </header>
//   );
// }

"use client";

import { useState } from "react";
import Link from "next/link";
import Logo from "@/components/ui/Logo";
import SearchBar from "@/components/layout/SearchBar";
import { User, Heart, ShoppingCart, ChevronDown, CircleUserRound } from "@/components/icons/icons";
import LanguageDropdown from "@/components/ui/LanguageDropdown";
import LocationModal from "@/components/ui/CodeZip";
import LoginModal from "@/components/auth/LoginModal";
import { useAuth } from "@/contexts/AuthContext";
import AccountMenu from "@/components/account/AccountMenu";
import WishlistMenu from "@/components/account/WishlistMenu";
import CartMenu from "../account/CartMenu";

type HeaderProps = {
  fixed?: boolean;
};

export default function Header({
  fixed = true,
}: HeaderProps) {
  const [isLoginModalOpen, setIsLoginModalOpen] = useState(false);
  const { user, isAuthenticated, isLoading } = useAuth();

  const firstName = typeof user?.name === "string" ? user.name.trim().split(" ")[0] : "";

  return (
    <>
      <header className={`${fixed ? "fixed top-0 leftt-0 z-50" : "relative"} fixed w-full h-20 bg-charcoal-dark px-4 shadow-md border-b border-black flex justify-start`}>
        <div className="max-w-7xl flex items-center justify-between gap-8 ml-14">
          <Link href="/" className="contents">
            <Logo width={170} />
          </Link>

          <div className="flex items-center gap-10 ml-1.5">
            <div className="flex items-center">
              <LocationModal />
            </div>

            <SearchBar />

            <div className="ml-8 mr-8">
              <nav className="flex items-center gap-10 text-sm font-medium whitespace-nowrap">
                {isLoading ? (
                  <div className="hover:text-deep-orange">
                    <span>&nbsp;</span>
                  </div>
                  ) : isAuthenticated && user ? (
                  <div className="relative group">
                      <button
                        type="button"
                        className="flex items-center justify-center gap-2 px-2 py-1.5 rounded-lg transition-colors cursor-pointer hover:bg-white/5"
                      >
                        <CircleUserRound size={30} className="transition-colors group-hover:text-deep-orange"/>
                        <div className="flex flex-col items-start">
                          <span className="text-sm leading-none text-white">Welcome, </span>

                          <div className="flex items-center gap-1 mt-1">
                            <span className="font-bold leading-none">{firstName}</span>
                            <ChevronDown size={13} className="mt-[2px] transition-transform group-hover:rotate-180"/>
                          </div>
                        </div>
                      </button>

                      <div className="absolute left-1/2 top-full -translate-x-1/2 pt-3 z-[9999] invisible opacity-0 pointer-events-none
                      group-hover:visible group-hover:opacity-100 group-hover:pointer-events-auto transition-opacity duration-150">
                        <AccountMenu/>
                      </div>
                  </div>
                  ) : (
                  <button
                    type="button"
                    onClick={() => setIsLoginModalOpen(true)}
                    className="group/signin h-12 flex items-center justify-center gap-2 px-2 py-1.5 rounded-lg transition-colors cursor-pointer hover:bg-white/5 hover:text-deep-orange"
                  >
                    <User size={30} className="transition-transform duration-150 group-hover/signin:scale-110"/>
                    <span className="text-sm leading-none text-white ">Sign in</span>
                  </button>
                )}

                <div className="relative group/wishlist">
                  <a
                    href="/wishlist"
                    className="flex items-center justify-center gap-1 px-2 py-3 rounded-lg transition-colors cursor-pointer hover:bg-white/5 hover:!text-deep-orange"
                  >
                    <Heart size={24} className="mr-1.5 transition-transform duration-150 group-hover/wishlist:scale-110"/>
                    <div className="flex items-center justify-center">
                      <span className="text-sm leading-none text-white">Wishlist</span>
                      <ChevronDown size={13} className="mt-[2px] ml-1 transition-transform group-hover/wishlist:rotate-180"/>
                    </div>
                  </a>

                  <div
                    className="absolute left-1/2 top-full -translate-x-1/2 pt-3 z-[9999] invisible opacity-0 pointer-events-none group-hover/wishlist:visible group-hover/wishlist:opacity-100 group-hover/wishlist:pointer-events-auto transition-opacity duration-150"
                  >
                    <WishlistMenu isAuthenticated={isAuthenticated} />
                  </div>
                </div>

                <div className="relative group/cart">
                 <a
                   href="/cart"
                   className="flex flex-col items-center justify-center px-2 py-2.5 rounded-lg transition-colors cursor-pointer hover:bg-white/5 hover:!text-deep-orange"
                 >
                  <div className="flex items-center justify-center">
                    <div className="relative flex items-center justify-center">
                      <ShoppingCart size={30} />
                      <span className="absolute -top-2 -right-3 flex min-w-5 h-5 items-center justify-center px-1 text-xs font-bold leading-none text-white bg-deep-orange rounded-full">
                        3
                      </span>
                    </div>
                      
                    <ChevronDown size={13} className="ml-2 mt-3 transition-transform group-hover/cart:rotate-180"/>
                   
                  </div>
                 </a>

                 <div
                   className="absolute top-full right-0 pt-3 z-[9999] invisible opacity-0 pointer-events-none group-hover/cart:visible group-hover/cart:opacity-100 group-hover/cart:pointer-events-auto transition-opacity duration-150"
                 >
                   <CartMenu isAuthenticated={isAuthenticated} />
                 </div>
                </div>

              </nav>
            </div>

            {/* 
            <div className="ml-8 mr-8">
              <LanguageDropdown />
            </div> 
            */}
            
          </div>
        </div>
      </header>

      <LoginModal
        isOpen={isLoginModalOpen}
        onClose={() => setIsLoginModalOpen(false)}
      />
    </>
  );
}