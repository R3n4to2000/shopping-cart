export type Product = {
  id: number
  description: string
  availableStock: number
  netPrice: number
}

export type Coupon = {
  id: number
  code: string
  discountPercentage: number
}

export type CartItem = {
  id: string
  productId: number
  productDescription: string
  quantity: number
  availableStock: number
  netUnitPrice: number
  totalPrice: number
}

export type CartStatus = 'Open' | 'Finalized' | string

export type Cart = {
  id: string
  status: CartStatus
  items: CartItem[]
  appliedCoupon: Coupon | null
  subtotal: number
  discount: number
  total: number
}

export type ProblemDetails = {
  title?: string
  status?: number
  detail?: string
  instance?: string
  code?: string
  traceId?: string
  errors?: Record<string, string[]>
}
