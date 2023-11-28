using System.Data;
using System.Data.SqlClient;

namespace OutboxCardService.Repository;

public class CardRepository
{
	private string connectionString = "";

	private readonly ILogger<CardRepository> _logger;
    private readonly IConfiguration _config;

    public CardRepository(ILogger<CardRepository> logger, IConfiguration config)
    {
	    _logger = logger;

         _config = config;

		connectionString = _config.GetConnectionString("crmSqlConnection");
}
    
    public DataTable GetChangedCards()
    {
		return GetCardDataTable(false);
    }
    
    public DataTable GetAllCards()
    {
	    return GetCardDataTable(true);
    }    
   
    private DataTable GetCardDataTable(bool exportAllCards)
    {
		DataTable dataTable = new DataTable();

	    using (SqlConnection conn = new SqlConnection(connectionString))
	    {
		    try
		    {
			    conn.Open();

			    using (SqlCommand cmd = new SqlCommand("spXXXXX", conn))
				{
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddWithValue("@ExportAll", exportAllCards);
				    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
				    {
					    adapter.Fill(dataTable);
					    _logger.LogInformation($"Card for transfer {dataTable.Rows.Count}");
				    }
			    }
		    }
		    catch (Exception ex)
		    {
			    _logger.LogError(ex, "Error in mark card");
		    }
	    }

	    return dataTable;
    }
}