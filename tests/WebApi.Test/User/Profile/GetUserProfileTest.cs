using CommonTestUtilities.Tokens;
using MyRecipeBook.Communication.Requests;
using Shouldly;
using System.Net;
using System.Text.Json;

namespace WebApi.Test.User.Profile
{
    public class GetUserProfileTest : MyRecipeBookClassFixture
    {
        private readonly string METHOD = "user";

        private readonly string _name;
        private readonly string _email;
        private readonly Guid _userIdentifier;
        public GetUserProfileTest(CustomWebApplicationFactory factory) : base(factory) 
        {
            _name = factory.GetName();
            _email = factory.GetEmail();
            _userIdentifier = factory.GetUserIdentifier();
        }

        [Fact]
        public async Task Success()
        {
            // ARRANGE → Monta os dados necessários para chamar a API
            var token = JwtTokenGeneratorBuilder.Build().Generate(_userIdentifier);

            // ACT → Envia a requisição GET para o endpoint User
            // utilizando argumento nomeado
            var response = await DoGet(method : METHOD, token : token);

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
                .GetProperty("email")
                .GetString()
                .ShouldBeEquivalentTo(_email);
        }
    }
}
