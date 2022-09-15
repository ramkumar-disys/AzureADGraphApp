using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AADReaderApp
{
    class AaDGroupMember
    {
        public string ObjectId { get; set; }
        public string Name { get; set; }
        public string UserPrincipalName { get; set; }
        public string Email { get; set; } //UserPrincipalName  
    }
}
