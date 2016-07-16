using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace moxiCommunity.Models
{
    public class proNode
    {
        public int proClassID { get; set; }
        public string proClassName { get; set; }

        public int depth { get; set; }

        public List<proNode> ThirdJsons { get; set; }
    }

    
    
}