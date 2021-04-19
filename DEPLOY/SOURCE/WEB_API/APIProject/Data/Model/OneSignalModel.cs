using Data.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model
{
    public class OneSignalModel
    {

    }
    public class OneSignalInput
    {
        public string app_id { get; set; }
        public object data { get; set; }
        public object headings { get; set; }
        public object contents { get; set; }
        public string android_channel_id { get; set; }
        public List<string> include_player_ids { get; set; }
    }
    public class TextInput
    {
        public string en { get; set; }
    }

    public class DataOnesignal
    {
        public int timeSend
        {
            get
            {
                Int32 unixTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                return unixTimestamp;
            }
            set { }
        }
        public int timeWait
        {
            get
            {
                return (SystemParam.TIME_DELAY / 1000);
            }
            set { }
        }
        public int OrderID { get; set; }
    }


}
