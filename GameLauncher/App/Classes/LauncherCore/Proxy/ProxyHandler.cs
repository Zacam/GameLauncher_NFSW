using GameLauncher;
using GameLauncher.App.Classes.LauncherCore.Global;
using Flurl;
using Flurl.Http;
using Flurl.Http.Content;
using GameLauncher.App.Classes.LauncherCore.FileReadWrite;
using GameLauncher.App.Classes.LauncherCore.RPC;
using GameLauncher.App.Classes.SystemPlatform.Linux;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.Extensions;
using Nancy.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FurlURL = Flurl.Url;

namespace GameLauncher.App.Classes.LauncherCore.Proxy
{
    public class ProxyHandler : IApplicationStartup
    {
        public void Initialize(IPipelines pipelines)
        {
            pipelines.BeforeRequest += ProxyRequest;
            pipelines.OnError += OnError;
        }

        private async Task<TextResponse> OnError(NancyContext context, Exception exception)
        {
            Log.Error($"PROXY ERROR [handling {context.Request.Path}]");
            Log.Error($"\tMESSAGE: {exception.Message}");
            Log.Error($"\t{exception.StackTrace}");
            CommunicationLog.RecordEntry(ScreenLogin.SelectedServerName, "PROXY",
                CommunicationLogEntryType.Error,
                new CommunicationLogLauncherError(exception.Message, context.Request.Path,
                    context.Request.Method));
            await SubmitError(exception);

            return new TextResponse(HttpStatusCode.BadRequest, exception.Message);
        }

        private async Task<Response> ProxyRequest(NancyContext context, CancellationToken cancellationToken)
        {
            string path = context.Request.Path;
            string method = context.Request.Method.ToUpperInvariant();

            if (!path.StartsWith("/nfsw/Engine.svc"))
            {
                throw new ProxyException("Invalid request path: " + path);
            }

            path = path.Substring("/nfsw/Engine.svc".Length);

            FurlURL resolvedUrl = new FurlURL(ScreenLogin.SelectedServerIP).AppendPathSegment(path);

            foreach (var queryParamName in context.Request.Query)
            {
                resolvedUrl = resolvedUrl.SetQueryParam(queryParamName, context.Request.Query[queryParamName],
                    NullValueHandling.Ignore);
            }

            IFlurlRequest request = resolvedUrl.AllowAnyHttpStatus().WithTimeout(TimeSpan.FromSeconds(30));

            foreach (var header in context.Request.Headers)
            {
                // Don't send Content-Length for GET requests
                if (method == "GET" && header.Key.ToLowerInvariant() == "content-length")
                {
                    continue;
                }

                request = request.WithHeader(header.Key,
                    header.Key == "Host" ? resolvedUrl.ToUri().Host : header.Value.First());
            }

            var requestBody = method != "GET" ? context.Request.Body.AsString(Encoding.UTF8) : "";

            CommunicationLog.RecordEntry(ScreenLogin.SelectedServerName, "SERVER",
                CommunicationLogEntryType.Request,
                new CommunicationLogRequest(requestBody, resolvedUrl.ToString(), method));

            IFlurlResponse responseMessage;

            var POSTContent = String.Empty;

            var queryParams = new Dictionary<string, object>();

            foreach (var param in context.Request.Query)
            {
                var value = context.Request.Query[param];
                queryParams[param] = value;
            }

            var GETContent = string.Join(";", queryParams.Select(x => x.Key + "=" + x.Value).ToArray());

            switch (method)
            {
                case "GET":
                    responseMessage = await request.GetAsync(cancellationToken);
                    break;
                case "POST":
                    responseMessage = await request.PostAsync(new CapturedStringContent(requestBody),
                        cancellationToken);
                    POSTContent = context.Request.Body.AsString();
                    break;
                case "PUT":
                    responseMessage = await request.PutAsync(new CapturedStringContent(requestBody),
                        cancellationToken);
                    break;
                case "DELETE":
                    responseMessage = await request.DeleteAsync(cancellationToken);
                    break;
                default:
                    throw new ProxyException("Cannot handle request method: " + method);
            }

            var responseBody = await responseMessage.GetStringAsync();

            if (path == "/User/GetPermanentSession")
            {
                responseBody = CleanFromUnknownChars(responseBody);
            }

            int statusCode = responseMessage.StatusCode;

            try
            {
                DiscordGamePresence.HandleGameState(path, responseBody, GETContent);
            }
            catch (Exception e)
            {
                Log.Error($"DISCORD RPC ERROR [handling {context.Request.Path}]");
                Log.Error($"\tMESSAGE: {e.Message}");
                Log.Error($"\t{e.StackTrace}");
                await SubmitError(e);
            }

            queryParams.Clear();

            CommunicationLog.RecordEntry(ScreenLogin.SelectedServerName, "SERVER",
                CommunicationLogEntryType.Response, new CommunicationLogResponse(
                    responseBody, resolvedUrl.ToString(), method));

            return new TextResponse(responseBody,
                responseMessage.ResponseMessage.Content.Headers.ContentType?.MediaType ?? "application/xml;charset=UTF-8")
            {
                StatusCode = (HttpStatusCode)statusCode
            }; ;
        }

        private static string CleanFromUnknownChars(string s)
        {
            StringBuilder sb = new StringBuilder(s.Length);
            foreach (char c in s)
            {
                if
                 (
                  (int)c >= 48 && (int)c <= 57 ||
                  (int)c == 60 || (int)c == 62 ||
                  (int)c >= 65 && (int)c <= 90 ||
                  (int)c >= 97 && (int)c <= 122 ||
                  (int)c == 47 || (int)c == 45 ||
                  (int)c == 46
                 )
                {
                    sb.Append(c);
                }
            }

            return sb.ToString();
        }

        private static async Task SubmitError(Exception exception)
        {
            var mainsrv = DetectLinux.LinuxDetected() ? URLs.mainserver.Replace("https", "http") : URLs.mainserver;
            FurlURL url = new FurlURL(mainsrv + "/error-report");
            await url.PostJsonAsync(new
            {
                message = exception.Message ?? "no message",
                stackTrace = exception.StackTrace ?? "no stack trace"
            });
        }
    }

    public class Helper
    {
        public static int session = 0;
        public static String personaid = String.Empty;
    }
}
