using Application.Cars.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Api.Modules.Errors;

public static class CarErrorHandler
{
    public static ObjectResult ToObjectResult(this CarException error)
    {
        return new ObjectResult(error.Message)
        {
            StatusCode = error switch
            {
                CarAlreadyExistException => StatusCodes.Status409Conflict,
                CarNotFoundException => StatusCodes.Status404NotFound,
                CarCategoriesNotFoundException => StatusCodes.Status404NotFound,
                CarImageNotFoundException => StatusCodes.Status404NotFound,
                UnhandledCarException => StatusCodes.Status500InternalServerError,
                _ => StatusCodes.Status500InternalServerError
            }
        };
    }
}