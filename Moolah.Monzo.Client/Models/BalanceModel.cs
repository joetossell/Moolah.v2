namespace Moolah.Monzo.Client.Models
{
    public class BalanceModel
    {
        public int balance { get; set; }
        public int total_balance { get; set; }
        public int balance_including_flexible_savings { get; set; }
        public string currency { get; set; }
        public int spend_today { get; set; }
        public string local_currency { get; set; }
        public int local_exchange_rate { get; set; }
        public Local_Spend[] local_spend { get; set; }
    }

    public class Local_Spend
    {
        public int spend_today { get; set; }
        public string currency { get; set; }
    }

}
