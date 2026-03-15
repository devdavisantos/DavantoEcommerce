import {
  Search as SearchIcon,
  User as UserIcon,
  Heart as HeartIcon,
  ShoppingCart as CartIcon,
  MapPin as MapPinIcon,
  ChevronDown as ChevronDownIcon,
  ChevronUp as ChevronUpIcon,
  Menu as MenuIcon,
  Mail as MailIcon,
  Phone as PhoneIcon
} from "lucide-react"

const defaultProps = {
  size: 22,
  strokeWidth: 2
}

export const Search = (props: any) => (
  <SearchIcon strokeWidth={2.5} {...props} />
)

export const User = (props: any) => (
  <UserIcon {...defaultProps} {...props} />
)

export const Heart = (props: any) => (
  <HeartIcon {...defaultProps} {...props} />
)

export const ShoppingCart = (props: any) => (
  <CartIcon {...defaultProps} {...props} />
)

export const MapPin = (props: any) => (
  <MapPinIcon {...defaultProps} {...props} />
)

export const ChevronDown = (props: any) => (
  <ChevronDownIcon {...defaultProps} {...props} />
)

export const ChevronUp = (props: any) => (
  <ChevronUpIcon {...defaultProps} {...props} />
)

export const Menu = (props: any) => (
  <MenuIcon {...defaultProps} {...props} />
)

export const Mail = (props: any) => (
  <MailIcon {...defaultProps} {...props} />
)

export const Phone = (props: any) => (
  <PhoneIcon {...defaultProps} {...props} />
)