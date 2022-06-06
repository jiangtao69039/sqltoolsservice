//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.SqlTools.Hosting.Protocol;
using Microsoft.SqlTools.ServiceLayer.IntegrationTests.Utility;
using Microsoft.SqlTools.ServiceLayer.Migration;
using Microsoft.SqlTools.ServiceLayer.Migration.Contracts;
using Microsoft.SqlTools.ServiceLayer.Test.Common;
using Microsoft.SqlTools.ServiceLayer.Test.Common.RequestContextMocking;
using Moq;
using NUnit.Framework;

namespace Microsoft.SqlTools.ServiceLayer.IntegrationTests.Migration
{
    public class MigrationServiceTests
    {
        [Test]
        public async Task TestHandleMigrationAssessmentRequest()
        {
            using (SelfCleaningTempFile queryTempFile = new SelfCleaningTempFile())
            {
                var connectionResult = await LiveConnectionHelper.InitLiveConnectionInfoAsync("master", queryTempFile.FilePath);

                var requestParams = new MigrationAssessmentsParams()
                {
                    OwnerUri = connectionResult.ConnectionInfo.OwnerUri
                };

                var requestContext = new Mock<RequestContext<MigrationAssessmentResult>>();

                MigrationService service = new MigrationService();
                await service.HandleMigrationAssessmentsRequest(requestParams, requestContext.Object);
                requestContext.VerifyAll();
            }
        }

        [Test]
        [Ignore("Test assessment with actual database name")]
        public async Task TestHandleSaveAssessmentRequest()
        {
            MigrationAssessmentResult assessmentResult = null;
            SaveAssessmentResult saveAssessmentResult = null;

            using (SelfCleaningTempFile queryTempFile = new SelfCleaningTempFile())
            {
                var connectionResult = await LiveConnectionHelper.InitLiveConnectionInfoAsync("master", queryTempFile.FilePath);

                var requestParams = new MigrationAssessmentsParams()
                {
                    OwnerUri = connectionResult.ConnectionInfo.OwnerUri,
                    // To test assessment workflow update below database from local sql instance
                    Databases = new string[] { "AdventureWorks2016" }
                };

                var requestContext = RequestContextMocks.Create<MigrationAssessmentResult>(r => assessmentResult = r).AddErrorHandling(null);

                MigrationService service = new MigrationService();
                await service.HandleMigrationAssessmentsRequest(requestParams, requestContext.Object);

                var saveAssessmentParams = new SaveAssessmentResultParams
                {
                    AssessmentResult = assessmentResult.RawAssessmentResult
                };

                var saveAssessmentRequestContext = RequestContextMocks.Create<SaveAssessmentResult>(r => saveAssessmentResult = r).AddErrorHandling(null);
                await service.HandleSaveAssessmentRequest(saveAssessmentParams, saveAssessmentRequestContext.Object);
            }
        }

        [Test]
        [Ignore ("Disable failing test")]
        public async Task TestHandleMigrationGetSkuRecommendationsRequest()
        {
            GetSkuRecommendationsResult result = null;

            var requestParams = new GetSkuRecommendationsParams()
            {
                DataFolder = Path.Combine("..", "..", "..", "Migration", "Data"),
                TargetPlatforms = new List<string> { "AzureSqlManagedInstance" },
                TargetSqlInstance = "Test",
                TargetPercentile = 95,
                StartTime = new DateTime(2020, 01, 01).ToString("yyyy-MM-dd HH:mm:ss"),
                EndTime = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"),
                PerfQueryIntervalInSec = 30,
                ScalingFactor = 1,
                DatabaseAllowList = new List<string> { "test", "test1" }
            };

            var requestContext = RequestContextMocks.Create<GetSkuRecommendationsResult>(r => result = r).AddErrorHandling(null);

            MigrationService service = new MigrationService();
            await service.HandleGetSkuRecommendationsRequest(requestParams, requestContext.Object);
            Assert.IsNotNull(result, "Get SKU Recommendation result is null");
            Assert.IsNotNull(result.SqlMiRecommendationResults, "Get MI SKU Recommendation result is null");
            // TODO: Include Negative Justification in future when we start recommending more than one SKU.
            Assert.Greater(result.SqlMiRecommendationResults.First().PositiveJustifications.Count, 0, "No positive justification for MI SKU Recommendation result");

            Assert.IsNotNull(result.InstanceRequirements);
            Assert.AreEqual(result.InstanceRequirements.InstanceId, "TEST");
            Assert.AreEqual(result.InstanceRequirements.DatabaseLevelRequirements.Count, 2);
            Assert.AreEqual(result.InstanceRequirements.DatabaseLevelRequirements.Sum(db => db.FileLevelRequirements.Count), 4);

            var saveSkuRecommendationsParams = new SaveSkuRecommendationsParams
            {
                SqlMiRecommendationResults = result.SqlMiRecommendationResults,
                SqlVmRecommendationResults = result.SqlVmRecommendationResults,
                InstanceRequirements = result.InstanceRequirements
            };

            SaveSkuRecommendationsResult saveSkuRecommendationsResult = null;
            var saveSkuRecommendationsRequestContext = RequestContextMocks.Create<SaveSkuRecommendationsResult>(r => saveSkuRecommendationsResult = r).AddErrorHandling(null);
            await service.HandleSaveSkuRecommendationsRequest(saveSkuRecommendationsParams, saveSkuRecommendationsRequestContext.Object);
            Assert.IsNotNull(saveSkuRecommendationsResult, "Save SKU Recommendation results is null");
            Assert.IsNotNull(saveSkuRecommendationsResult.SkuRecommendationsReportFileNames, "SKU Recommendation file names is null");
            Assert.IsTrue(saveSkuRecommendationsResult.SkuRecommendationsReportFileNames.Count > 0, " Invalid number of sku recommendation files");
            Assert.IsTrue(saveSkuRecommendationsResult.SkuRecommendationsReportFileNames.FirstOrDefault()?.StartsWith("SkuRecommendation"));
        }

        [Test]
        public async Task TestHandleStartStopPerfDataCollectionRequest()
        {
            StartPerfDataCollectionResult result = null;
            using (SelfCleaningTempFile queryTempFile = new SelfCleaningTempFile())
            {
                var connectionResult = await LiveConnectionHelper.InitLiveConnectionInfoAsync("master", queryTempFile.FilePath);
                string folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "SkuRecommendationTest");
                Directory.CreateDirectory(folderPath);

                var requestParams = new StartPerfDataCollectionParams()
                {
                    OwnerUri = connectionResult.ConnectionInfo.OwnerUri,
                    DataFolder = folderPath,
                    PerfQueryIntervalInSec = 30,
                    NumberOfIterations = 20,
                    StaticQueryIntervalInSec = 3600,
                };

                var requestContext = RequestContextMocks.Create<StartPerfDataCollectionResult>(r => result = r).AddErrorHandling(null);

                MigrationService service = new MigrationService();
                await service.HandleStartPerfDataCollectionRequest(requestParams, requestContext.Object);
                Assert.IsNotNull(result, "Start Perf Data Collection result is null");
                Assert.IsNotNull(result.DateTimeStarted, "Time perf data collection started is null");

                // Stop data collection
                StopPerfDataCollectionResult stopResult = null;
                var stopRequestParams = new StopPerfDataCollectionParams()
                {

                };

                var stopRequestContext = RequestContextMocks.Create<StopPerfDataCollectionResult>(r => stopResult = r).AddErrorHandling(null);

                await service.HandleStopPerfDataCollectionRequest(stopRequestParams, stopRequestContext.Object);
                Assert.IsNotNull(stopResult, "Stop Perf Data Collection result is null");
                Assert.IsNotNull(stopResult.DateTimeStopped, "Time perf data collection stoped is null");
            }
        }
    }
}