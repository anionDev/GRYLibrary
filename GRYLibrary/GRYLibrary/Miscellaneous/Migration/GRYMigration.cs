using GRYLibrary.Core.APIServer.ConcreteEnvironments;
using GRYLibrary.Core.APIServer.ExecutionModes;
using GRYLibrary.Core.Miscellaneous.FilePath;
using GRYLibrary.Core.Miscellaneous.Migration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using GRYLibrary.Core.Logging.GeneralPurposeLogger;

namespace GRYLibrary.Core.Miscellaneous
{
    public class GRYMigration
    {
        internal static Encoding Encoding { get; set; } = new UTF8Encoding(false);
        public static void MigrateIfRequired(
            AbstractFilePath basicInformationFile, string appName, Version3 currentVersion, IGeneralLogger logger,
            string baseFolder, GRYEnvironment targetEnvironmentType, ExecutionMode executionMode,
            MigrateInstanceInformation migrateInstanceInformation
        )
        {
            string informationFile = basicInformationFile.GetPath(baseFolder);
            if (File.Exists(informationFile))
            {
                ApplicationInformation appInformation = GetInformationFromFile(informationFile);
                Utilities.AssertCondition(appInformation.ApplicationName == appName);
                Version3 existingVersion = appInformation.GetCodeUnitVersion();
                if (migrateInstanceInformation.MetaMigrations.Count > 0)
                {
                    Utilities.AssertCondition(existingVersion <= currentVersion);
                    if (existingVersion != currentVersion)
                    {
                        List<MigrationMetaInformation> orderedMigrations = migrateInstanceInformation.MetaMigrations.ToList();
                        orderedMigrations.Sort((x, y) =>
                        {
                            if (x.SourceVersion < y.SourceVersion)
                            {
                                return -1;
                            }
                            else if (x.SourceVersion > y.SourceVersion)
                            {
                                return 1;
                            }
                            else
                            {
                                return 0;
                            }
                        });
                        for (int i = 0; i < orderedMigrations.Count; i++)
                        {
                            MigrationMetaInformation migration = orderedMigrations[i];
                            Utilities.AssertCondition(migration.SourceVersion <= migration.TargetVersion);
                            bool isLastMigration = i == orderedMigrations.Count - 1;
                            if (!isLastMigration)
                            {
                                MigrationMetaInformation nextMigration = orderedMigrations[i + 1];
                                Utilities.AssertCondition(migration.TargetVersion <= nextMigration.SourceVersion);
                            }
                        }
                        foreach (MigrationMetaInformation orderedMigration in orderedMigrations)
                        {
                            bool migrationIfRequired = existingVersion < orderedMigration.SourceVersion;
                            logger.Log($"Start migration from version {orderedMigration.SourceVersion} to {orderedMigration.TargetVersion}.", Microsoft.Extensions.Logging.LogLevel.Information);
                            try
                            {
                                orderedMigration.Migration(new MigrationInformation(orderedMigration.SourceVersion, baseFolder, targetEnvironmentType, executionMode, migrateInstanceInformation.CustomValues));
                                existingVersion = orderedMigration.TargetVersion;
                                WriteInformationToFile(informationFile, appName, existingVersion);
                                logger.Log($"Finished migration from version {orderedMigration.SourceVersion} to {orderedMigration.TargetVersion}.", Microsoft.Extensions.Logging.LogLevel.Information);
                            }
                            catch (Exception exception)
                            {
                                logger.LogException(exception, $"Error while migration.");
                            }
                        }
                    }
                }
            }
            else
            {
                Utilities.EnsureFileExists(informationFile, true);
                WriteInformationToFile(informationFile, appName, currentVersion);
            }
        }

        private static ApplicationInformation GetInformationFromFile(string informationFile)
        {
            SimpleObjectPersistence<ApplicationInformation> simpleObjectPersistence = new SimpleObjectPersistence<ApplicationInformation>
            {
                File = informationFile
            };
            simpleObjectPersistence.LoadObjectFromFile();
            return simpleObjectPersistence.Object;
        }

        private static void WriteInformationToFile(string informationFile, string appName, Version3 version)
        {
            SimpleObjectPersistence<ApplicationInformation> simpleObjectPersistence = new SimpleObjectPersistence<ApplicationInformation>
            {
                File = informationFile,
                Object = new ApplicationInformation(appName, version.ToString())
            };
            simpleObjectPersistence.SaveObjectToFile();
        }
        public class ApplicationInformation
        {
            public string ApplicationName { get; set; }

            public string ApplicationVersion { get; set; }

            public ApplicationInformation()
            {
            }
            public ApplicationInformation(string applicationName, string applicationVersion)
            {
                this.ApplicationName = applicationName;
                this.ApplicationVersion = applicationVersion;
            }

            public Version3 GetCodeUnitVersion()
            {
                return Version3.Parse(this.ApplicationVersion);
            }
        }
    }
}
