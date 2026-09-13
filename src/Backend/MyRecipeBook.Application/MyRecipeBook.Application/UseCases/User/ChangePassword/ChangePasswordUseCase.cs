using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Domain.Extension;
using MyRecipeBook.Domain.Repositories;
using MyRecipeBook.Domain.Repositories.User;
using MyRecipeBook.Domain.Security.Encrypt;
using MyRecipeBook.Domain.Services.LoggedUser;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionBase;

namespace MyRecipeBook.Application.UseCases.User.ChangePassword
{
    public class ChangePasswordUseCase : IChangePasswordUseCase
    {
        private readonly ILoggedUser _loggedUser;
        private readonly IUserUpdateOnlyRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordEncrypter _passwordEncrypter;
  
        public ChangePasswordUseCase(
            ILoggedUser loggedUser, 
            IUserUpdateOnlyRepository repository, 
            IUnitOfWork unitOfWork, 
            IPasswordEncrypter passwordEncrypter)
        {
            _loggedUser = loggedUser;
            _repository = repository;
            _unitOfWork = unitOfWork;
            _passwordEncrypter = passwordEncrypter;
        }
        public async Task Execute(RequestChangePasswordJson request)
        {
            // Usuário Logado
            var loggedUser = await _loggedUser.User();

            // Validação
            Validate(request, loggedUser);

            // Concluindo a atualização
            var user = await _repository.GetById(loggedUser.Id);
            user.Password = _passwordEncrypter.Encrypt(request.NewPassword);

            // Preparando e salvando no Bd
            _repository.Update(user);
            await _unitOfWork.Commit();                    
        }
        private void Validate(RequestChangePasswordJson request, Domain.Entities.User loggedUser)
        {
            // Create validator
            var validator = new ChangePasswordValidator();
            var result = validator.Validate(request);

            //Criptografando a password atual
            var currentPasswordEncripted = _passwordEncrypter.Encrypt(request.CurrentPassword);

            // Verificando se a password atual é igual a password salva no bd
            if (currentPasswordEncripted.Equals(loggedUser.Password).IsFalse())
            {
                result.Errors.Add(new FluentValidation.Results.ValidationFailure("Password", ResourceMessagesException.PASSWORD_DIFFERENT_CURRENT_PASSWORD));
            }

            // verificando se o result é False.
            if (result.IsValid.IsFalse())
            {
                throw new ErrorOnValidationException(result.Errors.Select(e => e.ErrorMessage).ToList());
            }
        }
    }
}
