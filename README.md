# 🍎 Quitanda Bits & Bananas

> Sistema de Gestão de Estoque Inteligente com Dashboard Analítico, Auditoria e Controle de Perecíveis.

![.NET](https://img.shields.io/badge/.NET-8.0-purple)
![Status](https://img.shields.io/badge/Status-Conclu%C3%ADdo-success)
![License](https://img.shields.io/badge/License-MIT-blue)

## 📖 Sobre o Projeto

O **Quitanda Bits & Bananas** evoluiu de um simples cadastro de produtos para um **Ecossistema de Gestão de Estoque**. O foco do projeto é resolver dores reais de pequenos comércios: perda de mercadoria por validade, falta de rastreabilidade e dificuldade de visualizar o fluxo de caixa parado no estoque.

O sistema utiliza uma arquitetura híbrida **MVC + API**, onde o Backend serve tanto as páginas HTML quanto os dados JSON para alimentar os gráficos do Dashboard em tempo real.

## 🚀 Funcionalidades de Destaque

### 📊 Dashboard Executivo (BI)
- **API Rest Integrada:** Os dados dos gráficos são consumidos de uma API interna (`/api/dashboard`) desenvolvida no próprio projeto.
- **Visualização de Dados:**
  - Gráfico de Rosca (Chart.js): Distribuição de Estoque Saudável vs. Vencido vs. Baixo.
  - Gráfico de Barras: Fluxo de Entradas e Saídas dos últimos 7 dias.
- **KPIs (Indicadores):** Cards com valor total capitalizado, alertas de vencimento e contagem de itens.

### 📦 Controle de Estoque & Kardex (Rastreabilidade)
- **Histórico Imutável:** A edição manual da quantidade foi bloqueada. O estoque só é alterado através de transações de **Entrada** e **Saída**.
- **Auditoria ("Dedo Duro"):** O sistema registra automaticamente **QUEM** (Usuário Logado) realizou a movimentação e **QUANDO**.
- **Motivos de Movimentação:** Categorização precisa (Compra, Venda, Perda por Validade, Consumo Próprio).
- **Extrato:** Visualização de histórico individual por produto e histórico geral da loja.

### 🔐 Segurança & Identity
- **Autenticação Robusta:** Implementação do **ASP.NET Core Identity**.
- **Proteção de Rotas:** Controllers e APIs protegidos com `[Authorize]`.
- **UI Customizada:** Telas de Login e Registro totalmente customizadas com CSS moderno (efeito Glassmorphism) fugindo do padrão básico do Visual Studio.

### 🛠️ Usabilidade & UX
- **Semáforo de Validade:** As linhas da tabela mudam de cor (Vermelho/Amarelo) conforme a proximidade da data de validade.
- **Quick Add (AJAX):** Cadastro rápido de Categorias e Fornecedores via Modal sem recarregar a página.
- **Feedback Visual:** Substituição de alertas nativos pelo **SweetAlert2** para notificações elegantes.
- **Relatórios:** Exportação de inventário completo para **Excel (.xlsx)** usando ClosedXML.

## 💻 Tecnologias Utilizadas

* **Core:** C# .NET 8, ASP.NET Core MVC
* **Dados:** Entity Framework Core, SQL Server (LocalDB)
* **Frontend:** Razor, Bootstrap 5, JavaScript (Fetch API)
* **Libs Gráficas:** Chart.js (Gráficos), SweetAlert2 (Popups), FontAwesome (Ícones)
* **Utilitários:** ClosedXML (Excel), jQuery Validation

## 📸 Galeria do Projeto

### Dashboard Analítico
![Dashboard](Assets/Tela-Inicial.png)

### Histórico de Movimentação (Com Auditoria)
![Histórico](Assets/Historico.png)

### Gestão de Estoque (Semáforo)
![Lista Produtos](Assets/Produtos.png)

## ⚙️ Como Rodar o Projeto Localmente

### Pré-requisitos
* [.NET SDK 8.0](https://dotnet.microsoft.com/download)
* SQL Server (ou LocalDB do Visual Studio)

### Instalação

1.  **Clone o repositório:**
    ```bash
    git clone [https://github.com/SEU-USUARIO/Quitanda-Bits-e-Bananas.git](https://github.com/SEU-USUARIO/Quitanda-Bits-e-Bananas.git)
    ```

2.  **Restaure os pacotes:**
    ```bash
    dotnet restore
    ```

3.  **Configure o Banco de Dados:**
    O projeto usa EF Core Migrations. Execute o comando para criar o banco e as tabelas (incluindo as de Identity):
    ```bash
    dotnet ef database update
    ```

4.  **Execute a aplicação:**
    ```bash
    dotnet run
    ```

5.  **Primeiro Acesso:**
    Acesse `https://localhost:7000` (ou a porta indicada). Registre um novo usuário para liberar o acesso ao sistema.

---
Desenvolvido por **[Luiz Canziani]** para fins de estudo e portfólio. 👨‍💻
