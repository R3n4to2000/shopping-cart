import { useCallback, useEffect, useMemo, useState } from 'react'
import type { Dispatch, SetStateAction } from 'react'
import {
  addCartItem,
  applyCoupon as applyCouponRequest,
  checkoutCart,
  createCart,
  getCart,
  listProducts,
  removeCartItem,
  removeCoupon as removeCouponRequest,
  updateCartItem,
} from '../api/shoppingCartApi'
import { ApiError } from '../api/httpClient'
import type { Cart, Product } from '../types/api'

const cartStorageKey = 'shopping-cart-id'

type Feedback = {
  type: 'success' | 'error'
  message: string
}

let startupCartPromise: Promise<Cart> | null = null

export function useShoppingCart() {
  const [products, setProducts] = useState<Product[]>([])
  const [productsLoading, setProductsLoading] = useState(true)
  const [productsError, setProductsError] = useState<string | null>(null)
  const [cart, setCart] = useState<Cart | null>(null)
  const [cartLoading, setCartLoading] = useState(true)
  const [feedback, setFeedback] = useState<Feedback | null>(null)
  const [addingProductIds, setAddingProductIds] = useState<Set<number>>(
    () => new Set(),
  )
  const [updatingItemIds, setUpdatingItemIds] = useState<Set<number>>(
    () => new Set(),
  )
  const [removingItemIds, setRemovingItemIds] = useState<Set<number>>(
    () => new Set(),
  )
  const [couponLoading, setCouponLoading] = useState(false)
  const [checkoutLoading, setCheckoutLoading] = useState(false)
  const [newCartLoading, setNewCartLoading] = useState(false)

  const isCartFinalized = cart?.status === 'Finalized'

  const loadProducts = useCallback(async () => {
    setProductsLoading(true)
    setProductsError(null)

    try {
      const catalog = await listProducts()
      setProducts(catalog)
    } catch (error) {
      setProductsError(getErrorMessage(error))
    } finally {
      setProductsLoading(false)
    }
  }, [])

  useEffect(() => {
    let isMounted = true

    listProducts()
      .then((catalog) => {
        if (isMounted) {
          setProducts(catalog)
        }
      })
      .catch((error: unknown) => {
        if (isMounted) {
          setProductsError(getErrorMessage(error))
        }
      })
      .finally(() => {
        if (isMounted) {
          setProductsLoading(false)
        }
      })

    return () => {
      isMounted = false
    }
  }, [])

  useEffect(() => {
    let isMounted = true

    loadOrCreateCart()
      .then((loadedCart) => {
        if (isMounted) {
          setCart(loadedCart)
        }
      })
      .catch((error: unknown) => {
        if (isMounted) {
          setFeedback({
            type: 'error',
            message: getErrorMessage(error),
          })
        }
      })
      .finally(() => {
        if (isMounted) {
          setCartLoading(false)
        }
      })

    return () => {
      isMounted = false
    }
  }, [])

  const addProduct = useCallback(
    async (productId: number) => {
      if (!cart || isCartFinalized) {
        return
      }

      markPending(setAddingProductIds, productId, true)

      try {
        const updatedCart = await addCartItem(cart.id, productId, 1)
        setCart(updatedCart)
        setFeedback({
          type: 'success',
          message: 'Produto adicionado ao carrinho.',
        })
      } catch (error) {
        setFeedback({
          type: 'error',
          message: getErrorMessage(error),
        })
      } finally {
        markPending(setAddingProductIds, productId, false)
      }
    },
    [cart, isCartFinalized],
  )

  const changeQuantity = useCallback(
    async (productId: number, quantity: number) => {
      if (!cart || isCartFinalized) {
        return
      }

      markPending(setUpdatingItemIds, productId, true)

      try {
        const updatedCart = await updateCartItem(cart.id, productId, quantity)
        setCart(updatedCart)
        setFeedback({
          type: 'success',
          message: 'Quantidade atualizada.',
        })
      } catch (error) {
        setFeedback({
          type: 'error',
          message: getErrorMessage(error),
        })
      } finally {
        markPending(setUpdatingItemIds, productId, false)
      }
    },
    [cart, isCartFinalized],
  )

  const removeProduct = useCallback(
    async (productId: number) => {
      if (!cart || isCartFinalized) {
        return
      }

      markPending(setRemovingItemIds, productId, true)

      try {
        const updatedCart = await removeCartItem(cart.id, productId)
        setCart(updatedCart)
        setFeedback({
          type: 'success',
          message: 'Produto removido do carrinho.',
        })
      } catch (error) {
        setFeedback({
          type: 'error',
          message: getErrorMessage(error),
        })
      } finally {
        markPending(setRemovingItemIds, productId, false)
      }
    },
    [cart, isCartFinalized],
  )

  const applyCoupon = useCallback(
    async (code: string) => {
      if (!cart || isCartFinalized) {
        return
      }

      setCouponLoading(true)

      try {
        const updatedCart = await applyCouponRequest(cart.id, code)
        setCart(updatedCart)
        setFeedback({
          type: 'success',
          message: 'Cupom aplicado.',
        })
      } catch (error) {
        setFeedback({
          type: 'error',
          message: getErrorMessage(error),
        })
      } finally {
        setCouponLoading(false)
      }
    },
    [cart, isCartFinalized],
  )

  const removeCoupon = useCallback(async () => {
    if (!cart || isCartFinalized) {
      return
    }

    setCouponLoading(true)

    try {
      const updatedCart = await removeCouponRequest(cart.id)
      setCart(updatedCart)
      setFeedback({
        type: 'success',
        message: 'Cupom removido.',
      })
    } catch (error) {
      setFeedback({
        type: 'error',
        message: getErrorMessage(error),
      })
    } finally {
      setCouponLoading(false)
    }
  }, [cart, isCartFinalized])

  const checkout = useCallback(async () => {
    if (!cart || isCartFinalized) {
      return
    }

    setCheckoutLoading(true)

    try {
      const updatedCart = await checkoutCart(cart.id)
      setCart(updatedCart)
      setFeedback({
        type: 'success',
        message: 'Compra finalizada com sucesso.',
      })
      void loadProducts()
    } catch (error) {
      setFeedback({
        type: 'error',
        message: getErrorMessage(error),
      })
    } finally {
      setCheckoutLoading(false)
    }
  }, [cart, isCartFinalized, loadProducts])

  const createNewCart = useCallback(async () => {
    setNewCartLoading(true)

    try {
      const newCart = await createCart()
      storeCartId(newCart.id)
      setCart(newCart)
      setFeedback({
        type: 'success',
        message: 'Novo carrinho criado.',
      })
    } catch (error) {
      setFeedback({
        type: 'error',
        message: getErrorMessage(error),
      })
    } finally {
      setNewCartLoading(false)
    }
  }, [])

  const pending = useMemo(
    () => ({
      addingProductIds,
      updatingItemIds,
      removingItemIds,
      couponLoading,
      checkoutLoading,
      newCartLoading,
    }),
    [
      addingProductIds,
      checkoutLoading,
      couponLoading,
      newCartLoading,
      removingItemIds,
      updatingItemIds,
    ],
  )

  return {
    products,
    productsLoading,
    productsError,
    cart,
    cartLoading,
    feedback,
    pending,
    isCartFinalized,
    addProduct,
    changeQuantity,
    removeProduct,
    applyCoupon,
    removeCoupon,
    checkout,
    createNewCart,
    clearFeedback: () => setFeedback(null),
    reloadProducts: loadProducts,
  }
}

async function loadOrCreateCart(): Promise<Cart> {
  if (startupCartPromise) {
    return startupCartPromise
  }

  startupCartPromise = initializeCart()

  try {
    return await startupCartPromise
  } finally {
    startupCartPromise = null
  }
}

async function initializeCart(): Promise<Cart> {
  const storedCartId = readStoredCartId()

  if (storedCartId) {
    try {
      return await getCart(storedCartId)
    } catch (error) {
      if (error instanceof ApiError && error.status === 404) {
        removeStoredCartId()
      } else {
        throw error
      }
    }
  }

  const cart = await createCart()
  storeCartId(cart.id)

  return cart
}

function markPending(
  setPending: Dispatch<SetStateAction<Set<number>>>,
  id: number,
  isPending: boolean,
) {
  setPending((current) => {
    const next = new Set(current)

    if (isPending) {
      next.add(id)
    } else {
      next.delete(id)
    }

    return next
  })
}

function readStoredCartId(): string | null {
  try {
    return window.localStorage.getItem(cartStorageKey)
  } catch {
    return null
  }
}

function storeCartId(cartId: string) {
  try {
    window.localStorage.setItem(cartStorageKey, cartId)
  } catch {
    return
  }
}

function removeStoredCartId() {
  try {
    window.localStorage.removeItem(cartStorageKey)
  } catch {
    return
  }
}

function getErrorMessage(error: unknown): string {
  if (error instanceof Error && error.message) {
    return error.message
  }

  return 'Ocorreu um erro inesperado.'
}
