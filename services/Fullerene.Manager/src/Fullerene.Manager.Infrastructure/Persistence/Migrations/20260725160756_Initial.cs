using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fullerene.Manager.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:pg_trgm", ",,");

            migrationBuilder.CreateTable(
                name: "ArtifactDataRecords",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ArtifactType = table.Column<int>(type: "integer", nullable: false),
                    VersionCode = table.Column<int>(type: "integer", nullable: false),
                    MinApiLevel = table.Column<int>(type: "integer", nullable: false),
                    TargetApiLevel = table.Column<int>(type: "integer", nullable: false),
                    SplitId = table.Column<string>(type: "text", nullable: true),
                    ModuleName = table.Column<string>(type: "text", nullable: true),
                    CpuArchitecture = table.Column<int>(type: "integer", nullable: true),
                    DeliveryType = table.Column<int>(type: "integer", nullable: true),
                    AssetModuleType = table.Column<int>(type: "integer", nullable: true),
                    TextureCompressionFormat = table.Column<int>(type: "integer", nullable: true),
                    LanguageTargeting = table.Column<string>(type: "text", nullable: true),
                    Density_Alias = table.Column<int>(type: "integer", nullable: true),
                    Density_Dpi = table.Column<int>(type: "integer", nullable: true),
                    LanguageArtifactSplitDataRecord_LanguageTargeting = table.Column<string>(type: "text", nullable: true),
                    StandaloneSingleAbiArtifactDataRecord_CpuArchitecture = table.Column<int>(type: "integer", nullable: true),
                    CpuArchitectures = table.Column<int[]>(type: "integer[]", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArtifactDataRecords", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NixPackageRepos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    GitRepositoryUrl = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NixPackageRepos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AndroidAppPackages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    NixPackageRepoId = table.Column<Guid>(type: "uuid", nullable: false),
                    NixPackageName = table.Column<string>(type: "text", nullable: false),
                    AndroidApplicationId = table.Column<string>(type: "text", nullable: false),
                    IsTracked = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AndroidAppPackages", x => x.Id);
                    table.UniqueConstraint("AK_AndroidAppPackages_NixPackageRepoId_NixPackageName_AndroidA~", x => new { x.NixPackageRepoId, x.NixPackageName, x.AndroidApplicationId });
                    table.ForeignKey(
                        name: "FK_AndroidAppPackages_NixPackageRepos_NixPackageRepoId",
                        column: x => x.NixPackageRepoId,
                        principalTable: "NixPackageRepos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NixRepoCommits",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    NixRepoId = table.Column<Guid>(type: "uuid", nullable: false),
                    CommitHash = table.Column<string>(type: "text", nullable: false),
                    Processed = table.Column<bool>(type: "boolean", nullable: false),
                    CommitDateTimeOffset = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NixRepoCommits", x => x.Id);
                    table.UniqueConstraint("AK_NixRepoCommits_NixRepoId_CommitHash", x => new { x.NixRepoId, x.CommitHash });
                    table.ForeignKey(
                        name: "FK_NixRepoCommits_NixPackageRepos_NixRepoId",
                        column: x => x.NixRepoId,
                        principalTable: "NixPackageRepos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AndroidAppPackageVersions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    NixPackageRepoId = table.Column<Guid>(type: "uuid", nullable: false),
                    CommitHash = table.Column<string>(type: "text", nullable: false),
                    NixPackageName = table.Column<string>(type: "text", nullable: false),
                    AndroidApplicationId = table.Column<string>(type: "text", nullable: false),
                    AppVersionString = table.Column<string>(type: "text", nullable: false),
                    BaseVersionCode = table.Column<int>(type: "integer", nullable: false),
                    NixPackageRevision = table.Column<int>(type: "integer", nullable: false),
                    NixDerivationHash = table.Column<string>(type: "text", nullable: false),
                    ReleaseChannel = table.Column<int>(type: "integer", nullable: false),
                    AppVersionReleaseDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    AppLogoUrl = table.Column<string>(type: "text", nullable: false),
                    AppName = table.Column<string>(type: "text", nullable: false),
                    AppSummary = table.Column<string>(type: "text", nullable: false),
                    AppDescription = table.Column<string>(type: "text", nullable: false),
                    AppLicense = table.Column<string>(type: "text", nullable: false),
                    ReleaseNotes = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AndroidAppPackageVersions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AndroidAppPackageVersions_AndroidAppPackages_NixPackageRepo~",
                        columns: x => new { x.NixPackageRepoId, x.NixPackageName, x.AndroidApplicationId },
                        principalTable: "AndroidAppPackages",
                        principalColumns: new[] { "NixPackageRepoId", "NixPackageName", "AndroidApplicationId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AndroidAppPackageVersions_NixPackageRepos_NixPackageRepoId",
                        column: x => x.NixPackageRepoId,
                        principalTable: "NixPackageRepos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AndroidAppPackageVersions_NixRepoCommits_NixPackageRepoId_C~",
                        columns: x => new { x.NixPackageRepoId, x.CommitHash },
                        principalTable: "NixRepoCommits",
                        principalColumns: new[] { "NixRepoId", "CommitHash" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BuildWorkflows",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AndroidAppPackageVersionId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BuildWorkflows", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BuildWorkflows_AndroidAppPackageVersions_AndroidAppPackageV~",
                        column: x => x.AndroidAppPackageVersionId,
                        principalTable: "AndroidAppPackageVersions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Artifacts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BuildWorkflowId = table.Column<Guid>(type: "uuid", nullable: false),
                    ArtifactDataRecordId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsSigned = table.Column<bool>(type: "boolean", nullable: false),
                    FileData_FileName = table.Column<string>(type: "text", nullable: false),
                    FileData_FileSha256 = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    FileData_FileSizeBytes = table.Column<long>(type: "bigint", nullable: false),
                    FileData_FileStorageKey = table.Column<string>(type: "text", nullable: false),
                    IdSigFileData_FileName = table.Column<string>(type: "text", nullable: true),
                    IdSigFileData_FileSha256 = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    IdSigFileData_FileSizeBytes = table.Column<long>(type: "bigint", nullable: true),
                    IdSigFileData_FileStorageKey = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Artifacts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Artifacts_ArtifactDataRecords_ArtifactDataRecordId",
                        column: x => x.ArtifactDataRecordId,
                        principalTable: "ArtifactDataRecords",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Artifacts_BuildWorkflows_BuildWorkflowId",
                        column: x => x.BuildWorkflowId,
                        principalTable: "BuildWorkflows",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkflowEvents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BuildWorkflowId = table.Column<Guid>(type: "uuid", nullable: false),
                    DateTimeOffset = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    EventType = table.Column<int>(type: "integer", nullable: false),
                    Payload = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkflowEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkflowEvents_BuildWorkflows_BuildWorkflowId",
                        column: x => x.BuildWorkflowId,
                        principalTable: "BuildWorkflows",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AndroidAppPackages_NixPackageRepoId_NixPackageName_AndroidA~",
                table: "AndroidAppPackages",
                columns: new[] { "NixPackageRepoId", "NixPackageName", "AndroidApplicationId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AndroidAppPackageVersions_AppName",
                table: "AndroidAppPackageVersions",
                column: "AppName")
                .Annotation("Npgsql:IndexMethod", "gist")
                .Annotation("Npgsql:IndexOperators", new[] { "gist_trgm_ops" });

            migrationBuilder.CreateIndex(
                name: "IX_AndroidAppPackageVersions_NixPackageRepoId_CommitHash",
                table: "AndroidAppPackageVersions",
                columns: new[] { "NixPackageRepoId", "CommitHash" });

            migrationBuilder.CreateIndex(
                name: "IX_AndroidAppPackageVersions_NixPackageRepoId_NixPackageName_A~",
                table: "AndroidAppPackageVersions",
                columns: new[] { "NixPackageRepoId", "NixPackageName", "AndroidApplicationId" });

            migrationBuilder.CreateIndex(
                name: "IX_Artifacts_ArtifactDataRecordId",
                table: "Artifacts",
                column: "ArtifactDataRecordId");

            migrationBuilder.CreateIndex(
                name: "IX_Artifacts_BuildWorkflowId",
                table: "Artifacts",
                column: "BuildWorkflowId");

            migrationBuilder.CreateIndex(
                name: "IX_BuildWorkflows_AndroidAppPackageVersionId",
                table: "BuildWorkflows",
                column: "AndroidAppPackageVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_NixPackageRepos_Name",
                table: "NixPackageRepos",
                column: "Name")
                .Annotation("Npgsql:IndexMethod", "gist")
                .Annotation("Npgsql:IndexOperators", new[] { "gist_trgm_ops" });

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowEvents_BuildWorkflowId",
                table: "WorkflowEvents",
                column: "BuildWorkflowId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Artifacts");

            migrationBuilder.DropTable(
                name: "WorkflowEvents");

            migrationBuilder.DropTable(
                name: "ArtifactDataRecords");

            migrationBuilder.DropTable(
                name: "BuildWorkflows");

            migrationBuilder.DropTable(
                name: "AndroidAppPackageVersions");

            migrationBuilder.DropTable(
                name: "AndroidAppPackages");

            migrationBuilder.DropTable(
                name: "NixRepoCommits");

            migrationBuilder.DropTable(
                name: "NixPackageRepos");
        }
    }
}
