import type { Product } from '../types/api'
import { ProductCard } from './ProductCard'

type ProductCatalogProps = {
  products: Product[]
  loading: boolean
  error: string | null
  addingProductIds: Set<number>
  disabled: boolean
  onAddProduct: (productId: number) => void
  onRetry: () => void
}

export function ProductCatalog({
  products,
  loading,
  error,
  addingProductIds,
  disabled,
  onAddProduct,
  onRetry,
}: ProductCatalogProps) {
  return (
    <section className="catalog" aria-labelledby="catalog-title">
      <div className="section-heading">
        <div>
          <span className="eyebrow">Catálogo</span>
          <h2 id="catalog-title">Produtos disponíveis</h2>
        </div>
        <span className="counter-pill">{products.length} itens</span>
      </div>

      {loading && (
        <div className="surface-state" role="status" aria-live="polite">
          Carregando catálogo...
        </div>
      )}

      {!loading && error && (
        <div className="surface-state surface-state--error" role="alert">
          <p>{error}</p>
          <button type="button" className="button" onClick={onRetry}>
            Tentar novamente
          </button>
        </div>
      )}

      {!loading && !error && products.length === 0 && (
        <div className="surface-state">Nenhum produto disponível.</div>
      )}

      {!loading && !error && products.length > 0 && (
        <div className="product-grid" aria-busy={loading}>
          {products.map((product) => (
            <ProductCard
              key={product.id}
              product={product}
              isAdding={addingProductIds.has(product.id)}
              disabled={disabled}
              onAdd={onAddProduct}
            />
          ))}
        </div>
      )}
    </section>
  )
}
