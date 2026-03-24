import Logo from "@/components/ui/Logo";
import { Mail, Phone } from "@/components/icons/icons"

export default function Footer() {
    return (
      <footer className="w-full bg-charcoal-dark text-gray-300 border-t border-gray-800 mt-6">
        <div className="max-w-7xl mx-auto px-6 py-7">
          
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
            <div>
              <Logo/>
              <p className="text-xs text-gray-400 mb-3 mt-3">
                Your auto parts store with the best brands and prices on the market.
              </p>
            </div>
          
            <div>
              <h3 className="text-deep-orange font-semibold mb-3 text-sm">COMPANY</h3>
              <ul className="space-y-1 text-sm">
                <li className="hover:text-white cursor-pointer">About Us</li>
                <li className="hover:text-white cursor-pointer">Privacy Policy</li>
                <li className="hover:text-white cursor-pointer">Terms of Use</li>
                <li className="hover:text-white cursor-pointer">Careers</li>
              </ul>
            </div>
          
            <div>
              <h3 className="text-deep-orange font-semibold mb-3 text-sm">SUPPORT</h3>
              <ul className="space-y-1 text-xs">
                <li className="hover:text-white cursor-pointer">Help Center</li>
                <li className="hover:text-white cursor-pointer">Returns & Exchanges</li>
                <li className="hover:text-white cursor-pointer">Shipping Information</li>
                <li className="hover:text-white cursor-pointer">Payment Methods</li>
              </ul>
            </div>
          
            <div>
              <h3 className="text-deep-orange font-semibold mb-3 text-sm">CONTACT</h3>
              <ul className="space-y-2 text-xs">
                <li className="flex gap-2 items-center">
                  <Phone className="text-deep-orange size-4"/>
                  (11) 4002-8922
                </li>
                <li className="flex gap-2 items-center">
                  <Mail className="text-deep-orange size-4"/>
                  contact@lastore.com
                </li>
              </ul>
            </div>
          </div>
          
          <div className="border-t border-gray-800 mt-6 pt-4 text-center text-xs text-gray-500">
            © {new Date().getFullYear()} LaStore. All Rights Reserved.
          </div>
          
        </div>
      </footer>
    );
}