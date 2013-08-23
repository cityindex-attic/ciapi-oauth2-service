using System;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Filters;
using Salient.ReflectiveLoggingAdapter;

namespace TradingAPI.Controllers
{
    public class TradingApiExceptionFilterAttribute : ExceptionFilterAttribute
    {


        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            
            if (actionExecutedContext.Exception is HttpResponseException)
            {
                return;
            }

                        
            // {"HttpStatus":401,"ErrorMessage":"The credentials used to authenticate are invalid.  Either the username, password or both are incorrect.","ErrorCode":4010}

            if (actionExecutedContext.Exception is CIAPI.Rpc.InvalidCredentialsException)
            {
                actionExecutedContext.Response = new ApiErrorResponseDTO()
                {
                    ErrorCode = 4010,
                    HttpStatus = 401,
                    ErrorMessage = "The credentials used to authenticate are invalid.  Either the username, password or both are incorrect."
                }.ToHttpResponseMessage();
                return;
            }
            if (actionExecutedContext.Exception is CIAPI.Rpc.InvalidSessionException)
            {
                // {"HttpStatus":401,"ErrorMessage":"Session is not valid","ErrorCode":4011}
                actionExecutedContext.Response = new ApiErrorResponseDTO()
                    {
                        ErrorCode = 4011,
                        HttpStatus = 401,
                        ErrorMessage = "Session is not valid"
                    }.ToHttpResponseMessage();
                return;
            }

            actionExecutedContext.Response = ApiErrorResponseDTO.InternalServerError.ToHttpResponseMessage();
            
            
        }




    }
}