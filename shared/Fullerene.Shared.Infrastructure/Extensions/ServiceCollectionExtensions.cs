using Fullerene.Shared.Common.Abstractions.Storage;
using Fullerene.Shared.Common.Extensions;
using Fullerene.Shared.Infrastructure.Abstractions;
using Fullerene.Shared.Infrastructure.Services;
using Fullerene.Shared.Infrastructure.Settings;
using Fullerene.Shared.Infrastructure.Util;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Clients;
using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Common.Authentication;
using Genbox.SimpleS3.Extensions.GenericS3;
using Genbox.SimpleS3.GenericS3;
using Genbox.SimpleS3.GenericS3.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Fullerene.Shared.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFullereneStorage(
        this IServiceCollection services, IConfiguration configuration)
    {
        var s3StorageSettings = services.ConfigureAndGetSettings<S3StorageSettings>(configuration);
        services.ConfigureSettings<UnsignedApkS3BucketSettings>(configuration);
        services.ConfigureSettings<SignedApkS3BucketSettings>(configuration);

        services.AddKeyedSingleton<ISimpleClient>(S3ClientRole.General, (_, _) =>
        {
            var config = new GenericS3Config
            {
                Endpoint = s3StorageSettings.InternalUrl,

                Credentials = new StringAccessKey(
                    s3StorageSettings.AccessKey,
                    s3StorageSettings.SecretKey),

                RegionCode = s3StorageSettings.Region,

                NamingMode = NamingMode.PathStyle
            };

            return new GenericS3Client(config);
        });

        if (s3StorageSettings.PublicUrl is not null)
        {
            services.AddKeyedSingleton<ISignedClient>(S3ClientRole.PublicUrlPresigner,
                (_, _) =>
                {
                    var config = new GenericS3Config
                    {
                        Endpoint = s3StorageSettings.PublicUrl,

                        Credentials = new StringAccessKey(
                            s3StorageSettings.AccessKey,
                            s3StorageSettings.SecretKey),

                        RegionCode = s3StorageSettings.Region,

                        NamingMode = NamingMode.PathStyle
                    };

                    return new GenericS3Client(config);
                });

            services.AddSingleton<IS3PublicPresignedUrlProvider>(sp =>
            {
                var presignerClient = sp.GetRequiredKeyedService<ISignedClient>(S3ClientRole.PublicUrlPresigner);
                return new GaragePublicPresignedUrlS3Provider(presignerClient);
            });
            services.AddSingleton<ISignedApkPublicPresignedUrlProvider, SignedApkPublicPresignedUrlProvider>();
        }

        services.AddSingleton<IGeneralS3Storage>(sp =>
        {
            var generalClient = sp.GetRequiredKeyedService<ISimpleClient>(S3ClientRole.General);
            return new GarageGeneralS3Storage(generalClient);
        });
        services.AddSingleton<ISignedApkStorage, SignedApkStorage>();
        services.AddSingleton<IUnsignedApkStorage, UnsignedApkStorage>();

        return services;
    }
}