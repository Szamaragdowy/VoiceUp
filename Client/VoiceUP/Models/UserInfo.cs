namespace VoiceUP.Models
{
    //Model to storing information about single User from server
    //It's simple but maybe later another properties will be added
    public class UserInfo
    {
        public string Name { get; set; }

        public bool isItMe { get; set; }

        public UserInfo(string name, bool isItme=false)
        {
            Name = name;
            isItMe = isItme;
        }
    }
}
