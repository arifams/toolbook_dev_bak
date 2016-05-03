using LightInject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace PI.Service
{
    public class HttpRequestMessageHandler: DelegatingHandler
    {
        private LogicalThreadStorage<HttpRequestMessageStorage> messageStorage =
        new LogicalThreadStorage<HttpRequestMessageStorage>(() => new HttpRequestMessageStorage());
        
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            messageStorage.Value.Message = request;
            return base.SendAsync(request, cancellationToken);
        }

        public HttpRequestMessage GetCurrentMessage()
        {
            return messageStorage.Value.Message;
        }
    }
}