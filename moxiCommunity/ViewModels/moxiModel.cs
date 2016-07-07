using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace moxiCommunity.ViewModels
{
    public class loginModel
    {
        public bool IsSuccess { get; set; }

        public ReturnObjectsModel ReturnObjects { get; set; }
    }

    public class ReturnObjectsModel
    {

        public resultModel result { get; set; }

        public class resultModel
        {
            public string UserName { get; set; }
            public string UserEmail { get; set; }

            public int UserID { get; set; }
        }
    }
    
}