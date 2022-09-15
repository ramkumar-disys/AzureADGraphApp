using Microsoft.Graph;
using Microsoft.Graph.Auth;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AADReaderApp
{
    class Program
    {
        static void Main(string[] args)
        {
            List<AaDGroupMember> groupList = GetGroupMembers("BaraniTeams");
        }
        public static List<AaDGroupMember> GetGroupMembers(string groupName)
        {
            var userList = new List<AaDGroupMember>();
            try
            {
                var clientId = "576e6c18-fb62-4a89-9cd3-1f83f18ff294";
                var tenantId = "08fdfb44-3c18-4aa7-9519-72bddc98ad3a";
                var secret = "qzH8Q~26Su4TryGsUZlCW2nbYeQqd0VqeF7OqdBR";
                IConfidentialClientApplication confidentialClientApplication = ConfidentialClientApplicationBuilder
                                                                                   .Create(clientId)
                                                                                   .WithTenantId(tenantId)
                                                                                   .WithClientSecret(secret)
                                                                                   .Build();

                IAuthenticationProvider authProvider = new ClientCredentialProvider(confidentialClientApplication);
                GraphServiceClient graphClient = new GraphServiceClient(authProvider);

                var groupsDetails = graphClient.Groups.Request()
                    .Filter($"startswith(displayName,'{groupName}')")  // TODO: optimize this filter criteria based on your need
                    .GetAsync()
                     .ConfigureAwait(false)
                       .GetAwaiter()
                       .GetResult()
                       .ToList()
                       .Where(x => string.Equals(x.DisplayName, groupName, StringComparison.InvariantCultureIgnoreCase))
                       .FirstOrDefault();


                var groupObjectId = groupsDetails.Id;
                var groupMembers = graphClient.Groups[groupObjectId]
                       .TransitiveMembers  // To get the recursive members (if there is another member group, it will also get that groups member list)
                                           //.Members  // just to get the direct member  
                       .Request()
                       .GetAsync()
                       .ConfigureAwait(false)
                       .GetAwaiter()
                       .GetResult();


                foreach (var mem in groupMembers.ToList())
                {
                    //var memType = mem.GetType();  
                    if (mem.GetType() == typeof(User))
                    {
                        var myUser = graphClient.Users[mem.Id].Request().GetAsync()
                       .ConfigureAwait(false)
                       .GetAwaiter()
                       .GetResult();

                        User forUser = (User)mem;

                        userList.Add(new AaDGroupMember
                        {
                            ObjectId = forUser.Id,
                            UserPrincipalName = forUser.UserPrincipalName,
                            Name = forUser.DisplayName,
                            Email = forUser.Mail,

                        });
                    }
                }

                return userList;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
