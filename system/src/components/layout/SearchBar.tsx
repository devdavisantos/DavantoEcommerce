import { Search, ChevronDown, ChevronUp } from "@/components/icons/icons";
import { Form, Frown } from "lucide-react";

export default function SearchBar() {
  return (
    <div className="flex-1 max-w-4xl w-700">
      <div className="flex w-full h-11">
        <button className="bg-steel-gray rounded-l-md w-21 cursor-pointer">
            <span className="flex items-center justify-center gap-0.5 text-gray-400 hover:text-white">
                All <ChevronDown size={17}/>
            </span>
        </button>
        <form className="flex-1 max-w-3xl flex">
            <input
              type="text"
              placeholder="Search..."
              name="s"
              className="w-full bg-charcoal-gray border border-charcoal-gray text-gray-200 focus:outline-none p-3"
            />
            <button
              type="submit"
              className="bg-amber-primary hover:bg-amber-secondary border border-black px-4 py-2 rounded-r-md flex items-center justify-center w-16 cursor-pointer"
            >
              <Search className="text-black"/>
            </button>
        </form>
      </div>
    </div>
  );
}