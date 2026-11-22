export class ApiError extends Error {
  constructor(message, status = 0, errors = {}) {
    super(message);
    this.name = 'ApiError';
    this.status = status;
    this.errors = errors;
  }
}

export function queryString(filters = {}) {
  const params = new URLSearchParams();
  for (const [key, value] of Object.entries(filters)) {
    if (value !== null && value !== undefined && String(value).trim() !== '') {
      params.set(key, String(value).trim());
    }
  }
  return params.size ? `?${params}` : '';
}

export function createApi(baseUrl = '/api', fetchImpl = globalThis.fetch, timeoutMs = 15000) {
  async function request(resource, { method = 'GET', data, signal } = {}) {
    const controller = new AbortController();
    const cancel = () => controller.abort();
    if (signal?.aborted) cancel();
    signal?.addEventListener('abort', cancel, { once: true });
    const timer = setTimeout(cancel, timeoutMs);
    try {
      const response = await fetchImpl(`${baseUrl}/${resource}`, {
        method,
        headers: { Accept: 'application/json', ...(data === undefined ? {} : { 'Content-Type': 'application/json' }) },
        ...(data === undefined ? {} : { body: JSON.stringify(data) }),
        signal: controller.signal,
        cache: 'no-store'
      });
      if (response.status === 204) return null;
      const text = await response.text();
      let payload;
      try { payload = text ? JSON.parse(text) : null; } catch { payload = null; }
      if (!response.ok) {
        const fallback = response.status === 404 ? 'O registro não foi encontrado. Atualize a lista e tente novamente.'
          : response.status === 409 ? 'Não foi possível concluir a operação por um conflito no cadastro.'
          : 'Não foi possível concluir a operação. Tente novamente.';
        throw new ApiError(payload?.mensagem || payload?.title || fallback, response.status, payload?.errors || {});
      }
      if (payload === null) throw new ApiError('O serviço retornou uma resposta inválida. Tente novamente.', response.status);
      return payload;
    } catch (error) {
      if (signal?.aborted) throw new DOMException('Consulta cancelada.', 'AbortError');
      if (error instanceof ApiError) throw error;
      if (controller.signal.aborted) throw new ApiError('O serviço demorou para responder. Tente novamente.');
      throw new ApiError('Não foi possível acessar o serviço. Verifique se a aplicação está em execução e tente novamente.');
    } finally {
      clearTimeout(timer);
      signal?.removeEventListener('abort', cancel);
    }
  }

  return {
    list: (resource, filters, signal) => request(resource + queryString(filters), { signal }),
    get: (resource, id, signal) => request(`${resource}/${id}`, { signal }),
    create: (resource, data) => request(resource, { method: 'POST', data }),
    update: (resource, id, data) => request(`${resource}/${id}`, { method: 'PUT', data }),
    remove: (resource, id) => request(`${resource}/${id}`, { method: 'DELETE' })
  };
}
