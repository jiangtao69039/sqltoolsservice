//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//

namespace Microsoft.SqlTools.ServiceLayer.ObjectManagement
{
    /// <summary>
    /// A class for storing various properties needed for Saving & Scripting a database
    /// </summary>
    public class DatabaseInfo : SqlObject
    {
        public string? Owner { get; set; }
        public string? CollationName { get; set; }
        public string? RecoveryModel { get; set; }
        public string? CompatibilityLevel { get; set; }
        public string? ContainmentType { get; set; }
        public string? DateCreated { get; set; }
        public string? LastDatabaseBackup { get; set; }
        public string? LastDatabaseLogBackup { get; set; }
        public double? MemoryAllocatedToMemoryOptimizedObjectsInMb { get; set; }
        public double? MemoryUsedByMemoryOptimizedObjectsInMb { get; set; }
        public int? NumberOfUsers { get; set; }
        public double? SizeInMb { get; set; }
        public double? SpaceAvailableInMb { get; set; }
        public string? Status { get; set; }
        public string? AzureBackupRedundancyLevel { get; set; }
        public string? AzureServiceLevelObjective { get; set; }
        public string? AzureEdition { get; set; }
        public string? AzureMaxSize { get; set; }
        public bool AutoCreateIncrementalStatistics { get; set; }
        public bool AutoCreateStatistics { get; set; }
        public bool AutoShrink { get; set; }
        public bool AutoUpdateStatistics { get; set; }
        public bool AutoUpdateStatisticsAsynchronously { get; set; }
        public bool? IsLedgerDatabase { get; set; }
        public string? PageVerify { get; set; }
        public int? TargetRecoveryTimeInSec { get; set; }
        public bool? DatabaseReadOnly { get; set; }
        public bool EncryptionEnabled { get; set; }
        public string? RestrictAccess { get; set; }
        public DatabaseScopedConfigurationsInfo[]? DatabaseScopedConfigurations { get; set; }
        public QueryStoreOptions? QueryStoreOptions { get; set; }
    }

    public class DatabaseScopedConfigurationsInfo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ValueForPrimary { get; set; }
        public string ValueForSecondary { get; set; }
    }

    public class QueryStoreOptions
    {
        public string ActualMode { get; set; }
        public long DataFlushIntervalInMinutes { get; set; }
        public string StatisticsCollectionInterval { get; set; }
        public long MaxPlansPerQuery { get; set; }
        public long MaxSizeInMB { get; set; }
        public string QueryStoreCaptureMode { get; set; }
        public string SizeBasedCleanupMode { get; set; }
        public long StaleQueryThresholdInDays { get; set; }
        public bool WaitStatisticsCaptureMode { get; set; }

    }
}