using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MyRecipeBook.Communication.Responses;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionBase;
using System.Net;

namespace MyRecipeBook.API.Filters
{
    
    public class ExceptionFilter : IExceptionFilter
    {
        // Esse é o método específico da nossa interface, ele sera executado automaticamente sempre que
        // ocorrer uma exceção (Erro) durante a execução de um Controller.
        public void OnException(ExceptionContext context)
        {
            // Em nosso if estamos verificando se o contexto da exception está em
            // MyRecipeBookException e se for uma Exception lançada por nós será true
            // como.
            if (context.Exception is MyRecipeBookException)
            {
                // caso seja true chamaremos o método:
                // e nesse, método teremos nossas trativas para cada Exception que criarmos, acredito que faremos isso
                // nas proximas aulas.
                HandleProjectException(context);
            }
            else
            {
                // Agora aqui caso a Exception lançada não seja uma especifica nossa
                // lançaremos ai sim um internal erro StatusCode 500 e com a msg de erro desconhecido UNKNOWN_ERROR.
                ThrowUnknowException(context);
            }
        }
        private static void HandleProjectException(ExceptionContext context)
        {
            if (context.Exception is InvalidLoginException)
            {                
                // StatusCode 401
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;

                context.Result = new UnauthorizedObjectResult(new ResponseErrorJson(context.Exception.Message));
               
            }
            else if (context.Exception is ErrorOnValidationException)
            {
                // Como identificamos que é do tipo ErrorOnValidationException sabemos que obrigatoriamente
                // essa classe gerou uma lista em seu construtor

                // Aqui estamos criando uma variável e fazendo um cast, ou seja estamos tentando converter o objeto para
                // o tipo especificado, e nítidamente sempre vamos conseguir pq já fizemos nossa condicional no if,
                // isso é feito por que precisamos pegar as msg que foram lançadas na nossa Exception e enviar como parametro para nossa
                // instancia de ResponseErrorJson
                var exception = context.Exception as ErrorOnValidationException;                                                                                

                // StatusCode 400
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;

                context.Result = new BadRequestObjectResult(new ResponseErrorJson(exception!.ErrorMessages));
            }
        }

        private static void ThrowUnknowException(ExceptionContext context)
        {
            // StatusCode 500
            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            // aqui lançamos a msg customizada lá da nossa MyRecipeBook.Exceptions, dos nossos arquivos Resources
            context.Result = new ObjectResult(new ResponseErrorJson(ResourceMessagesException.UNKNOWN_ERROR));
        }
    }
}
