namespace Contracts.APICommunication;

public class BuyPremiumRequest
{
    public string CardNumber { get; set; }
    public string Cvv { get; set; }
    public string ExpDate { get; set; }
}