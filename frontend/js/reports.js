const vehicles = [['placa','Placa'], ['modelo','Modelo'], ['fabricanteNome','Fabricante'], ['categoriaNome','Categoria']];
const reports = [
  { title:'Veículos por fabricante', fields:[['fabricanteId','Fabricante','fabricantes']], route:f => 'veiculos/por-fabricante/' + Number(f.fabricanteId), columns:vehicles },
  { title:'Veículos disponíveis', fields:[['categoriaId','Categoria','categorias',true]], route:() => 'veiculos/disponiveis', filters:f => f, columns:vehicles },
  { title:'Clientes sem aluguel ativo', route:() => 'clientes/sem-aluguel-ativo', columns:[['nome','Cliente'],['email','E-mail'],['telefone','Telefone']] },
  { title:'Aluguéis por período', fields:[['inicio','Início','date'],['fim','Fim','date']], route:() => 'alugueis/por-periodo',
    filters:f => ({ inicio:f.inicio + 'T00:00:00', fim:f.fim + 'T23:59:59.9999999' }),
    columns:[['clienteNome','Cliente'],['veiculoPlaca','Placa'],['dataRetirada','Retirada'],['valorTotal','Total']] },
  { title:'Faturamento por categoria', route:() => 'alugueis/faturamento-por-categoria',
    columns:[['categoriaNome','Categoria'],['quantidadeAlugueis','Aluguéis finalizados'],['faturamentoTotal','Faturamento']] }
];

export function mountReports(root, api, displayValue) {
  root.innerHTML = '<div class="heading"><div><h1>Consultas e relatórios</h1><p class="sub">Escolha uma consulta para ver os resultados do banco.</p></div></div>';
  const section = document.createElement('section');
  section.className = 'panel';
  section.innerHTML = '<form class="filters"><div class="field"><label for="report">Consulta</label><select id="report"></select></div><div class="report-fields"></div><button class="button primary" type="submit">Consultar</button></form><div class="table-scroll" aria-live="polite"></div>';
  root.append(section);
  const select = section.querySelector('#report');
  const form = section.querySelector('form');
  const submit = form.querySelector('button');
  const fields = section.querySelector('.report-fields');
  const result = section.querySelector('.table-scroll');
  let request;
  for (const [index, report] of reports.entries()) select.add(new Option(report.title, index));

  async function prepare() {
    request?.abort();
    const current = new AbortController();
    request = current;
    fields.replaceChildren();
    result.textContent = '';
    submit.disabled = true;
    try {
      for (const [key, name, type, optional] of reports[select.value].fields ?? []) {
        const wrapper = document.createElement('div');
        wrapper.className = 'field';
        const label = document.createElement('label');
        label.htmlFor = 'report-' + key;
        label.textContent = name;
        const control = document.createElement(type === 'date' ? 'input' : 'select');
        control.id = label.htmlFor;
        control.name = key;
        control.required = !optional;
        if (type === 'date') control.type = 'date';
        else {
          control.add(new Option(optional ? 'Todas' : 'Selecione', ''));
          for (const row of await api.list(type, {}, current.signal)) control.add(new Option(row.nome, row.id));
        }
        wrapper.append(label, control);
        fields.append(wrapper);
      }
      if (!current.signal.aborted) submit.disabled = false;
    } catch (error) {
      if (!current.signal.aborted) {
        result.textContent = error.message;
        const retry = document.createElement('button');
        retry.className = 'button';
        retry.textContent = 'Recarregar opções';
        retry.onclick = prepare;
        result.append(' ', retry);
      }
    }
  }
  select.addEventListener('change', prepare);
  form.addEventListener('submit', async event => {
    event.preventDefault();
    const report = reports[select.value];
    const params = Object.fromEntries(new FormData(form));
    if (params.inicio && params.fim < params.inicio) {
      result.textContent = 'A data final deve ser igual ou posterior à inicial.';
      return;
    }
    request?.abort();
    const current = new AbortController();
    request = current;
    submit.disabled = true;
    result.textContent = 'Consultando…';
    try {
      const rows = await api.list(report.route(params), report.filters?.(params), current.signal);
      if (!rows.length) { result.textContent = 'Nenhum resultado encontrado.'; return; }
      const table = document.createElement('table');
      const header = table.createTHead().insertRow();
      for (const [,name] of report.columns) {
        const th = document.createElement('th');
        th.scope = 'col';
        th.textContent = name;
        header.append(th);
      }
      const body = table.createTBody();
      for (const row of rows) {
        const tr = body.insertRow();
        for (const [key] of report.columns) tr.insertCell().textContent = displayValue(row, key);
      }
      result.replaceChildren(table);
    } catch (error) {
      if (!current.signal.aborted) result.textContent = error.message;
    } finally {
      if (!current.signal.aborted) submit.disabled = false;
    }
  });
  prepare();
  return () => request?.abort();
}
