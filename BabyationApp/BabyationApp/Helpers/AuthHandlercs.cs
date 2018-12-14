using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Diagnostics;
using BabyationApp.Helpers;
using System.Net.Http.Headers;
using BabyationApp.Managers;


namespace BabyationApp.Helpers
{
    class AuthHandler : DelegatingHandler
    {
        /// <summary>
        /// Override SendAsync to send ZUMO header required for Table Controller and Add Bearer Auth Token
        /// </summary>
        /// <param name="request">HttpRequestMessage origional request</param>
        /// <param name="cancellationToken">CancellationToken cancellation token</param>
        /// <returns>System.Threading.Tasks.Task</returns>
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            string body = string.Empty;
            HttpRequestMessage cleanRequest = new HttpRequestMessage(request.Method, request.RequestUri);
            if (request.Content != null)
            {
                body = await request.Content.ReadAsStringAsync();
            }
            cleanRequest.Content = request.Content;
            cleanRequest.Headers.Add("ZUMO-API-VERSION", "2.0.0");
            cleanRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", LoginManager.Instance.Token);
            var response = await base.SendAsync(cleanRequest, cancellationToken);
            return response;
        }
    }

}