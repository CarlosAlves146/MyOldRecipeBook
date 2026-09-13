using AutoMapper;
using MyRecipeBook.Communication.Enums;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Communication.Responses;
using Sqids;

namespace MyRecipeBook.Application.Services.AutoMapper
{
    public class AutoMapping : Profile
    {
        // Criptografia dos IDs
        private readonly SqidsEncoder<long> _idEnconder;

        //Construtor
        public AutoMapping(SqidsEncoder<long> idEnconder) 
        {
            _idEnconder = idEnconder;

            RequestToDomain();
            DomainToResponse();
        }
        private void RequestToDomain()
        {
            // Nosso primeiro parâmetro aqui será:
            // - De onde está vindo os meus dados, a origem.
            // E o segundo parametro o destino:
            // - Qual a classe que irá receber esses dados.

            // Agora vamos usar uma função ForMember() para definir duas coisas para ele:
            // "Quando estiver copiando os dados do objeto RequestRegisterUserJson para o objeto User, ignore o campo Password do destino (User)."
            //Obs: é aqui /\ Se você quiser aplicar alguma transformação em um campo ao fazer o mapeamento,
            // é nesse mesmo local — dentro do CreateMap(...) — que você faz isso, usando o método ForMember.
            CreateMap<RequestRegisterUserJson, Domain.Entities.User>()
            .ForMember(dest => dest.Password, opt => opt.Ignore());

            CreateMap<RequestRecipeJson, Domain.Entities.Recipe>()
                .ForMember(dest => dest.Instructions, opt => opt.Ignore())
                // Ok, tenho uma lista de string e preciso transformar em uma lista de Ingredient... será que existe um mapping de string → Ingredient
                .ForMember(dest => dest.Ingredient, opt => opt.MapFrom(source => source.Ingredient.Distinct()))
                // Ok, tenho uma lista de enum e preciso transformar em uma lista de DishType... será que existe um mapping de enum → DishType
                .ForMember(dest => dest.DishType, opt => opt.MapFrom(source => source.DishType.Distinct()));

            //“Durante o mapping, quando o AutoMapper precisar converter uma string em um objeto do tipo Ingredient,
            // ele criará um novo Ingredient e atribuirá o valor da string à propriedade "Item" desse objeto.”
            CreateMap<string, Domain.Entities.Ingredient>()
                .ForMember(dest => dest.Item, opt => opt.MapFrom(source => source));

            //“Durante o mapping, quando o AutoMapper precisar converter um enum DishType em um objeto do tipo Domain.Entities.DishType,
            // ele criará um novo objeto e atribuirá o valor do enum à propriedade Type desse objeto.”
            CreateMap<DishType, Domain.Entities.DishType>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(source => source));

            CreateMap<RequestInstructionJson, Domain.Entities.Instruction>();
        }
        private void DomainToResponse()
        {
            CreateMap<Domain.Entities.User, ResponseUserProfileJson>();

            CreateMap<Domain.Entities.Recipe, ResponsesRegisteredRecipeJson>()
                .ForMember(dest => dest.Id, config => config.MapFrom(source => _idEnconder.Encode(source.Id)));

            // Response Recipe Filter
            CreateMap<Domain.Entities.Recipe, ResponseShortsRecipeJson>()
                .ForMember(dest => dest.Id, config => config.MapFrom(source => _idEnconder.Encode(source.Id)))
                .ForMember(dest => dest.Title, config => config.MapFrom(source => source.Title.ToUpper()))
                .ForMember(dest => dest.AmountIngredients, config => config.MapFrom(source => source.Ingredient.Count));

            
        }
    }
}
