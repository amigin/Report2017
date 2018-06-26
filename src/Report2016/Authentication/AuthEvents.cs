using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Common.Log;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Hosting;

namespace Report2016.Authentication
{
 public class AuthEvent : OpenIdConnectEvents
    {
        private readonly ILog _log;

        public AuthEvent(ILog log)
        {
            _log = log;

        }

        public override Task RemoteFailure(RemoteFailureContext context)
        {
            _log.WriteErrorAsync("Authentication", "RemoteFailure", context.Failure.Message + context.Failure.InnerException, context.Failure).Wait();

            context.HandleResponse();
            context.Response.Redirect("/Home/AuthenticationFailed");

            return Task.FromResult(0);
        }

        public override async Task TokenValidated(TokenValidatedContext context)
        {
            var email = context.Principal.Claims.Where(c => c.Type == ClaimTypes.Email)
                   .Select(c => c.Value).SingleOrDefault();

            await base.TokenValidated(context);
        }

        public override Task TicketReceived(TicketReceivedContext context)
        {
            context.Properties.Items.Clear();
            context.Properties.Items.Clear();

            foreach (var principalClaim in context.Principal.Claims)
            {
                principalClaim.Properties.Clear();
            }

            return base.TicketReceived(context);
        }
    }
}
