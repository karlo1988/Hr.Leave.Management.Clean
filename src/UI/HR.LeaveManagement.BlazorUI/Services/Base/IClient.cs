using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HR.LeaveManagement.BlazorUI.Services.Base
{
    public partial interface IClient
    {
        public HttpClient HttpClient { get; }
    }
}