using DeliveryApp.Infrastructure;
public class Settings
{
    public string CONNECTION_STRING { get; set; }
    public string GEO_SERVICE_GRPC_HOST { get; set; }
    public string MESSAGE_BROKER_HOST { get; set; }
    public string BASKET_CONFIRMED_TOPIC { get; set; }
    public string ORDER_STATUS_CHANGED { get; set; }
}