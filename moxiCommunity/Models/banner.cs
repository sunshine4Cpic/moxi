using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace moxiCommunity.Models {
    public class banner {
        public string name { get; set; }

        public List<bannerNode> nodes { get; set; }
    }
    public class bannerNode {
        public string url { get; set; }
        public string img { get; set; }
        public string txt { get; set; }
        public float price { get; set; }
    }
}