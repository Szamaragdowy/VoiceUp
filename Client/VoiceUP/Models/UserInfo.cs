using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoiceUP.Models
{
    public class UserInfo
    {
        public string Name { get; set; }

        public UserInfo(string name)
        {
            Name = name;
        }
    }
}
