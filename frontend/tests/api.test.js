import test from 'node:test';
import assert from 'node:assert/strict';
import { createApi, ApiError, queryString } from '../js/api.js';

test('filtros preservam acentos, zero e caracteres especiais sem parâmetros vazios', () => {
  assert.equal(queryString({ nome: '  A&B  ', paisOrigem: 'Japão', diariaMin: 0, diariaMax: '', nada: null }), '?nome=A%26B&paisOrigem=Jap%C3%A3o&diariaMin=0');
});

test('GET envia os filtros combinados à API, sem corpo', async () => {
  let captured;
  const api = createApi('/api', async (url, options) => {
    captured = { url, options };
    return Response.json([{ id: 1, nome: 'Toyota', paisOrigem: 'Japão' }]);
  });
  const rows = await api.list('fabricantes', { nome: 'Toyota', paisOrigem: 'Japão' });
  assert.equal(captured.url, '/api/fabricantes?nome=Toyota&paisOrigem=Jap%C3%A3o');
  assert.equal(captured.options.method, 'GET');
  assert.equal(captured.options.body, undefined);
  assert.equal(captured.options.cache, 'no-store');
  assert.equal(rows[0].id, 1);
});

test('POST e PUT enviam JSON; DELETE e PUT aceitam 204 sem tentar ler JSON', async () => {
  const calls = [];
  const api = createApi('/api', async (url, options) => {
    calls.push({ url, ...options });
    return options.method === 'POST' ? Response.json({ id: 8 }, { status: 201 }) : new Response(null, { status: 204 });
  });
  const data = { nome: 'Econômico', descricao: '', valorDiariaBase: 120.5 };
  assert.equal((await api.create('categorias', data)).id, 8);
  assert.equal(await api.update('categorias', 8, data), null);
  assert.equal(await api.remove('categorias', 8), null);
  assert.deepEqual(calls.map(call => [call.method, call.url]), [['POST', '/api/categorias'], ['PUT', '/api/categorias/8'], ['DELETE', '/api/categorias/8']]);
  assert.deepEqual(JSON.parse(calls[0].body), data);
  assert.equal(calls[0].headers['Content-Type'], 'application/json');
  assert.equal(calls[2].body, undefined);
});

test('400 preserva os erros por campo enviados pelo ASP.NET', async () => {
  const api = createApi('/api', async () => Response.json({ title: 'Erro de validação.', errors: { Nome: ['Informe o nome.'] } }, { status: 400 }));
  await assert.rejects(api.create('fabricantes', { nome: '' }), error => error instanceof ApiError && error.status === 400 && error.errors.Nome[0] === 'Informe o nome.');
});

test('409 mantém a mensagem de exclusão bloqueada e não repete a operação', async () => {
  let attempts = 0;
  const api = createApi('/api', async () => {
    attempts++;
    return Response.json({ mensagem: 'Existem veículos vinculados.' }, { status: 409 });
  });
  await assert.rejects(api.remove('fabricantes', 1), error => error.status === 409 && error.message === 'Existem veículos vinculados.');
  assert.equal(attempts, 1);
});

test('respostas HTML de erro não aparecem como código no aviso ao usuário', async () => {
  const api = createApi('/api', async () => new Response('<html>Erro do servidor</html>', { status: 404 }));
  await assert.rejects(api.get('categorias', 99), error => error.status === 404 && !error.message.includes('<html>'));
});

test('uma resposta de sucesso com JSON inválido não é confundida com lista vazia', async () => {
  const api = createApi('/api', async () => new Response('texto inválido', { status: 200 }));
  await assert.rejects(api.list('categorias'), /resposta inválida/);
});

test('falha de rede resulta em mensagem de serviço indisponível', async () => {
  const api = createApi('/api', async () => { throw new TypeError('fetch failed'); });
  await assert.rejects(api.list('fabricantes'), error => error.status === 0 && error.message.includes('acessar o serviço'));
});

test('timeout é informado; cancelamento de consulta antiga permanece silencioso para a tela', async () => {
  const waitForAbort = (_, options) => new Promise((resolve, reject) => {
    options.signal.addEventListener('abort', () => reject(new DOMException('Aborted', 'AbortError')), { once: true });
  });
  await assert.rejects(createApi('/api', waitForAbort, 15).list('categorias'), /demorou para responder/);
  const controller = new AbortController();
  const pending = createApi('/api', waitForAbort).list('fabricantes', {}, controller.signal);
  controller.abort();
  await assert.rejects(pending, error => error.name === 'AbortError');
});
