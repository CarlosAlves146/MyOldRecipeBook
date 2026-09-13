using CommonTestUtilities.Tokens;
using Shouldly;
using System.Net;

namespace WebApi.Test.User.Profile
{
    public class GetUserProfileInvalidTokenTest : MyRecipeBookClassFixture
    {
        private readonly string METHOD = "user";
  
        public GetUserProfileInvalidTokenTest(CustomWebApplicationFactory factory) : base(factory) { }
        
        [Fact]
        public async Task Error_Token_Invalid()
        {
            // ACT → Envia a requisição GET para o endpoint User
            // utilizando argumento nomeado enviando um "token Inválido"
            var response = await DoGet(method : METHOD, token: "tokenInvalid");

            // ASSERT → Valida se o StatusCode retornado foi 401 (Unauthorized)
            response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
        }
        [Fact]
        public async Task Error_Token_Without_Token()
        {
            // ACT → Envia a requisição GET para o endpoint User
            // utilizando argumento nomeado enviando um token "vazio"
            var response = await DoGet(method : METHOD, token: string.Empty);

            // ASSERT → Valida se o StatusCode retornado foi 401 (Unauthorized)
            response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Error_Token_With_User_NotFound()
        {
            // ARRANGE → Monta os dados necessários para chamar a API
            var token = JwtTokenGeneratorBuilder.Build().Generate(Guid.NewGuid());

            // ACT → Envia a requisição GET para o endpoint User
            // utilizando argumento nomeado enviando um token "valido porém usuário não autenticado"
            var response = await DoGet(method : METHOD, token : token);

            // ASSERT → Valida se o StatusCode retornado foi 401 (Unauthorized)
            response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
        }
    }
}
