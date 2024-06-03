using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoAPIs.Entity.ViewModels.Index
{
    public class RefreshToken
    {
        public string refreshToken { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}
