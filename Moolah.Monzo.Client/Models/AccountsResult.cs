using System;

namespace Moolah.Monzo.Client.Models
{
    public class AccountsResult
    {
        public Account[] accounts { get; set; }
    }

    public class Account
    {
        public string id { get; set; }
        public bool closed { get; set; }
        public DateTimeOffset created { get; set; }
        public string description { get; set; }
        public string type { get; set; }
        public string currency { get; set; }
        public string country_code { get; set; }
        public Owner[] owners { get; set; }
        public string account_number { get; set; }
        public string sort_code { get; set; }
        public Payment_Details payment_details { get; set; }
    }

    public class Payment_Details
    {
        public Locale_Uk locale_uk { get; set; }
    }

    public class Locale_Uk
    {
        public string account_number { get; set; }
        public string sort_code { get; set; }
    }

    public class Owner
    {
        public string user_id { get; set; }
        public string preferred_name { get; set; }
        public string preferred_first_name { get; set; }
    }

}