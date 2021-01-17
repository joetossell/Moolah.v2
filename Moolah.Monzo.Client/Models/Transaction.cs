using System;
using System.Collections.Generic;

namespace Moolah.Monzo.Client.Models
{
    public class Transaction
    {
        public string id { get; set; }
        public DateTimeOffset? created { get; set; }
        public string description { get; set; }
        public int amount { get; set; }
        public Fees fees { get; set; }
        public string currency { get; set; }
        public Merchant merchant { get; set; }
        public string notes { get; set; }
        public Dictionary<string,string> metadata { get; set; }
        public string[] labels { get; set; }
        public Attachment[] attachments { get; set; }
        public International international { get; set; }
        public string category { get; set; }
        public Dictionary<string,int> categories { get; set; }
        public bool? is_load { get; set; }
        public DateTimeOffset? settled { get; set; }
        public int? local_amount { get; set; }
        public string local_currency { get; set; }
        public DateTimeOffset? updated { get; set; }
        public string account_id { get; set; }
        public string user_id { get; set; }
        public Counterparty counterparty { get; set; }
        public string scheme { get; set; }
        public string dedupe_id { get; set; }
        public bool? originator { get; set; }
        public bool? include_in_spending { get; set; }
        public bool? can_be_excluded_from_breakdown { get; set; }
        public bool? can_be_made_subscription { get; set; }
        public bool? can_split_the_bill { get; set; }
        public bool? can_add_to_tab { get; set; }
        public bool? amount_is_pending { get; set; }
        public object atm_fees_detailed { get; set; }
        public string decline_reason { get; set; }
    }

    public class Fees
    {
    }

    public class Merchant
    {
        public string id { get; set; }
        public string group_id { get; set; }
        public DateTimeOffset? created { get; set; }
        public string name { get; set; }
        public string logo { get; set; }
        public string emoji { get; set; }
        public string category { get; set; }
        public bool? online { get; set; }
        public bool? atm { get; set; }
        public Address address { get; set; }
        public DateTimeOffset? updated { get; set; }
        public Dictionary<string,string> metadata { get; set; }
        public bool? disable_feedback { get; set; }
    }

    public class Address
    {
        public string short_formatted { get; set; }
        public string formatted { get; set; }
        public string address { get; set; }
        public string city { get; set; }
        public string region { get; set; }
        public string country { get; set; }
        public string postcode { get; set; }
        public double? latitude { get; set; }
        public double? longitude { get; set; }
        public int? zoom_level { get; set; }
        public bool? approximate { get; set; }
    }

    public class International
    {
        public string provider { get; set; }
        public string transfer_id { get; set; }
        public int? source_amount { get; set; }
        public string source_currency { get; set; }
        public int? target_amount { get; set; }
        public string target_currency { get; set; }
        public int? fee_amount { get; set; }
        public string fee_currency { get; set; }
        public double? rate { get; set; }
        public string status { get; set; }
        public string reference { get; set; }
        public DateTimeOffset? delivery_estimate { get; set; }
        public string transferwise_transfer_id { get; set; }
        public string transferwise_pay_in_reference { get; set; }
    }

    public class Counterparty
    {
        public string account_number { get; set; }
        public string name { get; set; }
        public string sort_code { get; set; }
        public string user_id { get; set; }
        public string account_id { get; set; }
        public string preferred_name { get; set; }
        public string beneficiary_account_type { get; set; }
        public string service_user_number { get; set; }
    }

    public class Attachment
    {
        public DateTimeOffset? created { get; set; }
        public string external_id { get; set; }
        public string file_type { get; set; }
        public string file_url { get; set; }
        public string id { get; set; }
        public string type { get; set; }
        public string url { get; set; }
        public string user_id { get; set; }
    }


}



