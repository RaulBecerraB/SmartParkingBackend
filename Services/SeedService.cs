using Microsoft.Extensions.Logging;
using SmartParkingBackend.Repository;
using System;
using System.Threading.Tasks;

namespace SmartParkingBackend.Services
{
    public class SeedService : ISeedService
    {
        private readonly ISeedRepository _repository;
        private readonly ILogger<SeedService> _logger;

        public SeedService(ISeedRepository repository, ILogger<SeedService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<bool> InitializeAsync()
        {
            _logger.LogInformation("Inicializando la base de datos con datos de prueba");

            try
            {
                // Verificar si ya hay datos
                if (await _repository.DatabaseHasDataAsync())
                {
                    _logger.LogWarning("La base de datos ya contiene estacionamientos. No se inicializarán datos de prueba.");
                    return false;
                }

                await _repository.InitializeTestDataAsync();
                
                _logger.LogInformation("Base de datos inicializada con éxito");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al inicializar la base de datos");
                return false;
            }
        }

        public async Task<bool> ResetAsync()
        {
            _logger.LogWarning("Reseteando todos los datos de la base de datos");

            try
            {
                await _repository.ResetDatabaseAsync();
                
                _logger.LogInformation("Datos eliminados con éxito");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al resetear la base de datos");
                return false;
            }
        }
    }
} 