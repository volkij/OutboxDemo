using System.Text.Json;

namespace OutboxDemo.Messages.Data;

public class OutboxCardData
{
    public int CardID { get; set; }
    
    public string DefaultPoolCode { get; set; }
    
    public string MPTSID { get; set; }
    
    public DateTime DateCreate { get; set; }
    
    public bool IsActive { get; set; }
   
    public string DefaultEspFpPoolCode { get; set; }

    public string HostCode { get; set; }
    
    public string ClientID { get; set; }    
    
    public string ExternalID { get; set; }
    
    public string CardRangeCode { get; set; }

    public DateTime ModDate { get; set; }

}