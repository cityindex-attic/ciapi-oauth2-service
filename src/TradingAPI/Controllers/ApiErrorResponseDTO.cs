using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Runtime.Serialization;
using CIAPI.DTO;

namespace TradingAPI.Controllers
{
    [Serializable, DataContract]
    public class ApiErrorResponseDTO
    {
        ///<summary>
        /// The intended HTTP status code. This will be the same value as the actual
        /// HTTP status code unless the QueryString contains only200=true.
        /// This is useful for JavaScript clients who
        /// can only read responses with status code 200.
        ///</summary>
        /// <jschema 
        /// demoValue="200"
        /// />
        [DataMember]
        public int HttpStatus { get; set; }

        ///<summary>
        /// This is a description of the ErrorMessage property.
        ///</summary>
        /// <jschema 
        /// demoValue="sample value"
        /// />
        [DataMember]
        public string ErrorMessage { get; set; }

        ///<summary>
        /// The error code.
        ///</summary>
        /// <jschema 
        /// underlyingType="RESTWebServicesDTO.ErrorCode, RESTWebServicesDTO" 
        /// />
        [DataMember]
        public int ErrorCode { get; set; }

        public ApiErrorResponseDTO()
        {

        }

        public ApiErrorResponseDTO(HttpStatusCode httpStatusCode, ErrorCode errorCode, string errorMessage)
        {
            HttpStatus = (int)httpStatusCode;
            ErrorCode = (int)errorCode;
            ErrorMessage = errorMessage;
        }

        public static readonly ApiErrorResponseDTO NullGatewayResponseError = new ApiErrorResponseDTO
            {
                ErrorCode = (int)CIAPI.DTO.ErrorCode.NoDataAvailable,
                ErrorMessage = "Unable to fetch data relevant to your request (received a null response from a backend service)",
                HttpStatus = (int)HttpStatusCode.InternalServerError
            };

        public static readonly ApiErrorResponseDTO InternalServerError = new ApiErrorResponseDTO
            {
                ErrorCode = (int)CIAPI.DTO.ErrorCode.InternalServerError,
                ErrorMessage = "Server error",
                HttpStatus = (int)HttpStatusCode.InternalServerError
            };

        public static readonly ApiErrorResponseDTO Unauthorized = new ApiErrorResponseDTO
            {
                ErrorCode = (int)CIAPI.DTO.ErrorCode.InvalidSession,
                ErrorMessage = "Session is not valid",
                HttpStatus = (int)HttpStatusCode.Unauthorized
            };

        public static readonly ApiErrorResponseDTO InvalidCredentials = new ApiErrorResponseDTO
            {
                ErrorCode = (int)CIAPI.DTO.ErrorCode.InvalidCredentials,
                ErrorMessage = "The credentials used to authenticate are invalid.The supplied password is incorrect.",
                HttpStatus = (int)HttpStatusCode.Unauthorized
            };

        public static readonly ApiErrorResponseDTO InvalidJson = new ApiErrorResponseDTO
            {
                ErrorCode = (int)CIAPI.DTO.ErrorCode.InvalidJsonRequest,
                ErrorMessage = "The request body is invalid",
                HttpStatus = (int)HttpStatusCode.BadRequest
            };

        public static readonly ApiErrorResponseDTO InvalidContentType = new ApiErrorResponseDTO
            {
                ErrorCode = 4005,
                ErrorMessage = "The request content-type is not supported",
                HttpStatus = (int)HttpStatusCode.BadRequest
            };

        public static readonly ApiErrorResponseDTO InvalidJsonCaseFormat = new ApiErrorResponseDTO
            {
                ErrorCode = (int)CIAPI.DTO.ErrorCode.InvalidJsonRequestCaseFormat,
                ErrorMessage = "The request body is invalid. Check your Json is correctly formatted (including the case of the property names)",
                HttpStatus = (int)HttpStatusCode.BadRequest
            };

        public static readonly ApiErrorResponseDTO TooManyRequests = new ApiErrorResponseDTO
            {
                ErrorCode = (int)CIAPI.DTO.ErrorCode.Throttling,
                ErrorMessage = "Request throttled",
                HttpStatus = (int)HttpStatusCode.ServiceUnavailable
            };

        public static readonly ApiErrorResponseDTO InvalidParameterValue = new ApiErrorResponseDTO
            {
                ErrorCode = (int)CIAPI.DTO.ErrorCode.InvalidParameterValue,
                ErrorMessage = "Bad parameter",
                HttpStatus = (int)HttpStatusCode.BadRequest
            };

        public HttpResponseMessage ToHttpResponseMessage()
        {
            return new HttpResponseMessage((HttpStatusCode)HttpStatus)
                {
                    Content = new ObjectContent<ApiErrorResponseDTO>(this, new JsonMediaTypeFormatter())
                };
        }

    }
}