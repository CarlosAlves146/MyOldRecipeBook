using CommonTestUtilities.Requests;
using CommonTestUtilities.Tokens;
using Shouldly;
using System.Net;

namespace WebApi.Test.User.ChangePassword
{
    public class ChangePasswordInvalidToken : MyRecipeBookClassFixture
    {
        // variável método utilizado
        private readonly string METHOD = "user";
        public ChangePasswordInvalidToken(CustomWebApplicationFactory webApplication) : base(webApplication)
        {
        }
        [Fact]
        public async Task Error_Token_Invalid()
        {
            // Arrange/Preparar
            var request = RequestChangePasswordJsonBuilder.Build();
            var token = "tokenInvalid";

            // Act/Agir
            var response = await DoPut(method : METHOD, request : request, token : token);

            // Assert -> Verificar/Validar
            response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
            response.Content.ShouldNotBeNull();
        }

        [Fact]
        public async Task Error_Without_Token()
        {
            // Arrange/Preparar
            var request = RequestChangePasswordJsonBuilder.Build();
            var token = string.Empty;

            // Act/Agir
            var response = await DoPut(method : METHOD, request: request, token: token);

            // Assert -> Verificar/Validar
            response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
            response.Content.ShouldNotBeNull();
        }

        [Fact]
        public async Task Error_Token_With_User_NotFound()
        {
            // Arrange/Preparar
            var request = RequestChangePasswordJsonBuilder.Build();
            // gerando um token real porém inexistente
            var token = JwtTokenGeneratorBuilder.Build().Generate(Guid.NewGuid());

            // Act/Agir
            var response = await DoPut(method : METHOD, request : request, token : token);

            // Assert -> Verificar/Validar
            response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
            response.Content.ShouldNotBeNull();
        }
    }
}
