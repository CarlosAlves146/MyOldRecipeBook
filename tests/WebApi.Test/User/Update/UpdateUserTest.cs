using CommonTestUtilities.Requests;
using CommonTestUtilities.Tokens;
using MyRecipeBook.Communication.Responses;
using MyRecipeBook.Exceptions;
using Shouldly;
using System.Globalization;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using WebApi.Test.InlineData;

namespace WebApi.Test.User.Update
{
    public class UpdateUserTest : MyRecipeBookClassFixture
    {
        private const string METHOD = "user";

        private readonly Guid _userIdentifier;

        public UpdateUserTest(CustomWebApplicationFactory factory) : base(factory)
        {
            _userIdentifier = factory.GetUserIdentifier();
        }
        [Fact]
        public async Task Sucess()
        {
            // ARRANGE → Monta os dados necessários para chamar a API
            var request = RequestUpdateUserJsonBuilder.Build();

            var token = JwtTokenGeneratorBuilder.Build().Generate(_userIdentifier);

            // ACT → Envia a requisição POST para o endpoint User
            var response = await DoPut(method : METHOD, request : request, token : token);

            // ASSERT → Valida se o StatusCode retornado foi 204 (No Content)
            response.StatusCode.ShouldBe(HttpStatusCode.NoContent);

            // Buscar novamente para conferir se os dados foram trocados
            var profileGet = await DoGet(method : METHOD, token : token);

            var user = await profileGet.Content.ReadFromJsonAsync<ResponseUserProfileJson>();

            user!.Name.ShouldBe(request.Name);
            user.Email.ShouldBe(request.Email);
        }

        [Theory]
        [ClassData(typeof(CultureInlineDataTest))]
        public async Task Error_Empty_Name(string culture)
        {
            // ARRANGE → Monta os dados necessários para chamar a API
            var request = RequestUpdateUserJsonBuilder.Build();
            request.Name = string.Empty;

            var token = JwtTokenGeneratorBuilder.Build().Generate(_userIdentifier);

            // ACT → Envia a requisição POST para o endpoint User
            var response = await DoPut(method: METHOD, request : request, token : token, culture : culture);

            // ASSERT → Valida se o StatusCode retornado BadRequest
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
