import Logo from "@/components/ui/Logo";
import SearchBar from "@/components/ui/SearchBar";
import { User, Heart, ShoppingCart } from "@/components/icons/icons";
import LanguageDropdown from "@/components/ui/LanguageDropdown";
import LocationModal from "@/components/ui/CodeZip";

export default function Header() {
  return (
      <header className="fixed w-full h-20 bg-charcoal-dark px-4 shadow-md border-b border-black flex justify-start">
        <div className="max-w-7xl flex items-center justify-between gap-8 ml-14">
          <Logo width={170}/>
          <div className="flex items-center gap-10 ml-1.5">
            <div className="flex items-center">
              <LocationModal/>
            </div>

            <SearchBar/>

            <div className="ml-8 mr-8">
              <nav className="flex items-center gap-10 text-sm font-medium whitespace-nowrap">
                <div className="hover:text-deep-orange">
                  <a href="/login" className="flex items-center gap-1 transition flex-col">
                    <User size={30}/>
                    <span>Sign in</span>
                  </a>
                </div>
                
                <div className="hover:text-deep-orange">
                  <a href="/orders" className="flex items-center gap-1 hover:text-gray-300 transition flex-col">
                    <Heart size={30}/>
                    <span>Orders</span>
                  </a>
                </div>

                <div className="hover:text-deep-orange">
                  <a href="/cart" className="flex items-center gap-1 hover:text-gray-300 transition flex-col">
                    <ShoppingCart size={30}/>
                    <span>Cart</span>
                  </a>
                </div>
              </nav>
            </div>

            <div className="ml-8 mr-8">
              <LanguageDropdown />
            </div>

          </div>
        </div>
      </header>
  );
}