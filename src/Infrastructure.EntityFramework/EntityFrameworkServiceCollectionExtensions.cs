﻿using Bit.Core.Enums;
using Bit.Core.Repositories;
using Bit.Infrastructure.EntityFramework.Repositories;
using LinqToDB.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

public static class EntityFrameworkServiceCollectionExtensions
{
    public static void SetupEntityFramework(this IServiceCollection services, string connectionString, SupportedDatabaseProviders provider)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new Exception($"Database provider type {provider} was selected but no connection string was found.");
        }

        LinqToDBForEFTools.Initialize();
        services.AddAutoMapper(typeof(UserRepository));
        services.AddDbContext<DatabaseContext>(options =>
        {
            if (provider == SupportedDatabaseProviders.Postgres)
            {
                options.UseNpgsql(connectionString);
                // Handle NpgSql Legacy Support for `timestamp without timezone` issue
                AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            }
            else if (provider == SupportedDatabaseProviders.MySql)
            {
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
            }
            else if (provider == SupportedDatabaseProviders.SqlServer)
            {
                options.UseSqlServer(connectionString);
            }
        });
    }

    public static void AddPasswordManagerEFRepositories(this IServiceCollection services, bool selfHosted)
    {
        // TODO: We should move away from using LINQ syntax for EF (TDL-48).
        LinqToDBForEFTools.Initialize();

        services.AddSingleton<IApiKeyRepository, ApiKeyRepository>();
        services.AddSingleton<IAuthRequestRepository, AuthRequestRepository>();
        services.AddSingleton<ICipherRepository, CipherRepository>();
        services.AddSingleton<ICollectionCipherRepository, CollectionCipherRepository>();
        services.AddSingleton<ICollectionRepository, CollectionRepository>();
        services.AddSingleton<IDeviceRepository, DeviceRepository>();
        services.AddSingleton<IEmergencyAccessRepository, EmergencyAccessRepository>();
        services.AddSingleton<IFolderRepository, FolderRepository>();
        services.AddSingleton<IGrantRepository, GrantRepository>();
        services.AddSingleton<IGroupRepository, GroupRepository>();
        services.AddSingleton<IInstallationRepository, InstallationRepository>();
        services.AddSingleton<IMaintenanceRepository, MaintenanceRepository>();
        services.AddSingleton<IOrganizationApiKeyRepository, OrganizationApiKeyRepository>();
        services.AddSingleton<IOrganizationConnectionRepository, OrganizationConnectionRepository>();
        services.AddSingleton<IOrganizationRepository, OrganizationRepository>();
        services.AddSingleton<IOrganizationSponsorshipRepository, OrganizationSponsorshipRepository>();
        services.AddSingleton<IOrganizationUserRepository, OrganizationUserRepository>();
        services.AddSingleton<IPolicyRepository, PolicyRepository>();
        services.AddSingleton<IProviderOrganizationRepository, ProviderOrganizationRepository>();
        services.AddSingleton<IProviderRepository, ProviderRepository>();
        services.AddSingleton<IProviderUserRepository, ProviderUserRepository>();
        services.AddSingleton<ISendRepository, SendRepository>();
        services.AddSingleton<ISsoConfigRepository, SsoConfigRepository>();
        services.AddSingleton<ISsoUserRepository, SsoUserRepository>();
        services.AddSingleton<ITaxRateRepository, TaxRateRepository>();
        services.AddSingleton<ITransactionRepository, TransactionRepository>();
        services.AddSingleton<IUserRepository, UserRepository>();

        if (selfHosted)
        {
            services.AddSingleton<IEventRepository, EventRepository>();
        }
    }
}