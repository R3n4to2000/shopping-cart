type Feedback = {
  type: 'success' | 'error'
  message: string
}

type FeedbackMessageProps = {
  feedback: Feedback | null
  onClose: () => void
}

export function FeedbackMessage({
  feedback,
  onClose,
}: FeedbackMessageProps) {
  if (!feedback) {
    return null
  }

  return (
    <div
      className={`feedback feedback--${feedback.type}`}
      role={feedback.type === 'error' ? 'alert' : 'status'}
      aria-live={feedback.type === 'error' ? 'assertive' : 'polite'}
    >
      <p>{feedback.message}</p>
      <button type="button" className="button button--ghost" onClick={onClose}>
        Fechar
      </button>
    </div>
  )
}
