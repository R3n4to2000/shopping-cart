import type { Cart } from '../types/api'
import { CartItemRow } from './CartItemRow'
import { CouponForm } from './CouponForm'
import { OrderSummary } from './OrderSummary'

type CartPanelProps = {
  cart: Cart | null
  loading: boolean
  disabled: boolean
  isFinalized: boolean
  updatingItemIds: Set<number>
  removingItemIds: Set<number>
  couponLoading: boolean
  checkoutLoading: boolean
  newCartLoading: boolean
  onChangeQuantity: (productId: number, quantity: number) => void
  onRemoveProduct: (productId: number) => void
  onApplyCoupon: (code: string) => void
  onRemoveCoupon: () => void
  onCheckout: () => void
  onCreateNewCart: () => void
}

export function CartPanel({
  cart,
  loading,
  disabled,
  isFinalized,
  updatingItemIds,
  removingItemIds,
  couponLoading,
  checkoutLoading,
  newCartLoading,
  onChangeQuantity,
  onRemoveProduct,
  onApplyCoupon,
  onRemoveCoupon,
  onCheckout,
  onCreateNewCart,
}: CartPanelProps) {
  function handleCheckout() {
    const confirmed = window.confirm('Deseja finalizar a compra?')

    if (confirmed) {
      onCheckout()
    }
  }

  return (
    <aside className="cart-panel" aria-labelledby="cart-title">
      <div className="section-heading section-heading--compact">
        <div>
          <span className="eyebrow">Carrinho</span>
          <h2 id="cart-title">Sua compra</h2>
        </div>
        {cart && (
          <span
            className={
              isFinalized
                ? 'status-pill status-pill--done'
                : 'status-pill'
            }
          >
            {isFinalized ? 'Finalizado' : 'Aberto'}
          </span>
        )}
      </div>

      {loading && (
        <div className="surface-state" role="status" aria-live="polite">
          Carregando carrinho...
        </div>
      )}

      {!loading && !cart && (
        <div className="surface-state">Preparando carrinho...</div>
      )}

      {!loading && cart && (
        <div className="cart-panel__content">
          {cart.items.length === 0 ? (
            <div className="cart-empty">
              O carrinho está vazio.
            </div>
          ) : (
            <ul className="cart-list">
              {cart.items.map((item) => (
                <CartItemRow
                  key={`${item.id}-${item.quantity}`}
                  item={item}
                  disabled={disabled}
                  isUpdating={updatingItemIds.has(item.productId)}
                  isRemoving={removingItemIds.has(item.productId)}
                  onChangeQuantity={onChangeQuantity}
                  onRemove={onRemoveProduct}
                />
              ))}
            </ul>
          )}

          <CouponForm
            key={cart.appliedCoupon?.code ?? 'sem-cupom'}
            appliedCoupon={cart.appliedCoupon}
            disabled={disabled}
            loading={couponLoading}
            onApply={onApplyCoupon}
            onRemove={onRemoveCoupon}
          />

          <OrderSummary cart={cart} />

          {isFinalized && (
            <div className="checkout-success" role="status">
              Compra finalizada. Crie um novo carrinho para continuar.
            </div>
          )}

          <div className="cart-panel__footer">
            <button
              type="button"
              className="button button--primary button--wide"
              disabled={disabled || checkoutLoading}
              onClick={handleCheckout}
            >
              {checkoutLoading ? 'Finalizando...' : 'Finalizar compra'}
            </button>

            {isFinalized && (
              <button
                type="button"
                className="button button--wide"
                disabled={newCartLoading}
                onClick={onCreateNewCart}
              >
                {newCartLoading ? 'Criando...' : 'Criar novo carrinho'}
              </button>
            )}
          </div>
        </div>
      )}
    </aside>
  )
}
