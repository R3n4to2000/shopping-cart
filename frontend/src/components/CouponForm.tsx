import { useState } from 'react'
import type { FormEvent } from 'react'
import type { Coupon } from '../types/api'

type CouponFormProps = {
  appliedCoupon: Coupon | null
  disabled: boolean
  loading: boolean
  onApply: (code: string) => void
  onRemove: () => void
}

export function CouponForm({
  appliedCoupon,
  disabled,
  loading,
  onApply,
  onRemove,
}: CouponFormProps) {
  const [code, setCode] = useState(appliedCoupon?.code ?? '')

  function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()
    onApply(code)
  }

  return (
    <section className="coupon" aria-labelledby="coupon-title">
      <h3 id="coupon-title">Cupom</h3>

      <form className="coupon__form" onSubmit={handleSubmit}>
        <label htmlFor="coupon-code">Código do cupom</label>
        <div className="coupon__controls">
          <input
            id="coupon-code"
            type="text"
            maxLength={30}
            required
            value={code}
            disabled={disabled || loading}
            onChange={(event) => setCode(event.target.value)}
            placeholder="10off"
          />
          <button
            type="submit"
            className="button"
            disabled={disabled || loading || code.length === 0}
          >
            {loading ? 'Aplicando...' : 'Aplicar cupom'}
          </button>
        </div>
      </form>

      {appliedCoupon && (
        <div className="coupon__applied">
          <p>
            Cupom aplicado: <strong>{appliedCoupon.code}</strong> (
            {appliedCoupon.discountPercentage.toLocaleString('pt-BR')}%)
          </p>
          <button
            type="button"
            className="button button--ghost"
            disabled={disabled || loading}
            onClick={onRemove}
          >
            Remover cupom
          </button>
        </div>
      )}
    </section>
  )
}
