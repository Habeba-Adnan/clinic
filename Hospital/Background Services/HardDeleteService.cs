using Hospital.Repositories;

namespace Hospital.Background_Services
{
    public class HardDeleteService : BackgroundService
    {
        private readonly ILogger<HardDeleteService> _logger;
        private readonly IServiceProvider _serviceProvider; // Add IServiceProvider

        public HardDeleteService(ILogger<HardDeleteService> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider; // Initialize IServiceProvider
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Hard Delete Service is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceProvider.CreateScope()) // Create a scoped service
                    {
                        var patientRepo = scope.ServiceProvider.GetRequiredService<PatientRepo>();

                        // Get all patients soft-deleted more than 2 weeks ago
                        var patientsToDelete = patientRepo.GetAllDeleted()
                            .Where(p =>p.DeletionDate.HasValue &&
                            p.DeletionDate.Value.AddDays(1) <= DateTime.UtcNow).ToList();
                        //p.DeletionDate.Value.AddDays(1) <= DateTime.UtcNow)
                        // p.DeletionDate.Value.AddMinutes(3) <= DateTime.UtcNow


                        foreach (var patient in patientsToDelete)
                        {
                            patientRepo.HardDelete(patient.Id); // Hard delete
                        }

                        if (patientsToDelete.Any())
                        {
                            patientRepo.Save(); // Save changes
                            _logger.LogInformation($"Hard deleted {patientsToDelete.Count} patients.");
                        }
                        _logger.LogInformation($"Hard deleted {patientsToDelete.Count} patients.");

                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error during hard delete: {ex.Message}");
                    _logger.LogError($"Error during hard delete: {ex.Message}, StackTrace: {ex.StackTrace}");

                }

                // Run every 24 hours
                await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
                //await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }

}

