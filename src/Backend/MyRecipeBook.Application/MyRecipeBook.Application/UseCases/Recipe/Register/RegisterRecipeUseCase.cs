using AutoMapper;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Communication.Responses;
using MyRecipeBook.Domain.Extension;
using MyRecipeBook.Domain.Repositories;
using MyRecipeBook.Domain.Repositories.Recipe;
using MyRecipeBook.Domain.Services.LoggedUser;
using MyRecipeBook.Exceptions.ExceptionBase;

namespace MyRecipeBook.Application.UseCases.Recipe.Register
{
    public class RegisterRecipeUseCase : IRegisterRecipeUseCase
    {
        private readonly ILoggedUser _loggedUser;
        private readonly IRecipeWriteOnlyRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        
        public RegisterRecipeUseCase(
            ILoggedUser loggedUser, 
            IRecipeWriteOnlyRepository repository,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _loggedUser = loggedUser;
            _repository = repository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<ResponsesRegisteredRecipeJson> Execute(RequestRecipeJson request)
        {
            Validate(request);
            var user = await _loggedUser.User();

            var recipe = _mapper.Map<Domain.Entities.Recipe>(request);
            recipe.UserId = user.Id;       
            
            // Ordenando a list de obj com base na propriedade Step da request.Instructions
            var instructions = request.Instructions.OrderBy(i => i.Step).ToList();          

            // Redefinindo os valores de Step para garantir uma sequência contínua e ordenada.
            for (var index = 0; index < instructions.Count; index++)
            {
                instructions.ElementAt(index).Step = index + 1;
            }

            // Agora estamos utilizando o AutoMapper para converter a lista de
            // RequestInstructionJson em uma lista de Domain.Entities.Instruction.
            // Nesse processo, os dados já organizados anteriormente são transferidos
            // para os objetos do domínio da aplicação. * Lembre-se que é preciso configurar o createMap no Mapper
            recipe.Instructions = _mapper.Map<IList<Domain.Entities.Instruction>>(instructions);

            await _repository.Add(recipe);                    
            await _unitOfWork.Commit();

            return _mapper.Map<ResponsesRegisteredRecipeJson>(recipe);
        }
        private static void Validate(RequestRecipeJson request)
        {
            var result = new RecipeValidator().Validate(request);
            if (result.IsValid.IsFalse()) 
            {
                throw new ErrorOnValidationException(result.Errors.Select(e => e.ErrorMessage).ToList());
            }      
        }
    }
}
