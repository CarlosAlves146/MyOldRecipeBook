using MyRecipeBook.Domain.Extension;
using System.Globalization;
using System.Runtime.Serialization;

namespace MyRecipeBook.API.Middleware
{
    public class CultureMiddleware
    {
        // readonly, para que o valor dessa variável seja atribuída apenas uma única vez podemos
        // atribuir na definição dela ou dentro do construto, vamos atribuir no construtor.
        private readonly RequestDelegate _next;

        // Criamos o construtor para que após utilizarmos o Middleware, precisamos dar continuidade no fluxo.
        public CultureMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        //➡️ public: qualquer parte do código pode chamar esse método.
        //➡️ async: indica que o método é assíncrono, ou seja, pode executar tarefas que não bloqueiam o fluxo principal.
        //➡️ Task: significa que ele retorna uma tarefa(uma operação assíncrona).
        //➡️ Invoke: nome padrão para métodos de Middleware, que são chamados quando a requisição passa por ele.
        //➡️ HttpContext context: é o contexto da requisição HTTP. Contém informações sobre o que está acontecendo, como cabeçalhos, rota, usuário, etc.
        public async Task Invoke(HttpContext context)
        {
            // Aqui estamos criando uma variável que vai armazenar todos os tipos de linguagens
            // que o .net da suporte, para depois podermos identificar se a cultere que veio na
            // requisição está entre elas.
            var supportedLanguages = CultureInfo.GetCultures(CultureTypes.AllCultures);

            // recuperando do cabeçalho da requisição a cultura que o app solicitou / 35 - 5:30
            // pegamos o primeiro valor, se tiver ou se não houver, null/default
            var requestCulture = context.Request.Headers.AcceptLanguage.FirstOrDefault();

            // após identificar a cultura que veio da requisição, vamos trocar a nossa, a princípio para
            // um valor de default "en". já vamos deixa-lo salvo nessa variavel.
            var cultureInfo = new CultureInfo("en");

            // verificando se o valor que veio sobre a culture não é um valor null ou em branco.
            // verificando se o valor que veio faz parte das linguagens suportadas pelas .net
            // método IsFalse criado por nos em Domain, para alinhar com os requisitos do SonarCloud
            if(string.IsNullOrWhiteSpace(requestCulture).IsFalse() && supportedLanguages.Any(c => c.Name.Equals(requestCulture)))
            {
                // Se o if passar, você estará efetivamente aceitando o idioma solicitado pelo cliente no header da requisição
                // desde que seja um idioma suportado.Isso é o que permite que sua API seja multi-idioma de forma segura.
                cultureInfo = new CultureInfo(requestCulture!);
            }
            // Aqui estamos configurando a cultura atual da aplicação:
            // CurrentCulture → usado para formatar números, datas, moedas...
            // CurrentUICulture → usado para escolher textos de interface localizados.
            CultureInfo.CurrentCulture = cultureInfo; // caso tenhamos entrado no if será setado o idioma solicitado no header caso não tenha entrado será o idioma default "en"
            CultureInfo.CurrentUICulture = cultureInfo; // tbm dependendo do controle do "if" aqui vai definir a cultura para recursos de interface, como strings, mensagens, labels e traduções que você tenha no seu projeto

            // dizendo aqui, que com esse trecho o código de continuídade no fluxo.
            // await é um é uma palavra-chave do c# que serve para dizer, "Espere essa tarefa terminar antes de continuar".
            // isso evita um travamento do sistema, até que a tarefa seja concluída.
            await _next(context);
        }
    }
}
