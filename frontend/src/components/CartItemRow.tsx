import { useMemo, useState } from 'react'
import type { FormEvent } from 'react'
import type { CartItem } from '../types/api'
import { formatCurrency } from '../utils/currency'

type CartItemRowProps = {
  item: CartItem
  disabled: boolean
  isUpdating: boolean
  isRemoving: boolean
  onChangeQuantity: (productId: number, quantity: number) => void
  onRemove: (productId: number) => void
}

export function CartItemRow({
  item,
  disabled,
  isUpdating,
  isRemoving,
  onChangeQuantity,
  onRemove,
}: CartItemRowProps) {
  const [draftQuantity, setDraftQuantity] = useState(String(item.quantity))
  const inputId = `quantity-${item.productId}`

  const parsedQuantity = useMemo(
    () => Number(draftQuantity),
    [draftQuantity],
  )
  const isQuantityValid =
    Number.isInteger(parsedQuantity) &&
    parsedQuantity >= 1 &&
    parsedQuantity <= item.availableStock
  const hasQuantityChanged = parsedQuantity !== item.quantity
  const controlsDisabled = disabled || isUpdating || isRemoving

  function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()

    if (isQuantityValid && hasQuantityChanged) {
      onChangeQuantity(item.productId, parsedQuantity)
    }
  }

  return (
    <li className="cart-item">
      <div className="cart-item__details">
        <h3>{item.productDescription}</h3>
        <dl>
          <div>
            <dt>Preço unitário</dt>
            <dd>{formatCurrency(item.netUnitPrice)}</dd>
          </div>
          <div>
            <dt>Total do item</dt>
            <dd>{formatCurrency(item.totalPrice)}</dd>
          </div>
          <div>
            <dt>Estoque</dt>
            <dd>{item.availableStock} unidade(s)</dd>
          </div>
        </dl>
      </div>

      <form className="cart-item__actions" onSubmit={handleSubmit}>
        <label htmlFor={inputId}>Quantidade</label>
        <div className="quantity-control">
          <input
            id={inputId}
            type="number"
            min={1}
            max={item.availableStock}
            value={draftQuantity}
            disabled={controlsDisabled}
            onChange={(event) => setDraftQuantity(event.target.value)}
          />
          <button
            type="submit"
            className="button"
            disabled={
              controlsDisabled || !isQuantityValid || !hasQuantityChanged
            }
          >
            {isUpdating ? 'Atualizando...' : 'Atualizar'}
          </button>
        </div>
        <button
          type="button"
          className="button button--danger"
          disabled={controlsDisabled}
          onClick={() => onRemove(item.productId)}
        >
          {isRemoving ? 'Removendo...' : 'Remover'}
        </button>
      </form>
    </li>
  )
}
