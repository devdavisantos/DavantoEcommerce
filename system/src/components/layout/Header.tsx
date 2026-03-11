import Logo from "@/components/ui/Logo";

export default function Header() {
  return (
      <header className="w-full bg-[#131921] text-white py-3 px-4 shadow-md border-b border-black">
        <div className="max-w-7xl mx-auto flex items-center justify-between gap-6">

          <Logo width={170}/>

          <form className="flex-1 max-w-3xl">
            <div className="flex w-full">
              <input
                type="text"
                placeholder="Buscar produtos..."
                name="q"
                className="w-full bg-gray-100 border border-gray-300 rounded-l-md text-black px-4 py-2 focus:outline-none"
              />

              <button
                type="submit"
                className="bg-blue-600 hover:bg-blue-700 px-4 py-2 rounded-r-md flex items-center justify-center"
              >
                <span className="material-symbols-outlined text-white">search</span>
              </button>
            </div>
          </form>

          <nav className="flex items-center gap-6 text-sm font-medium whitespace-nowrap">
            <a href="/login" className="flex items-center gap-1 hover:text-gray-300 transition">
              <span className="material-symbols-outlined text-lg">account_circle</span>
              <span>Entrar</span>
            </a>
          
            <a href="/orders" className="hover:text-gray-300 transition">Pedidos</a>
          
            <div className="relative">
              <a href="/cart" className="flex items-center gap-1 hover:text-gray-300 transition">
                <span className="material-symbols-outlined text-lg">shopping_cart</span>
                <span>Carrinho</span>
              </a>
            </div>
          </nav>
          
        </div>
      </header>
  );
}