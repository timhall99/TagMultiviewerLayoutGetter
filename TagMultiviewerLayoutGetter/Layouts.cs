using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TagMultiviewerLayoutGetter
{

    public class Multiviewer
    {
        public string Address { get; set; }
        public string Uid { get; set; }

        public Layout[] Layouts{ get; set; }


    }




    public class LayoutRootObject
    {
        public Layout Layout { get; set; }


    }

    public class Layout
    {
        public string uuid { get; set; }
        public string label { get; set; }
        public string modified { get; set; }
        public string created { get; set; }
        public string text_mode { get; set; }
        public string background { get; set; }
        public string layout_type_uuid { get; set; }
        public string layout_type_label { get; set; }
        public Tile[] Tiles { get; set; }
    }

    public class Tile
    {
        public int index { get; set; }
        public string channel_uuid { get; set; }
        public string channel_label { get; set; }
        public string content_type { get; set; }
        public string umd_1 { get; set; }
        public string umd_2 { get; set; }
        public string tally_settings { get; set; }
        public string activation_mode { get; set; }
        public string timezone_uuid { get; set; }
        public string timezone_label { get; set; }
        public string content { get; set; }
    }

}
