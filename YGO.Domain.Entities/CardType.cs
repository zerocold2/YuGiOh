using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Security.Permissions;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace YGO.Domain.Entities
{
    public class CardType
    {
        public string TypeName { get; set; }
        public byte[] TypeIconBytes { get; set; }
    }
}
