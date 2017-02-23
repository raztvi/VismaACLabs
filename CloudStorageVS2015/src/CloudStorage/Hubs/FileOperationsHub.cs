using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Entities;
using Microsoft.AspNetCore.SignalR;

namespace CloudStorage.Hubs
{
    public class FileOperationsHub : Hub
    {
        private static List<CompanyUser> _companyUsers = new List<CompanyUser>();

        public void PingHello(string param)
        {
            Clients.All.pongHello("You wrote: " + param);
        }

        public void ConnectToCompanyGroup(string username, string companyId)
        {
            
        }
    }
}