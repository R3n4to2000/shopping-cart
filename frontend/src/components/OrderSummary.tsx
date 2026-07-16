import type { Cart } from '../types/api'
import { formatCurrency } from '../utils/currency'

type OrderSummaryProps = {
  cart: Cart
}

export function OrderSummary({ cart }: OrderSummaryProps) {
  return (
    <section className="summary" aria-labelledby="summary-title">
      <h3 id="summary-title">Resumo financeiro</h3>
      <dl>
        <div>
          <dt>Subtotal</dt>
          <dd>{formatCurrency(cart.subtotal)}</dd>
        </div>
        <div>
          <dt>Desconto</dt>
          <dd>{formatCurrency(cart.discount)}</dd>
        </div>
        <div className="summary__total">
          <dt>Total</dt>
          <dd>{formatCurrency(cart.total)}</dd>
        </div>
      </dl>
    </section>
  )
}
