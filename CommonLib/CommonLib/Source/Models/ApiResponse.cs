﻿using System;
using System.Linq;
using CommonLib.Source.Models.Interfaces;

namespace CommonLib.Source.Models
{
    public class ApiResponse : ApiResponse<object>
    {
        public ApiResponse() : base() { }
        public ApiResponse(StatusCodeType statusCode, string message, ILookup<string, string> validationMessages) : base(statusCode, message, validationMessages) { }
        public ApiResponse(StatusCodeType statusCode, string message, ILookup<string, string> validationMessages, object result) : base(statusCode, message, validationMessages, result) { }
        public ApiResponse(StatusCodeType statusCode, string message, ILookup<string, string> validationMessages, object result, Exception responseException) : base(statusCode, message, validationMessages, result, responseException) { }
    }
    // https://localhost:44396/Account/ConfirmEmail?email=rvnlord@gmail.com&code=xxx

    public class ApiResponse<T> : IApiResponse
    {
        public StatusCodeType StatusCode { get; set; }
        public bool IsError => (int) StatusCode > 299;
        public string Message { get; set; }
        public Exception ResponseException { get; set; }
        public T Result { get; set; }
        public ILookup<string, string> ValidationMessages { get; set; }
        object IApiResponse.Result { get => Result; set => Result = (T) value; }

        public ApiResponse() { }

        public ApiResponse(StatusCodeType statusCode, string message, ILookup<string, string> validationMessages)
        {
            StatusCode = statusCode;
            Message = message;
            ValidationMessages = validationMessages;
        }

        public ApiResponse(StatusCodeType statusCode, string message, ILookup<string, string> validationMessages, T result)
        {
            StatusCode = statusCode;
            Message = message;
            Result = result;
            ValidationMessages = validationMessages;
        }

        public ApiResponse(StatusCodeType statusCode, string message, ILookup<string, string> validationMessages, T result, Exception responseException)
        {
            StatusCode = statusCode;
            Message = message;
            Result = result;
            ResponseException = responseException;
            ValidationMessages = validationMessages;
        }

        public ApiResponse(StatusCodeType statusCode, string message, Exception responseException)
        {
            StatusCode = statusCode;
            Message = message;
            ResponseException = responseException;
        }

        public ApiResponse(StatusCodeType statusCode, string message)
        {
            StatusCode = statusCode;
            Message = message;
        }

        public ApiResponse(StatusCodeType statusCode, string message, T result)
        {
            StatusCode = statusCode;
            Message = message;
            Result = result;
        }

        public ApiResponse(T result)
        {
            StatusCode = StatusCodeType.Status200OK;
            Result = result;
        }

        public ApiResponse(string message, T result)
        {
            StatusCode = StatusCodeType.Status200OK;
            Result = result;
            Message = message;
        }
    }

    public enum StatusCodeType
    {
        Status100Continue = 100,
        Status412PreconditionFailed = 412,
        Status413PayloadTooLarge = 413,
        Status414RequestUriTooLong = 414,
        Status415UnsupportedMediaType = 415,
        Status416RequestedRangeNotSatisfiable = 416,
        Status417ExpectationFailed = 417,
        Status418ImATeapot = 418,
        Status419AuthenticationTimeout = 419,
        Status421MisdirectedRequest = 421,
        Status422UnprocessableEntity = 422,
        Status423Locked = 423,
        Status424FailedDependency = 424,
        Status426UpgradeRequired = 426,
        Status428PreconditionRequired = 428,
        Status429TooManyRequests = 429,
        Status431RequestHeaderFieldsTooLarge = 431,
        Status451UnavailableForLegalReasons = 451,
        Status500InternalServerError = 500,
        Status501NotImplemented = 501,
        Status502BadGateway = 502,
        Status503ServiceUnavailable = 503,
        Status504GatewayTimeout = 504,
        Status505HttpVersionNotsupported = 505,
        Status506VariantAlsoNegotiates = 506,
        Status507InsufficientStorage = 507,
        Status508LoopDetected = 508,
        Status411LengthRequired = 411,
        Status510NotExtended = 510,
        Status410Gone = 410,
        Status408RequestTimeout = 408,
        Status101SwitchingProtocols = 101,
        Status102Processing = 102,
        Status200OK = 200,
        Status201Created = 201,
        Status202Accepted = 202,
        Status203NonAuthoritative = 203,
        Status204NoContent = 204,
        Status205ResetContent = 205,
        Status206PartialContent = 206,
        Status207MultiStatus = 207,
        Status208AlreadyReported = 208,
        Status226IMUsed = 226,
        Status300MultipleChoices = 300,
        Status301MovedPermanently = 301,
        Status302Found = 302,
        Status303SeeOther = 303,
        Status304NotModified = 304,
        Status305UseProxy = 305,
        Status306SwitchProxy = 306,
        Status307TemporaryRedirect = 307,
        Status308PermanentRedirect = 308,
        Status400BadRequest = 400,
        Status401Unauthorized = 401,
        Status402PaymentRequired = 402,
        Status403Forbidden = 403,
        Status404NotFound = 404,
        Status405MethodNotAllowed = 405,
        Status406NotAcceptable = 406,
        Status407ProxyAuthenticationRequired = 407,
        Status409Conflict = 409,
        Status511NetworkAuthenticationRequired = 511
    }
}
