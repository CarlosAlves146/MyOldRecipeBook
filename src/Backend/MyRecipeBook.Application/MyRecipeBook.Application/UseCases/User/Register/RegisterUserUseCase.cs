using AutoMapper;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Communication.Responses;
using MyRecipeBook.Domain.Extension;
using MyRecipeBook.Domain.Repositories;
using MyRecipeBook.Domain.Repositories.User;
using MyRecipeBook.Domain.Security.Encrypt;
using MyRecipeBook.Domain.Security.Tokens;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionBase;

namespace MyRecipeBook.Application.UseCases.User.Register
{
    public class RegisterUserUseCase : IRegisterUserUseCase
    {
        // 🔹 Campos que serão injetados via Injeção de Dependência (Dependency Injection)

        // 1. A classe RegisterUserUseCase declara que depende das seguintes interfaces:
        //    - IUserWriteOnlyRepository: responsável por salvar dados de usuários.
        //    - IUserReadOnlyRepository: responsável por consultar dados de usuários.
        //    - IMapper: faz mapeamento entre objetos (por exemplo, entre DTOs e entidades).
        //    - PasswordEncripter: utilitário para encriptar senhas (classe concreta, não interface).

        // 2. No Program.cs, o método AddInfrastructure() registra essas dependências:
        //    Ele informa ao container qual classe concreta deve ser usada para cada interface.
        //    Exemplo: IUserWriteOnlyRepository será implementado pela classe UserRepository.

        // 3. Quando a aplicação é iniciada, o ASP.NET Core configura o container de injeção de dependência.
        //    Sempre que RegisterUserUseCase for instanciada, o container irá:
        //    - Criar as instâncias corretas (UserRepository, Mapper, PasswordEncripter, etc.),
        //    - E injetar automaticamente nos campos via o construtor da classe.
        private readonly IUserWriteOnlyRepository _writeOnlyRepository;
        private readonly IUserReadOnlyRepository _readOnlyRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IPasswordEncrypter _passwordEncripter;
        private readonly IAccessTokenGenerator _accessTokenGenerator;

        // Construtor
        public RegisterUserUseCase(
            IUserWriteOnlyRepository writeOnlyRepository,
            IUserReadOnlyRepository readOnlyRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IPasswordEncrypter passwordEncripter,
            IAccessTokenGenerator accessTokenGenerator
            )
        {
            _writeOnlyRepository = writeOnlyRepository;
            _readOnlyRepository = readOnlyRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _passwordEncripter = passwordEncripter;
            _accessTokenGenerator = accessTokenGenerator;
        }

        // Método que vai retornar valores dos tipos "ResponsesRegisteredUserJson"
        public async Task<ResponsesRegisteredUserJson> Execute(RequestRegisterUserJson request)
        {
            // Validação do request
            await Validate(request);

            // Realizando de fato o mapeamento:
            // 2 - "AutoMapper, quero transformar esse request (que é do tipo RequestRegisterUserJson) em um User."
            var user = _mapper.Map<Domain.Entities.User>(request);

            // criptografando a senha
            user.Password = _passwordEncripter.Encrypt(request.Password);

            // Criação do UserIdentifier - Guid, para tokens
            user.UserIdentifier = Guid.NewGuid();

            // criando o jti, porém ainda não está sendo salvo no banco de dados
            // var jti = Guid.NewGuid().ToString();
            
            await _writeOnlyRepository.Add(user);// Preparando o Bd
            await _unitOfWork.Commit();          // Persistindo no Bd

            // Caso o registro seja realizado de forma satisfatória
            // Retornaremos aqui.
            return new ResponsesRegisteredUserJson
            {
                Name = user.Name,
                Tokens = new ResponseTokensJson
                {
                    // gerando um access token no return
                    AccessToken = _accessTokenGenerator.Generate(user.UserIdentifier)
                }
            };
        }
        // Método para validação
        private async Task Validate(RequestRegisterUserJson request)
        {
            // Criamos uma nova inst. de RegisterUser... class que contem as válidações.
            var validator = new RegisterUserValidator();

            // Chamamos o método Validate da biblioteca. E ficará armazenado em "result" o resultado da nossas validações.
            // e dentro de "result" podemos verificar a propriedade IsValid que vai nos retornar true se a validação deu tudo certo ou false
            // se a validação teve alguma falha.
            var result = validator.Validate(request);

            // Aqui estamos verificando se já existe o e-mail enviado na requisição em nosso banco de dados.
            var emailExist = await _readOnlyRepository.ExistActiveUserWithEmail(request.Email);
            if(emailExist)
            {
               result.Errors.Add(new FluentValidation.Results.ValidationFailure(string.Empty, ResourceMessagesException.EMAIL_ALREADY_REGISTER));
            }

            // Se false, lançamos a Exception.
            if (result.IsValid.IsFalse()) 
            {
                // Desta forma estamos coletando somente as strings das msgs de erros
                // posteriormente adicionamos o método To.list para personalizamos nossas Exceptions
                var errorMessages = result.Errors.Select(e => e.ErrorMessage).ToList();

                // Aqui lançamos um (throw) uma exceção chamada ErrorOnValidationException e "criando uma nova instância" dela, que é uma
                // class que criamos, no projeto de Exceptions e herda de MyRecipeBookException que por sua vez herda de SystemException
                // que é uma classe base do .NET para todas as exceções geradas pelo sistema.
                // Ela serve como categoria para erros que acontecem dentro do próprio ambiente do .NET, como problemas de memória, falhas etc.
                // A partir daqui INTERROMPEMOS O FLUXO NORMAL DO PROGRAMA, e sinalizamos um ERRO.
                // Passamos como parâmetro a lista de mensagens de erro.
                throw new ErrorOnValidationException(errorMessages);
            }
        }
    }
}
