import { createApi, ApiError } from './api.js';

const api = createApi();
const $ = selector => document.querySelector(selector);
const escapeHtml = value => String(value ?? '').replace(/[&<>"']/g, char => ({ '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#39;' }[char]));
const currency = value => Number(value).toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' });
const config = {
  fabricantes: {
    title: 'Fabricantes', singular: 'fabricante', article: 'o', createLabel: 'Novo fabricante',
    description: 'Organize as marcas e os países de origem da sua frota.',
    columns: ['Fabricante', 'País de origem', 'Ações'],
    fields: [
      { key: 'nome', label: 'Nome', required: true, maxLength: 100 },
      { key: 'paisOrigem', label: 'País de origem', maxLength: 60 }
    ],
    filters: [{ key: 'nome', label: 'Nome', maxLength: 100 }, { key: 'paisOrigem', label: 'País de origem', maxLength: 60 }]
  },
  categorias: {
    title: 'Categorias', singular: 'categoria', article: 'a', createLabel: 'Nova categoria',
    description: 'Defina as categorias dos veículos e suas diárias de referência.',
    columns: ['Categoria', 'Descrição', 'Diária base', 'Ações'],
    fields: [
      { key: 'nome', label: 'Nome', required: true, maxLength: 60 },
      { key: 'valorDiariaBase', label: 'Diária base (R$)', type: 'number', required: true, min: '0.01', max: '99999999.99', step: '0.01' },
      { key: 'descricao', label: 'Descrição', type: 'textarea', maxLength: 255, full: true }
    ],
    filters: [
      { key: 'nome', label: 'Nome', maxLength: 60 },
      { key: 'diariaMin', label: 'Diária mínima (R$)', type: 'number', min: '0', step: '0.01' },
      { key: 'diariaMax', label: 'Diária máxima (R$)', type: 'number', min: '0', step: '0.01' }
    ]
  }
};
const state = { page: 'fabricantes', filters: {}, rows: [], loading: false, error: null };
let listController, listSequence = 0, actionSequence = 0, saving = false, toastTimer;
const dialog = $('#record-dialog');

function notify(message) {
  clearTimeout(toastTimer);
  $('#notifications').innerHTML = `<div class="toast">${escapeHtml(message)}</div>`;
  toastTimer = setTimeout(() => $('#notifications').replaceChildren(), 6500);
}

function fieldMarkup(field, prefix, value = '', grid = false) {
  const id = `${prefix}-${field.key}`;
  const attributes = [
    `id="${id}"`, `name="${field.key}"`,
    field.required ? 'required' : '',
    field.maxLength ? `maxlength="${field.maxLength}"` : '',
    ...['min', 'max', 'step'].map(key => field[key] !== undefined ? `${key}="${field[key]}"` : '')
  ].filter(Boolean).join(' ');
  const input = field.type === 'textarea'
    ? `<textarea ${attributes} rows="3">${escapeHtml(value)}</textarea>`
    : `<input ${attributes} type="${field.type || 'text'}" value="${escapeHtml(value)}">`;
  return `<div class="${grid ? field.full ? 'col-12' : 'col-md-6' : ''}"><div class="field"><label for="${id}">${field.label}${field.required ? ' <span aria-label="obrigatório">*</span>' : ''}</label>${input}</div></div>`;
}

function clearErrors(root) {
  root.querySelectorAll('[aria-invalid]').forEach(input => {
    input.removeAttribute('aria-invalid');
    input.removeAttribute('aria-describedby');
  });
  root.querySelectorAll('.field-error').forEach(node => node.remove());
  const alert = root.querySelector('[data-error]');
  if (alert) { alert.hidden = true; alert.replaceChildren(); }
}

function showError(root, error) {
  const alert = root.querySelector('[data-error]');
  if (alert) {
    alert.textContent = error.message || 'Não foi possível concluir a operação.';
    alert.hidden = false;
  }
  const unmatched = [];
  for (const [key, messages] of Object.entries(error.errors || {})) {
    const input = Array.from(root.querySelectorAll('[name]')).find(node => node.name.toLowerCase() === key.toLowerCase());
    const message = Array.isArray(messages) ? messages.join(' ') : String(messages);
    if (!input) { unmatched.push(message); continue; }
    input.setAttribute('aria-invalid', 'true');
    input.setAttribute('aria-describedby', `${input.id}-error`);
    const label = document.createElement('p');
    label.className = 'field-error'; label.id = `${input.id}-error`; label.textContent = message;
    input.closest('.field').append(label);
  }
  if (unmatched.length && alert) {
    const list = document.createElement('ul');
    unmatched.forEach(message => { const item = document.createElement('li'); item.textContent = message; list.append(item); });
    alert.append(list);
  }
  root.querySelector('[aria-invalid=true]')?.focus();
}

function renderFrame() {
  const c = config[state.page];
  document.title = `${c.title} | Locadora`;
  document.querySelectorAll('nav [data-page]').forEach(link => {
    if (link.dataset.page === state.page) link.setAttribute('aria-current', 'page');
    else link.removeAttribute('aria-current');
  });
  $('#content').innerHTML = `<div class="heading"><div><p class="eyebrow">Gestão da locadora</p><h1>${c.title}</h1><p class="sub">${c.description}</p></div><button class="button primary" id="new-record">+ ${c.createLabel}</button></div>
    <section class="panel" aria-label="Lista de ${c.title.toLowerCase()}">
      <form class="filters" id="filters">${c.filters.map(field => fieldMarkup(field, 'filter')).join('')}<button class="button primary" type="submit">Filtrar</button><button class="button" type="button" id="clear-filters">Limpar</button></form>
      <div class="alert filter-error" data-error role="alert" hidden></div>
      <div class="table-scroll"><table><thead><tr>${c.columns.map(column => `<th scope="col">${column}</th>`).join('')}</tr></thead><tbody id="rows"></tbody></table></div>
      <div class="table-foot"><span id="result-count" aria-live="polite"></span><span id="updated-at"></span></div>
    </section>`;
  $('#new-record').addEventListener('click', () => openForm(state.page));
  $('#filters').addEventListener('submit', event => {
    event.preventDefault();
    clearErrors($('#content'));
    const filters = Object.fromEntries(new FormData(event.currentTarget));
    if (filters.diariaMin !== '' && filters.diariaMax !== '' && filters.diariaMin !== undefined && filters.diariaMax !== undefined && Number(filters.diariaMin) > Number(filters.diariaMax)) {
      showError($('#content'), new ApiError('Confira a faixa de diária.', 400, { diariaMax: ['A diária máxima deve ser maior ou igual à mínima.'] }));
      return;
    }
    state.filters = filters;
    loadRows();
  });
  $('#clear-filters').addEventListener('click', () => {
    $('#filters').reset(); state.filters = {}; clearErrors($('#content')); loadRows();
  });
  $('#rows').addEventListener('click', event => {
    const button = event.target.closest('[data-action]');
    if (button && !button.disabled) recordAction(button);
  });
}

function renderRows() {
  const c = config[state.page];
  const body = $('#rows');
  body.setAttribute('aria-busy', String(state.loading));
  if (state.loading) {
    body.innerHTML = `<tr><td class="empty" colspan="${c.columns.length}" role="status">Carregando registros…</td></tr>`;
    $('#result-count').textContent = 'Atualizando…'; $('#updated-at').textContent = ''; return;
  }
  if (state.error) {
    body.innerHTML = `<tr><td class="empty" colspan="${c.columns.length}"><p>${escapeHtml(state.error.message)}</p><button type="button" class="button" id="retry-load">Tentar novamente</button></td></tr>`;
    $('#retry-load').addEventListener('click', () => loadRows());
    $('#result-count').textContent = 'Consulta indisponível'; $('#updated-at').textContent = ''; return;
  }
  body.innerHTML = state.rows.map(row => {
    const cells = state.page === 'fabricantes'
      ? [`${escapeHtml(row.nome)}<small>#${row.id}</small>`, escapeHtml(row.paisOrigem) || '—']
      : [escapeHtml(row.nome), escapeHtml(row.descricao) || '—', currency(row.valorDiariaBase)];
    const actions = [['details', 'Ver'], ['edit', 'Editar'], ['delete', 'Excluir']].map(([action, text]) => `<button type="button" class="button link${action === 'delete' ? ' danger' : ''}" data-action="${action}" data-id="${row.id}" aria-label="${text} ${escapeHtml(row.nome)}">${text}</button>`).join('');
    return `<tr>${cells.map(cell => `<td>${cell}</td>`).join('')}<td><div class="actions">${actions}</div></td></tr>`;
  }).join('') || `<tr><td colspan="${c.columns.length}" class="empty">${Object.values(state.filters).some(value => String(value).trim()) ? 'Nenhum registro encontrado. Altere ou limpe os filtros.' : `Nenhum${c.article === 'a' ? 'a' : ''} ${c.singular} cadastrad${c.article}. Use o botão acima para cadastrar.`}</td></tr>`;
  const count = state.rows.length;
  $('#result-count').textContent = `${count} registro${count === 1 ? '' : 's'} encontrado${count === 1 ? '' : 's'}`;
  $('#updated-at').textContent = `Atualizado às ${new Date().toLocaleTimeString('pt-BR', { hour: '2-digit', minute: '2-digit' })}`;
}

async function loadRows() {
  listController?.abort();
  listController = new AbortController();
  const sequence = ++listSequence;
  state.loading = true; state.error = null; state.rows = [];
  renderRows();
  try {
    if (location.protocol === 'file:') throw new ApiError('Abra a aplicação pelo endereço exibido ao iniciar o projeto para acessar os cadastros.');
    const rows = await api.list(state.page, state.filters, listController.signal);
    if (sequence !== listSequence) return false;
    if (!Array.isArray(rows)) throw new ApiError('O serviço retornou uma lista inválida. Tente novamente.');
    state.rows = rows;
    return true;
  } catch (error) {
    if (sequence !== listSequence || error.name === 'AbortError') return false;
    state.error = error;
    if (error.status === 400) showError($('#content'), error);
    return false;
  } finally {
    if (sequence === listSequence) { state.loading = false; renderRows(); }
  }
}

function closeDialog() { if (!saving && dialog.open) dialog.close(); }
$('#close-dialog').addEventListener('click', closeDialog);
dialog.addEventListener('cancel', event => { if (saving) event.preventDefault(); });

function showDialog(title, body) {
  $('#dialog-title').textContent = title;
  $('#dialog-content').innerHTML = body;
  if (!dialog.open) dialog.showModal();
  dialog.querySelector('[data-close]')?.addEventListener('click', closeDialog);
}

function lockForm(form, locked) {
  saving = locked;
  $('#close-dialog').disabled = locked;
  form.querySelectorAll('input, textarea, button').forEach(control => { control.disabled = locked; });
  form.setAttribute('aria-busy', String(locked));
}

async function recordAction(button) {
  const type = state.page, id = Number(button.dataset.id), action = button.dataset.action;
  const sequence = ++actionSequence;
  button.disabled = true;
  try {
    const row = await api.get(type, id);
    if (type !== state.page || sequence !== actionSequence) return;
    if (action === 'edit') openForm(type, row);
    else if (action === 'delete') openDelete(type, row);
    else openDetails(type, row);
  } catch (error) {
    if (type === state.page && sequence === actionSequence) notify(error.message);
  } finally { button.disabled = false; }
}

function openForm(type, record = null) {
  const c = config[type];
  showDialog(record ? `Editar ${c.singular}` : c.createLabel, `<form id="record-form" novalidate>
    <div class="dialog-body"><p class="form-note">Campos com * são obrigatórios.</p><div class="alert" role="alert" data-error hidden></div>
    <div class="row">${c.fields.map(field => fieldMarkup(field, 'record', record?.[field.key] ?? '', true)).join('')}</div></div>
    <div class="dialog-foot"><button type="button" class="button" data-close>Cancelar</button><button type="submit" class="button primary" id="save-record">${record ? 'Salvar alterações' : 'Cadastrar'}</button></div></form>`);
  const form = $('#record-form');
  form.addEventListener('submit', async event => {
    event.preventDefault();
    if (saving) return;
    clearErrors(form);
    form.querySelectorAll('input[type=text],textarea').forEach(input => { input.value = input.value.trim(); });
    if (!form.reportValidity()) return;
    const data = Object.fromEntries(new FormData(form));
    if (type === 'categorias') data.valorDiariaBase = Number(data.valorDiariaBase);
    lockForm(form, true);
    const submitButton = $('#save-record'), originalLabel = submitButton.textContent;
    submitButton.textContent = 'Salvando…';
    let saved = false;
    try {
      if (record) await api.update(type, record.id, data);
      else await api.create(type, data);
      saved = true;
    } catch (error) { showError(form, error); }
    finally { lockForm(form, false); submitButton.textContent = originalLabel; }
    if (saved) {
      closeDialog();
      const refreshed = await loadRows();
      notify(refreshed ? `${c.singular[0].toUpperCase() + c.singular.slice(1)} ${record ? 'atualizad' : 'cadastrad'}${c.article} com sucesso.` : 'Registro salvo. Não foi possível atualizar a lista; use “Tentar novamente”.');
    }
  });
}

function openDetails(type, record) {
  const c = config[type];
  const fields = [{ key: 'id', label: 'Identificador' }, ...c.fields];
  showDialog(`Detalhes d${c.article} ${c.singular}`, `<div class="dialog-body"><dl class="detail-grid">${fields.map(field => `<div><dt>${field.label}</dt><dd>${field.key === 'valorDiariaBase' ? currency(record[field.key]) : escapeHtml(record[field.key]) || '—'}</dd></div>`).join('')}</dl></div><div class="dialog-foot"><button type="button" class="button" data-close>Fechar</button></div>`);
}

function openDelete(type, record) {
  const c = config[type];
  showDialog('Confirmar exclusão', `<form id="delete-form"><div class="dialog-body"><div class="alert" role="alert" data-error hidden></div><p>Deseja excluir ${c.article} ${c.singular} <strong>${escapeHtml(record.nome)}</strong>?</p><p class="form-note">A exclusão será impedida se houver veículos vinculados a este cadastro.</p></div><div class="dialog-foot"><button type="button" class="button" data-close>Cancelar</button><button type="submit" class="button danger" id="delete-record">Excluir registro</button></div></form>`);
  const form = $('#delete-form');
  form.addEventListener('submit', async event => {
    event.preventDefault();
    if (saving) return;
    clearErrors(form); lockForm(form, true);
    $('#delete-record').textContent = 'Excluindo…';
    let deleted = false;
    try { await api.remove(type, record.id); deleted = true; }
    catch (error) { showError(form, error); }
    finally { lockForm(form, false); $('#delete-record').textContent = 'Excluir registro'; }
    if (deleted) {
      closeDialog(); const refreshed = await loadRows();
      notify(refreshed ? 'Registro excluído com sucesso.' : 'Registro excluído. Não foi possível atualizar a lista; use “Tentar novamente”.');
    }
  });
}

function navigate() {
  // Uma gravação deve terminar antes de trocar a tela, inclusive pelo histórico.
  if (saving) { history.replaceState(null, '', `#${state.page}`); return; }
  closeDialog();
  ++actionSequence;
  const requested = location.hash.slice(1);
  state.page = Object.hasOwn(config, requested) ? requested : 'fabricantes';
  state.filters = {}; state.rows = []; state.error = null;
  renderFrame(); loadRows();
}
window.addEventListener('hashchange', navigate);
navigate();
