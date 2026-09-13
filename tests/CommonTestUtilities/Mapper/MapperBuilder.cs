using AutoMapper;
using CommonTestUtilities.IdEncrypter;
using MyRecipeBook.Application.Services.AutoMapper;

namespace CommonTestUtilities.Mapper
{
    public class MapperBuilder
    {
        public static IMapper Build()
        {
            var idEncripter = IdEncrypterBuilder.Build();

            return new AutoMapper.MapperConfiguration(options =>
            {
                options.AddProfile(new AutoMapping(idEncripter)); // Define as regras/configurações de mapeamento que configuramos
            }).CreateMapper();
        }
    }
}
