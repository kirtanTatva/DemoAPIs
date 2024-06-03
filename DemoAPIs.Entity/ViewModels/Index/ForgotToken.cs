using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoAPIs.Entity.ViewModels.Index
{
    public class ForgotToken
    {
        public string Token { get; set; }
        public DateTime ForgotTokenExpires{ get; set; }
    }
}
