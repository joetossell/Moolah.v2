using System;

namespace Moolah.Monzo.Client.Models
{
    public class PotsResult
    {
        public Pot[] pots { get; set; }
    }

    public class Pot
    {
        public string id { get; set; }
        public string name { get; set; }
        public string style { get; set; }
        public int balance { get; set; }
        public string currency { get; set; }
        public string type { get; set; }
        public string product_id { get; set; }
        public string current_account_id { get; set; }
        public string cover_image_url { get; set; }
        public string isa_wrapper { get; set; }
        public bool round_up { get; set; }
        public int? round_up_multiplier { get; set; }
        public bool is_tax_pot { get; set; }
        public DateTimeOffset created { get; set; }
        public DateTimeOffset updated { get; set; }
        public bool deleted { get; set; }
        public bool locked { get; set; }
        public string charity_id { get; set; }
        public bool available_for_bills { get; set; }
        public int goal_amount { get; set; }
    }
}
