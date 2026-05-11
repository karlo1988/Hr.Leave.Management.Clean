using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HR.LeaveManagement.BlazorUI.Services.Base
{
    public class BaseHttpService
    {
        protected IClient _client;
        public BaseHttpService(IClient client)
        {
            _client = client;
        }

        protected Response<Guid> ConvertApiExceptions<Guid>(ApiException exception)
        {          
            if (exception.StatusCode == 400)
            {
               return new Response<Guid>
                {
                    Success = false,
                    Message ="Invalid data was submitted",
                    ValidationErrors = exception.Response
                };
            }
            else if (exception.StatusCode == 404)
            {
                return new Response<Guid>
                {
                    Success = false,
                    Message ="The record was not found"
                };
            }
            else
            {
                return new Response<Guid>
                {
                    Success = false,
                    Message ="Something went wrong, please try again later"
                };
            }
           

        }
    }
}