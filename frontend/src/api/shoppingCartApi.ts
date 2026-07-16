import { request } from './httpClient'
import type { Cart, Product } from '../types/api'

export function listProducts(): Promise<Product[]> {
  return request<Product[]>('/api/products')
}

export function createCart(): Promise<Cart> {
  return request<Cart>('/api/carts', {
    method: 'POST',
  })
}

export function getCart(cartId: string): Promise<Cart> {
  return request<Cart>(`/api/carts/${cartId}`)
}

export function addCartItem(
  cartId: string,
  productId: number,
  quantity: number,
): Promise<Cart> {
  return request<Cart>(`/api/carts/${cartId}/items`, {
    method: 'POST',
    body: {
      productId,
      quantity,
    },
  })
}

export function updateCartItem(
  cartId: string,
  productId: number,
  quantity: number,
): Promise<Cart> {
  return request<Cart>(`/api/carts/${cartId}/items/${productId}`, {
    method: 'PUT',
    body: {
      quantity,
    },
  })
}

export function removeCartItem(
  cartId: string,
  productId: number,
): Promise<Cart> {
  return request<Cart>(`/api/carts/${cartId}/items/${productId}`, {
    method: 'DELETE',
  })
}

export function applyCoupon(cartId: string, code: string): Promise<Cart> {
  return request<Cart>(`/api/carts/${cartId}/coupon`, {
    method: 'PUT',
    body: {
      code,
    },
  })
}

export function removeCoupon(cartId: string): Promise<Cart> {
  return request<Cart>(`/api/carts/${cartId}/coupon`, {
    method: 'DELETE',
  })
}

export function checkoutCart(cartId: string): Promise<Cart> {
  return request<Cart>(`/api/carts/${cartId}/checkout`, {
    method: 'POST',
  })
}
