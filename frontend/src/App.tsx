import { CartPanel } from './components/CartPanel'
import { FeedbackMessage } from './components/FeedbackMessage'
import { ProductCatalog } from './components/ProductCatalog'
import { useShoppingCart } from './hooks/useShoppingCart'
import './App.css'

function App() {
  const {
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
    clearFeedback,
    reloadProducts,
  } = useShoppingCart()

  const cartActionsDisabled = cartLoading || !cart || isCartFinalized

  return (
    <main className="app-shell">
      <header className="app-header">
        <div>
          <span className="eyebrow">Teste técnico</span>
          <h1>Carrinho de Compras</h1>
        </div>
        {cart && (
          <div className="cart-reference" aria-label="Identificador do carrinho">
            Carrinho {cart.id.slice(0, 8)}
          </div>
        )}
      </header>

      <FeedbackMessage feedback={feedback} onClose={clearFeedback} />

      <div className="shopping-layout">
        <ProductCatalog
          products={products}
          loading={productsLoading}
          error={productsError}
          addingProductIds={pending.addingProductIds}
          disabled={cartActionsDisabled}
          onAddProduct={addProduct}
          onRetry={reloadProducts}
        />

        <CartPanel
          cart={cart}
          loading={cartLoading}
          disabled={cartActionsDisabled}
          isFinalized={isCartFinalized}
          updatingItemIds={pending.updatingItemIds}
          removingItemIds={pending.removingItemIds}
          couponLoading={pending.couponLoading}
          checkoutLoading={pending.checkoutLoading}
          newCartLoading={pending.newCartLoading}
          onChangeQuantity={changeQuantity}
          onRemoveProduct={removeProduct}
          onApplyCoupon={applyCoupon}
          onRemoveCoupon={removeCoupon}
          onCheckout={checkout}
          onCreateNewCart={createNewCart}
        />
      </div>
    </main>
  )
}

export default App
