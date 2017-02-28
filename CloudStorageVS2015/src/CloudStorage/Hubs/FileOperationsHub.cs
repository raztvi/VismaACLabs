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
        private static readonly List<CompanyUser> CompanyUsers = new List<CompanyUser>();

        #region Hub event overrides

        public override Task OnConnected()
        {
            AddOrUpdateUser();
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            var entry = CompanyUsers.FirstOrDefault(_ => _.UserConnectionId.Equals(Context.ConnectionId));
            if (entry != null)
            {
                CompanyUsers.Remove(entry);
            }
            return base.OnDisconnected(stopCalled);
        }

        public override Task OnReconnected()
        {
            AddOrUpdateUser();
            return base.OnReconnected();
        }

        #endregion

        public void PingHello(string param)
        {
            Clients.All.pongHello("You wrote: " + param);
        }


        private void AddOrUpdateUser()
        {
            var user = Context.User as ClaimsPrincipal;
            if (Context.User.Identity.IsAuthenticated &&
                user != null && user.HasClaim(_ => _.Type == "Company"))
            {
                var companyId = user.Claims.FirstOrDefault(_ => _.Type == "Company")?.Value;
                var existingEntry = CompanyUsers.FirstOrDefault(_ => _.Username.Equals(user.Identity.Name,
                    StringComparison.OrdinalIgnoreCase));
                if (existingEntry != null)
                {
                    existingEntry.UserConnectionId = Context.ConnectionId;
                    existingEntry.CompanyId = companyId;
                }
                else
                {
                    CompanyUsers.Add(new CompanyUser
                    {
                        CompanyId = companyId,
                        Username = user.Identity.Name,
                        UserConnectionId = Context.ConnectionId
                    });
                }
            }
        }
    }
}