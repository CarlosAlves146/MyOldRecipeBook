using System.Net.Http;
using System.Net.Http.Json;
using System.Net.Http.Headers;
using CommonTestUtilities.Requests;

namespace WebApi.Test
{
    // Classe criada para centralizarmos alguns códigos duplicados após refatoração.
    public class MyRecipeBookClassFixture : IClassFixture<CustomWebApplicationFactory>
    {
        // Váriavel privada para armazenar o HttpClient, que será usada para fazer chamadas HTTP para nossa API durante os testes.
        private readonly HttpClient _httpClient;

        public MyRecipeBookClassFixture(CustomWebApplicationFactory factory) 
        {
            // O CustomWebApplicationFactory cria uma "versão em memória" da sua API.
            // Isso significa que, durante o teste, sua API roda sem precisar abrir servidor real,
            // sem precisar rodar no navegador, sem precisar usar IIS ou Kestrel.
            // Ela roda internamente, dentro do próprio teste.

            // O CreateClient() devolve um HttpClient conectado a essa API de teste.
            // Assim, quando fazemos _httpClient.Get/Post/Put(...),
            // estamos chamando os endpoints da nossa API como se estivéssemos chamando de verdade,
            // mas tudo rodando automaticamente em segundo plano.
            _httpClient = factory.CreateClient();
        }

        // Método criado para evitar a duplicação de código nos teste de integração
        protected async Task <HttpResponseMessage> DoPost(string method, object request, string token = "", string culture = "en")
        {
           ChangeRequestCulture(culture);
           if(token is not null)
            {
                AuthorizeRequest(token);
            }
            // ACT → Envia a requisição POST para o endpoint User
            return await _httpClient.PostAsJsonAsync(method, request);
        }
        protected async Task<HttpResponseMessage> DoGet(string method, string token = "", string culture = "en")
        {
            ChangeRequestCulture(culture);
            AuthorizeRequest(token);

            // ACT → Envia a requisição POST para o endpoint User
            return await _httpClient.GetAsync(method);
        }
        protected async Task<HttpResponseMessage> DoPut(string method, object request, string token, string culture = "en")
        {
            ChangeRequestCulture(culture);
            AuthorizeRequest(token);

            // ACT → Envia a requisição PUT para o endpoint User
            return await _httpClient.PutAsJsonAsync(method, request);
        }
        private void ChangeRequestCulture(string culture)
        {
            Console.WriteLine(_httpClient.DefaultRequestHeaders.Contains("Accept-Language"));
            // verificando a existência na requisição de um Accept-Language
            // se houver vamos remover e adicionar a nossa "culture" para teste
            if (_httpClient.DefaultRequestHeaders.Contains("Accept-Language"))
            {
                // removendo o valor e adicionando novamente a culture
                _httpClient.DefaultRequestHeaders.Remove("Accept-Language");
            }
            _httpClient.DefaultRequestHeaders.Add("Accept-Language", culture);
        }
        
        // Aqui criamos um "gerador" de token
        // Se o token for informado, adicionaremos esse token no cabeçalho da requisição HTTP como um header de autorização do tipo Bearer
        private void AuthorizeRequest(string token)
        {
            if(string.IsNullOrWhiteSpace(token))
            {
                return;
            }
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

    }
}
