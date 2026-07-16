import type { Product } from '../types/api'
import { formatCurrency } from '../utils/currency'

type ProductCardProps = {
  product: Product
  isAdding: boolean
  disabled: boolean
  onAdd: (productId: number) => void
}

export function ProductCard({
  product,
  isAdding,
  disabled,
  onAdd,
}: ProductCardProps) {
  const isOutOfStock = product.availableStock <= 0
  const actionDisabled = disabled || isAdding || isOutOfStock

  return (
    <article className="product-card">
      <div>
        <h3>{product.description}</h3>
        <p className="product-card__price">
          {formatCurrency(product.netPrice)}
        </p>
      </div>

      <p
        className={
          isOutOfStock
            ? 'product-card__stock product-card__stock--empty'
            : 'product-card__stock'
        }
      >
        {isOutOfStock
          ? 'Sem estoque'
          : `${product.availableStock} unidade(s) em estoque`}
      </p>

      <button
        type="button"
        className="button button--primary"
        disabled={actionDisabled}
        onClick={() => onAdd(product.id)}
        aria-label={`Adicionar ${product.description} ao carrinho`}
      >
        {isAdding ? 'Adicionando...' : 'Adicionar ao carrinho'}
      </button>
    </article>
  )
}
