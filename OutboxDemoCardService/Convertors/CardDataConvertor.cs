using System.Data;
using OutboxDemo.Messages.Data;

namespace OutboxCardService.Convertors;

public static class CardDataConvertor
{
    public static OutboxCardData Convert(DataRow row)
    {
        OutboxCardData cardData = new OutboxCardData()
        {
            CardID = System.Convert.ToInt32(row["ExtID"]),
            DefaultPoolCode = row["DefaultPoolCode"].ToString(),
            MPTSID = row["MptsID"].ToString(),
            DateCreate = System.Convert.ToDateTime(row["DateCreate"]),
            IsActive = System.Convert.ToBoolean(row["IsActive"]),
            DefaultEspFpPoolCode = row["DefaultEspFpPoolCode"].ToString(),
            HostCode = row["CodeHost"].ToString(),
            ClientID = row["CustomerID"].ToString(),
            ExternalID = row["ExternalID"].ToString(),
            CardRangeCode = row["ExternalCardRange"].ToString(),
            ModDate = System.Convert.ToDateTime(row["ModDate"])
        };

        return cardData;
    }
    
}