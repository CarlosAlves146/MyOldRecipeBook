using CommonTestUtilities.Requests;
using CommonTestUtilities.Tokens;
using Google.Protobuf.WellKnownTypes;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Communication.Responses;
using MyRecipeBook.Exceptions;
using Shouldly;
using System.Globalization;
using System.Net;
using System.Text.Json;
using WebApi.Test.InlineData;

namespace WebApi.Test.User.ChangePassword
{
    public class ChangePasswordTest : MyRecipeBookClassFixture
    {
        // variável método utilizado
        private readonly string METHOD = "user/change-password";

        private readonly Guid _userIdentifier;
        private readonly string _userEmail;
        private readonly string _password;
        public ChangePasswordTest(CustomWebApplicationFactory factory) : base(factory)
        {
            _userIdentifier = factory.GetUserIdentifier();
            _userEmail = factory.GetEmail();
            _password = factory.GetPassword();
        }
        [Fact]
        public async Task Sucess()
        {
            // ARRANGE → Monta os dados necessários para chamar a API
            var request = RequestChangePasswordJsonBuilder.Build();
            request.CurrentPassword = _password;

            var token = JwtTokenGeneratorBuilder.Build().Generate(_userIdentifier);

            // ACT → Envia a requisição POST para o endpoint User
            var response = await DoPut(method : METHOD, request: request, token : token);

            // ASSERT → Valida se o StatusCode retornado foi 204 (No Content)
            response.StatusCode.ShouldBe(HttpStatusCode.NoContent);

            var loginRequest = new RequestLoginJson
            {
                Email = _userEmail,
                Password = _password
            };
            response = await DoPost(method : "login", request : loginRequest);
            response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);

            loginRequest.Password = request.NewPassword;

            response = await DoPost("login", request : loginRequest);
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
        }

        [Theory]
        [ClassData(typeof(CultureInlineDataTest))]
        public async Task Error_NewPassword_Empty(string culture)
        {
            // ARRANGE → Monta os dados necessários para chamar a API
            var request = new RequestChangePasswordJson
            {
                CurrentPassword = _password,
                NewPassword = string.Empty
            };
            
            var token = JwtTokenGeneratorBuilder.Build().Generate(_userIdentifier);

            // ACT → Envia a requisição POST para o endpoint User
            var response = await DoPut(method: METHOD, request: request, token: token, culture : culture);

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
            var expectedMessage = ResourceMessagesException.ResourceManager.GetString("PASSWORD_EMPTY", new CultureInfo(culture));

            errors.ShouldHaveSingleItem()
                .GetString()!.ShouldBe(expectedMessage);
        }
    }
}
