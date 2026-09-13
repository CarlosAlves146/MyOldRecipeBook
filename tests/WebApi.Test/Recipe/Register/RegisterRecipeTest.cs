using Castle.Core.Resource;
using CommonTestUtilities.Requests;
using CommonTestUtilities.Tokens;
using MyRecipeBook.Exceptions;
using Shouldly;
using System.Globalization;
using System.Net;
using System.Text.Json;
using WebApi.Test.InlineData;

namespace WebApi.Test.Recipe.Register
{
    public class RegisterRecipeTest : MyRecipeBookClassFixture
    {
        private readonly string method = "recipe";
        private readonly Guid _userIdentifier;

        public RegisterRecipeTest(CustomWebApplicationFactory factory) : base(factory)
        {
            _userIdentifier = factory.GetUserIdentifier();
        }

        [Fact]
        public async Task Sucess()
        {
            // ARRANGE → Monta os dados necessários para chamar a API
            var request = RequestRecipeJsonBuilder.Build();

            var token = JwtTokenGeneratorBuilder.Build().Generate(_userIdentifier);

            // ACT → Envia a requisição POST para o endpoint User
            var response = await DoPost(method: method, token: token, request: request);

            // ASSERT → Valida se o StatusCode retornado foi 201 (Created)
            response.StatusCode.ShouldBe(HttpStatusCode.Created);

            // Captura o corpo da resposta como texto (a API devolve JSON)
            await using var responseBody = await response.Content.ReadAsStreamAsync();

            // Converte o texto da resposta para objeto JSON
            var json = await JsonDocument.ParseAsync(responseBody);

            // Valida se o campo "name" retornado é igual ao "Name" enviado na request
            json.RootElement
                .GetProperty("title")
                .GetString()
                .ShouldBeEquivalentTo(request.Title);
        }

        [Theory]
        [ClassData(typeof(CultureInlineDataTest))]
        public async Task Error_Empty_Title(string culture)
        {
            // ARRANGE → Monta os dados necessários para chamar a API
            var request = RequestRecipeJsonBuilder.Build();
            request.Title = string.Empty;

            var token = JwtTokenGeneratorBuilder.Build().Generate(_userIdentifier);

            // ACT → Envia a requisição POST para o endpoint User
            var response = await DoPost(method: method, token: token, request: request, culture : culture);

            // ASSERT → Valida se o StatusCode retornado foi 400 (Created)
            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

            // Captura o corpo da resposta como texto (a API devolve JSON)
            await using var responseBody = await response.Content.ReadAsStreamAsync();

            // Converte o texto da resposta para objeto JSON
            var json = await JsonDocument.ParseAsync(responseBody);
            Console.WriteLine(json);
            // Aqui pegamos a propriedade "errors" do JSON e obtemos acesso aos
            // itens do array para poder validar cada mensagem de erro retornada pela API.
            var errors = json.RootElement.GetProperty("errors").EnumerateArray();

            // Busca no arquivo de recursos a mensagem esperada para a chave
            // RECIPE_TITLE_EMPTY no idioma informado pelo teste.
            var expectedMessage = ResourceMessagesException.ResourceManager.GetString("RECIPE_TITLE_EMPTY", new CultureInfo(culture));

            // Confirma que a API retornou apenas um erro e que a mensagem
            // desse erro é exatamente a mensagem esperada no idioma do teste.
            errors.ShouldHaveSingleItem()
                .GetString()!.ShouldBe(expectedMessage);
            // *O item retornado é um "JsonElement" então GetString extrai o texto dele
        }
    }
}
