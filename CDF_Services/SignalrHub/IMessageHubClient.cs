using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDF_Services.SignalrHub
{
    public interface IMessageHubClient
    {

        Task RequestCurrentLocationToUser(int TaskId, int UserId);
    }

}
