using CommonTestUtilities.Requests;
using MyRecipeBook.Exceptions;
using Shouldly;
using System.Globalization;
using System.Net;
using System.Text.Json;
using WebApi.Test.InlineData;

namespace WebApi.Test.User.Register
{
    public class RegisterUserTest : MyRecipeBookClassFixture
    {
        // varável com a url em minúsculo
        private readonly string METHOD = "user";

        public RegisterUserTest(CustomWebApplicationFactory factory) : base(factory) { }       

        [Fact]
        public async Task Sucess()
        {
            // ARRANGE → Monta os dados necessários para chamar a API
            var request = RequestRegisterUserJsonBuilder.Build();

            // ACT → Envia a requisição POST para o endpoint User
            var response = await DoPost(method : METHOD, request: request);

            // ASSERT → Valida se o StatusCode retornado foi 201 (Created)
            response.StatusCode.ShouldBe(HttpStatusCode.Created);

            // Captura o corpo da resposta como texto (a API devolve JSON)
            await using var responseBody = await response.Content.ReadAsStreamAsync();

            // Converte o texto da resposta para objeto JSON
            var json = await JsonDocument.ParseAsync(responseBody);

            // Valida se o campo "name" retornado é igual ao "Name" enviado na request
            json.RootElement
                .GetProperty("name")
                .GetString()
                .ShouldBeEquivalentTo(request.Name);

            // Valida o campo "tokens"
            json.RootElement
                .GetProperty("tokens").GetProperty("accessToken")
                .GetString().ShouldNotBeNullOrEmpty();
        }

        [Theory]
        [ClassData(typeof(CultureInlineDataTest))]
        public async Task Error_Empty_Name(string culture)
        {
            // ARRANGE → Monta os dados necessários para chamar a API
            var request = RequestRegisterUserJsonBuilder.Build();
            request.Name = string.Empty; // criando o erro
  
            // ACT → Envia a requisição "inválida propositalmente" POST para o endpoint User
            var response = await DoPost(method : METHOD, request : request, culture : culture);

            // ASSERT → Valida se o StatusCode retornado foi 400 (BabRequest)
            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

            // Captura o corpo da resposta como texto (a API devolve JSON)
            await using var responseBody = await response.Content.ReadAsStreamAsync();

            // Converte o texto da resposta para objeto JSON
            var json = await JsonDocument.ParseAsync(responseBody);

            // pegando o campo "erros" do JSON e transformando ele em uma lista enumerável
            var errors = json.RootElement.GetProperty("errors").EnumerateArray();

            // Recuperando a mensagem esperada do Resource de acordo com a culture recebida no parâmetro
            // ✔ Busca no arquivo.resx a mensagem correspondente à chave "NAME_EMPTY"
            // ✔ Usando a cultura atual da execução
            var expectedMessage = ResourceMessagesException.ResourceManager.GetString("NAME_EMPTY", new CultureInfo(culture));
           
            errors.ShouldHaveSingleItem()
                .GetString()!.ShouldBe(expectedMessage);      
        }
    }
}
