// Substitui a validação de RANGE (intervalo)
$.validator.methods.range = function (value, element, param) {
    var globalizedValue = value.replace(",", ".");
    return this.optional(element) || (globalizedValue >= param[0] && globalizedValue <= param[1]);
}

// Substitui a validação de NUMBER (se é número)
$.validator.methods.number = function (value, element) {
    return this.optional(element) || /^-?(?:\d+|\d{1,3}(?:[\s\.,]\d{3})+)(?:[\.,]\d+)?$/.test(value);
}


async function salvarCategoriaRapida() {
    var nomeCategoria = document.getElementById('inputNovaCategoria').value;

    if (!nomeCategoria) {
        alert("Digite um nome para a categoria!");
        return;
    }

    // Prepara os dados para enviar
    var dados = { Name: nomeCategoria };

    try {
        // Chama o Backend sem recarregar a página (Fetch API)
        const resposta = await fetch('/Categorias/CriarRapido', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(dados)
        });

        if (resposta.ok) {
            const novaCategoria = await resposta.json();

            // 1. Cria a nova opção no Dropdown
            var select = document.getElementById('selectCategoria');
            var option = document.createElement("option");
            option.text = novaCategoria.name;
            option.value = novaCategoria.id;
            option.selected = true; // Já deixa selecionado
            select.add(option);

            // 2. Fecha o Modal
            var modalEl = document.getElementById('modalNovaCategoria');
            var modal = bootstrap.Modal.getInstance(modalEl);
            modal.hide();

            // 3. Limpa o campo
            document.getElementById('inputNovaCategoria').value = "";
        } else {
            alert("Erro ao salvar categoria. Tente novamente.");
        }
    } catch (erro) {
        console.error("Erro:", erro);
        alert("Erro de conexão.");
    }
}