using System;
using System.Collections.Generic;

namespace dbcontext
{
    public partial class User
    {
        public long UserId { get; set; }
        public int GameId { get; set; }
        public int ChannelId { get; set; }
        public string Sdkid { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
