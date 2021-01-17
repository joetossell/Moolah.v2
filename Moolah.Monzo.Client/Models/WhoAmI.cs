namespace Moolah.Monzo.Client.Models
{ 
    public class WhoAmI
    {
        public bool authenticated { get; set; }
        public string client_id { get; set; }
        public string user_id { get; set; }
    }

}
