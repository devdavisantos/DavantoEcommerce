import Logo from "@/components/ui/Logo";
import { Mail, Phone } from "@/components/icons/icons"

export default function Footer() {
    return (
        <footer className="w-full [background:var(--headerground)] text-gray-300 border-t border-gray-800 mt-16">
          <div className="max-w-7xl mx-auto px-6 py-10">

            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-10">

              <div>
                <Logo/>
                <p className="text-sm text-gray-400 mb-4 mt-5">
                  Your auto parts store with the best brands and prices on the market.
                </p>
              </div>

              {/* Institucional */}
              <div>
                <h3 className="text-[var(--orangecolor)] font-semibold mb-4">COMPANY</h3>
                <ul className="space-y-2 text-sm">
                  <li className="hover:text-white cursor-pointer">About Us</li>
                  <li className="hover:text-white cursor-pointer">Privacy Policy</li>
                  <li className="hover:text-white cursor-pointer">Terms of Use</li>
                  <li className="hover:text-white cursor-pointer">Careers</li>
                </ul>
              </div>

              {/* Ajuda */}
              <div>
                <h3 className="text-[var(--orangecolor)] font-semibold mb-4">SUPPORT</h3>
                <ul className="space-y-2 text-sm">
                  <li className="hover:text-white cursor-pointer">Help Center</li>
                  <li className="hover:text-white cursor-pointer">Returns & Exchanges</li>
                  <li className="hover:text-white cursor-pointer">Shipping Information</li>
                  <li className="hover:text-white cursor-pointer">Payment Methods</li>
                </ul>
              </div>

              {/* Contato */}
              <div>
                <h3 className="text-[var(--orangecolor)] font-semibold mb-4">CONTACT</h3>
                <ul className="space-y-3 text-sm">
                  <li className="flex gap-2 items-center">
                    <Phone className="text-[var(--orangecolor)] size-4"/>
                    (11) 4002-8922
                  </li>
                  <li className="flex gap-2 items-center">
                    <Mail className="text-[var(--orangecolor)] size-4"/>
                    contact@autoparts.com
                    </li>
                </ul>
              </div>

            </div>

            {/* linha */}
            <div className="border-t border-gray-800 mt-10 pt-6 text-center text-sm text-gray-500">
              © {new Date().getFullYear()} LaStore. All Rights Reserved.
            </div>

          </div>
        </footer>
    );
}