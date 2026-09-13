using CommonTestUtilities.Requests;
using CommonTestUtilities.Tokens;
using Google.Protobuf.WellKnownTypes;
using Shouldly;
using System.Net;

namespace WebApi.Test.Recipe.Register
{
    public class RegisterRecipeInvalidTokenTest : MyRecipeBookClassFixture
    {
        // variável método utilizado
        private readonly string METHOD = "recipe";
        public RegisterRecipeInvalidTokenTest(CustomWebApplicationFactory webApplication) : base(webApplication)
        {   
        }
        [Fact]
        public async Task Error_Token_Invalid()
        {
            // Arrange/Preparar
            var request = RequestRecipeJsonBuilder.Build();

            // Act/Agir
            var response = await DoPost(method: METHOD, request : request, token: "tokenInvalid");

            // Assert -> Verificar/Validar
            response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
            response.Content.ShouldNotBeNull();
        }

        [Fact]
        public async Task Error_Without_Token()
        {
            // Arrange/Preparar
            var request = RequestRecipeJsonBuilder.Build();

            // Act/Agir
            var response = await DoPost(method: METHOD, request: request, token: string.Empty);

            // Assert -> Verificar/Validar
            response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
            response.Content.ShouldNotBeNull();
        }

        [Fact]
        public async Task Error_Token_With_User_NotFound()
        {
            // Arrange/Preparar
            var request = RequestRecipeJsonBuilder.Build();
            var token = JwtTokenGeneratorBuilder.Build().Generate(Guid.NewGuid());

            // Act/Agir
            var response = await DoPost(method: METHOD, request: request, token : token);

            // Assert -> Verificar/Validar
            response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
            response.Content.ShouldNotBeNull();
        }
    }
}
