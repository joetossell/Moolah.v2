namespace Moolah.Monzo.Client.Models
{
    public class WebhookEvent<T>
    {
        public string type { get; set; }
        public T data { get; set; }
    }
}