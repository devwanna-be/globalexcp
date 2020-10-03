using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GlobalExceptionSample.Middleware
{
    public class GlobalException
    {
        private readonly RequestDelegate _next;
        public GlobalException(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            //Capture JSON request
            string jsonRequest = await FormatRequest(context.Request);
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                //Logic disini, misal:
                //Log ke database atau file,
                //Send notif, etc.
                //Bukan best practice mengirimkan stack trace/exception message rebagai response.
                //Ini dilakukan hanya untuk demo saja.
                await HandleExceptionAsync(context, ex.Message);
            }
        }

        /// <summary>
        /// Capture JSON request dari HTTP context
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private async Task<string> FormatRequest(HttpRequest request)
        {
            HttpRequestRewindExtensions.EnableBuffering(request);
            var body = request.Body;
            byte[] buffer = new byte[Convert.ToInt32(request.ContentLength)];
            await request.Body.ReadAsync(buffer, 0, buffer.Length);
            string requestBody = Encoding.UTF8.GetString(buffer);
            requestBody = RemoveLFCR(requestBody);
            body.Seek(0, SeekOrigin.Begin);
            request.Body = body;
            return requestBody;
        }

        /// <summary>
        /// Cleansing text dari line feed dan carriage return
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private string RemoveLFCR(string text)
        {
            RegexOptions options = RegexOptions.None;
            string result = string.Empty;
            Regex regex = new Regex("[ ]{2,}", options);
            result = text.Replace(@"\\", string.Empty);
            result = text.Replace(System.Environment.NewLine, string.Empty);
            result = regex.Replace(result, " ");
            return result;
        }

        /// <summary>
        /// Return response
        /// </summary>
        /// <param name="context"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        private async Task HandleExceptionAsync(HttpContext context, string message)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            //Umumunya response berbentuk class object
            //Ini hanya untuk demo saja
            await context.Response.WriteAsync(message);
        }
    }
}
