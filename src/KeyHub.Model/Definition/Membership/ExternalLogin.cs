using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyHub.Model.Definition.Membership
{
    public class ExternalLogin
    {
        public int UserId { get; set; }

        public virtual User User { get; set; }

        public string LoginProvider { get; set; }

        public string ProviderKey { get; set; }
    }
}
