using CommonTestUtilities.Requests;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Exceptions;
using Shouldly;
using System.Globalization;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using WebApi.Test.InlineData;



namespace WebApi.Test.Login
{
    public class DoLoginTest : MyRecipeBookClassFixture
    {
        private readonly string METHOD = "login";

        private readonly string _email;
        private readonly string _password;
        private readonly string _name;

        // Construtor
        public DoLoginTest(CustomWebApplicationFactory factory) : base(factory) 
        {
            _email = factory.GetEmail();
            _password = factory.GetPassword();
            _name = factory.GetName();
        }
        
        [Fact]
        public async Task Success()
        {
            // ARRANGE → Monta os dados necessários para chamar a API
            var request = new RequestLoginJson()
            {
                Email = _email,
                Password = _password
            };

            // ACT → Envia a requisição POST para o endpoint User
            var response = await DoPost(method : METHOD, request : request);

            // ASSERT → Valida se o StatusCode retornado foi 200 (OK)
            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            // Captura o corpo da resposta como texto (a API devolve JSON)
            await using var responseBody = await response.Content.ReadAsStreamAsync();

            // Converte o texto da resposta para objeto JSON
            var json = await JsonDocument.ParseAsync(responseBody);

            // Valida se o campo "name" retornado é igual ao "Name" enviado na request
            json.RootElement
                .GetProperty("name")
                .GetString()
                .ShouldBeEquivalentTo(_name);

            // Valida o campo "tokens"
            json.RootElement
                .GetProperty("tokens").GetProperty("accessToken")
                .GetString().ShouldNotBeNullOrEmpty();
        }

        [Theory]
        [ClassData(typeof(CultureInlineDataTest))]
        public async Task Error_login_Invalid(string culture)
        {
            // ARRANGE → Monta os dados necessários para chamar a API
            var request = RequestLoginUserJsonBuilder.Build();

            // ACT → Envia a requisição "inválida propositalmente" POST para o endpoint User
            var response = await DoPost(method : METHOD, request : request, culture : culture);

            // ASSERT → Valida se o StatusCode retornado foi 401 (BabRequest)
            response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);

            // Captura o corpo da resposta como texto (a API devolve JSON)
            await using var responseBody = await response.Content.ReadAsStreamAsync();

            // Converte o texto da resposta para objeto JSON
            var json = await JsonDocument.ParseAsync(responseBody);

            // pegando o campo "erros" do JSON e transformando ele em uma lista enumerável
            var errors = json.RootElement.GetProperty("errors").EnumerateArray();

            // Recuperando a mensagem esperada do Resource de acordo com a culture recebida no parâmetro
            // ✔ Busca no arquivo.resx a mensagem correspondente à chave "NAME_EMPTY"
            // ✔ Usando a cultura atual da execução
            var expectedMessage = ResourceMessagesException.ResourceManager.GetString("EMAIL_OR_PASSWORD_INVALID", new CultureInfo(culture));

            errors.ShouldHaveSingleItem()
                .GetString()!.ShouldBe(expectedMessage);
        }
    }
}
