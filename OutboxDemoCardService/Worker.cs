using OutboxCardService.Repository;
using OutboxCardService.Services;

namespace OutboxCardService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private CardRepository _cardRepository;
        private IConfiguration _config;

        private CardPublisher _publisherCardChanges;
        //private CardPublisherSync _publisherCardSync;

        private DateTime _nextSyncDate;

        public Worker(ILogger<Worker> logger, CardRepository cardRepository, IConfiguration config)
        {
            _logger = logger;
            _cardRepository = cardRepository;
            _config = config;


            _nextSyncDate = new DateTime(DateTime.Now.AddDays(1).Year, DateTime.Now.AddDays(1).Month, DateTime.Now.AddDays(1).Day, 2, 0, 0);

            _logger.LogInformation($"Next sync date is {_nextSyncDate.ToString()}");

            _publisherCardChanges = new CardPublisher(_cardRepository, _logger, _config, "OutBox.Card");
            //_publisherCardSync = new CardPublisherSync(_cardRepository, _logger, _config, "OutBox.Card");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                if (DateTime.Now > _nextSyncDate)
                {
                    _logger.LogInformation("Running CardDaySync");
                    _nextSyncDate = new DateTime(DateTime.Now.AddDays(1).Year, DateTime.Now.AddDays(1).Month, DateTime.Now.AddDays(1).Day, 2, 0, 0);

                    //_publisherCardSync.Run();
                }
                else
                {
                    _logger.LogInformation("Running CardChanges");
                    _publisherCardChanges.Run();

                }                

                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }
    }
}