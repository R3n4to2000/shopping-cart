import type { ProblemDetails } from '../types/api'

const apiUrl = import.meta.env.VITE_API_URL

const baseUrl =
  typeof apiUrl === 'string' && apiUrl.trim().length > 0
    ? apiUrl.trim().replace(/\/+$/, '')
    : ''

export class ApiError extends Error {
  readonly status?: number
  readonly code?: string

  constructor(message: string, status?: number, code?: string) {
    super(message)
    this.name = 'ApiError'
    this.status = status
    this.code = code
  }
}

type RequestOptions = {
  method?: 'GET' | 'POST' | 'PUT' | 'DELETE'
  body?: unknown
}

export async function request<T>(
  path: string,
  options: RequestOptions = {},
): Promise<T> {
  if (!baseUrl) {
    throw new ApiError(
      'Configure a variável VITE_API_URL para acessar a API.',
    )
  }

  const headers = new Headers()

  if (options.body !== undefined) {
    headers.set('Content-Type', 'application/json')
  }

  let response: Response

  try {
    response = await fetch(`${baseUrl}${path}`, {
      method: options.method ?? 'GET',
      headers,
      body:
        options.body === undefined ? undefined : JSON.stringify(options.body),
    })
  } catch {
    throw new ApiError(
      'Não foi possível acessar a API. Verifique se o backend está em execução.',
    )
  }

  const data = await parseJson(response)

  if (!response.ok) {
    throw toApiError(response, data)
  }

  return data as T
}

async function parseJson(response: Response): Promise<unknown> {
  const text = await response.text()

  if (!text) {
    return null
  }

  try {
    return JSON.parse(text) as unknown
  } catch {
    return null
  }
}

function toApiError(response: Response, data: unknown): ApiError {
  const problem = isProblemDetails(data) ? data : null
  const validationMessages = problem?.errors
    ? Object.values(problem.errors).flat()
    : []

  const message =
    validationMessages.length > 0
      ? validationMessages.join(' ')
      : problem?.detail ??
        problem?.title ??
        'Não foi possível concluir a solicitação.'

  return new ApiError(message, response.status, problem?.code)
}

function isProblemDetails(data: unknown): data is ProblemDetails {
  if (typeof data !== 'object' || data === null) {
    return false
  }

  return 'title' in data || 'detail' in data || 'errors' in data
}
