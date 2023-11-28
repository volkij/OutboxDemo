using System.Data;
using OutboxCardService.Convertors;
using OutboxCardService.Repository;
using OutboxDemo.Messages;
using OutboxDemo.Messages.Data;
using OutboxDemo.Outbox;
using OutboxDemo.Outbox.Configuration;
using OutboxDemo.Outbox.Publishers;

namespace OutboxCardService.Services;

public class CardPublisher
{
    private readonly RabbitConfiguration _rabbitConfiguration;
    private readonly ILogger _logger;
    private RabbitPublisher<OutboxCard> publisher;
    private readonly CardRepository _cardRepository;
    private readonly IConfiguration _config;
    private string _configSectionName;
    
    public CardPublisher(CardRepository repository, ILogger logger, IConfiguration config, string configSectionName)
    {
        _cardRepository = repository;
        _logger = logger;
        _config = config;
        _configSectionName = configSectionName; 

        publisher = CreatePublisher();
        publisher.MessageSent += Publisher_MessageSent;
    }
    
    private RabbitPublisher<OutboxCard> CreatePublisher()
    {
       
        var rabbitConfigFactory = OutboxFactory.Create(builder =>
        {
            builder
                .ConfigureFromSection(_config.GetSection(_configSectionName));

        }, _logger);
        return rabbitConfigFactory.CreatePublisher<OutboxCard>();
    }
    
    private void Publisher_MessageSent(OutboxCard message)
    {
        Console.WriteLine($"EVENT Message sent: {message.ID}");
    }
    
    public void Run()
    {
        var cardTable = _cardRepository.GetChangedCards();
        foreach (DataRow row in cardTable.Rows)
        {
            int cardID = System.Convert.ToInt32(row["CardID"]);
            OutboxCardData cardData = CardDataConvertor.Convert(row);
            OutboxCard cardMessage = new OutboxCard(cardData);
            publisher.Publish(cardMessage);
        }
    }
}