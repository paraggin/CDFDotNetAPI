using Microsoft.AspNetCore.SignalR;

namespace CDF_Services.SignalrHub
{
    public class MessageHub : Hub<IMessageHubClient>
    {

        public async Task RequestCurrentLocationToUser(int TaskId, int UserId)
        {
            await Clients.Client(Convert.ToString(UserId)).RequestCurrentLocationToUser(TaskId, UserId);
        }
    }
}
