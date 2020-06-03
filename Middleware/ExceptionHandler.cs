using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using CoviIDApiCore.Exceptions;
using CoviIDApiCore.V1.Constants;
using CoviIDApiCore.V1.DTOs.System;
using CoviIDApiCore.V1.Interfaces.Services;
using Newtonsoft.Json.Serialization;
using Sentry;

namespace CoviIDApiCore.Middleware
{
    public class ExceptionHandler
    {
        private readonly RequestDelegate _next;
        private readonly IAuthenticationService _authentication;

        public ExceptionHandler(RequestDelegate next, IAuthenticationService authentication)
        {
            _next = next;
            _authentication = authentication;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                if (!_authentication.IsAuthorized(context.Request.Headers["x-api-key"])
                    && context.Request.Path != UrlConstants.PartialRoutes[UrlConstants.Routes.Organisation]
                    && context.Request.Path != UrlConstants.PartialRoutes[UrlConstants.Routes.Health])
                    throw new UnauthorizedAccessException();

                await _next(context);
            }
            catch (ValidationException e)
            {
                await HandleValidationException(context, e);
            }
            catch (NotFoundException e)
            {
                await HandleNotFoundException(context, e);
            }
            catch (StreetCredBrokerException e)
            {
                await HandleStreetCredBrokerException(context, e);
            }
            catch (SendGridException e)
            {
                await HandleSendGridException(context, e);
            }
            catch (ClickatellException e)
            {
                await HandleClickatellException(context, e);
            }
            catch (AmazonS3Exception e)
            {
                await HandleAmazonS3Exception(context, e);
            }
            catch (QRException e)
            {
                await HandleQRException(context, e);
            }
            catch (UnauthorizedAccessException e)
            {
                await HandleUnauthorisedException(context, e);
            }
            catch (BitlyException e)
            {
                await HandleBitlyException(context, e);
            }
            catch (Exception e)
            {
                await HandleUnexpectedException(context, e);
            }
        }
    
        #region Exception Handler Methods

        private static Task HandleValidationException(HttpContext context, Exception e)
        {
            var code = HttpStatusCode.BadRequest;

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int) code;

            var rsp = new Response(false, code, e.Message);

            return ReturnResult(context, rsp);
        }

        private static Task HandleNotFoundException(HttpContext context, Exception e)
        {
            var code = HttpStatusCode.NotFound;

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;

            var rsp = new Response(false, HttpStatusCode.NotFound);
            return ReturnResult(context, rsp);
        }

        private Task HandleStreetCredBrokerException(HttpContext context, StreetCredBrokerException e)
        {
            CaptureException(e);

            var statusCode = HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;
            var message = Messages.Misc_ThirdParty;

            #if DEBUG
            message = e.Message;
            #endif
            var rsp = new Response(false, statusCode, message);
            return ReturnResult(context, rsp);
        }

        private Task HandleSendGridException(HttpContext context, SendGridException e)
        {
            CaptureException(e);

            var statusCode = HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;
            var message = Messages.Misc_ThirdParty;

            #if DEBUG
            message = e.Message;
            #endif
            var rsp = new Response(false, statusCode, message);
            return ReturnResult(context, rsp);
        }

        private Task HandleClickatellException(HttpContext context, ClickatellException e)
        {
            CaptureException(e);

            var statusCode = HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;
            var message = Messages.Misc_ThirdParty;

            #if DEBUG
            message = e.Message;
            #endif
            var rsp = new Response(false, statusCode, message);
            return ReturnResult(context, rsp);
        }
        private Task HandleAmazonS3Exception(HttpContext context, AmazonS3Exception e)
        {
            CaptureException(e);

            var statusCode = HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;
            var message = Messages.Misc_ThirdParty;

            #if DEBUG
            message = e.Message;
            #endif
            var rsp = new Response(false, statusCode, message);
            return ReturnResult(context, rsp);
        }

        private Task HandleBitlyException(HttpContext context, BitlyException e)
        {
            CaptureException(e);

            var statusCode = HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;
            var message = Messages.Misc_ThirdParty;

            #if DEBUG
            message = e.Message;
            #endif
            var rsp = new Response(false, statusCode, message);
            return ReturnResult(context, rsp);
        }

        private static Task HandleUnexpectedException(HttpContext context, Exception e)
        {
            CaptureException(e);

            var code = HttpStatusCode.InternalServerError;

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;

            var message = Messages.Misc_SomethingWentWrong;
            #if DEBUG
            message = e.Message;
            #endif

            var rsp = new Response(false, code, message);
            return ReturnResult(context, rsp);
        }

        private static Task HandleQRException(HttpContext context, Exception e)
        {
            CaptureException(e);

            var code = HttpStatusCode.InternalServerError;

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;

            var message = Messages.Misc_ThirdParty;

            var rsp = new Response(false, code, message);
            return ReturnResult(context, rsp);
        }

        private static Task HandleUnauthorisedException(HttpContext context, Exception e)
        {
            var code = HttpStatusCode.Unauthorized;

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;

            var message = Messages.Misc_Unauthorized;

            var rsp = new Response(false, code, message);
            return ReturnResult(context, rsp);
        }

        private static Task ReturnResult(HttpContext context, object response)
        {
            return context.Response.WriteAsync(JsonConvert.SerializeObject(response, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Formatting = Formatting.Indented
            }));
        }

        private static void CaptureException(Exception e)
        {
            #if !DEBUG
                SentrySdk.CaptureException(e);
            #endif
        }
        #endregion Exception Handler Methods
    }
}
