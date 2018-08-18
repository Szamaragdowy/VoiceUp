namespace VoiceUpServer.Models
{
    class NetworkAdapter
    {
        private string networkAdapterName;
        private string ip;
        #region properties
        public string Name
        {
            get { return networkAdapterName; }
            set { networkAdapterName = value; }
        }
        public string IPAdress
        {
            get { return ip; }
            set { ip = value; }
        }
        #endregion
        public NetworkAdapter(string name, string ip)
        {
            this.networkAdapterName = name;
            this.ip = ip;
        }
        public NetworkAdapter()
        {

        }
    }
}
