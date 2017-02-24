using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Core.Entities;
using Microsoft.AspNetCore.SignalR;

namespace CloudStorage.Hubs
{
    public class FileOperationsHub : Hub
    {
        private static List<CompanyUser> _companyUsers = new List<CompanyUser>();

        public override Task OnConnected()
        {
            var user = Context.User as ClaimsPrincipal;
            if (Context.User.Identity.IsAuthenticated &&
                user != null && user.HasClaim(_ => _.Type == "Company"))
            {
                var companyId = user.Claims.FirstOrDefault(_ => _.Type == "Company")?.Value;
                var existingEntry = _companyUsers.FirstOrDefault(_ => _.Username.Equals(user.Identity.Name, 
                    StringComparison.OrdinalIgnoreCase));
                if (existingEntry != null)
                {
                    existingEntry.UserConnectionId = Context.ConnectionId;
                    existingEntry.CompanyId = companyId;
                }
                else
                {
                    
                }
            }

            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            return base.OnDisconnected(stopCalled);
        }

        public override Task OnReconnected()
        {
            return base.OnReconnected();
        }

        public void PingHello(string param)
        {
            Clients.All.pongHello("You wrote: " + param);
        }
    }
}