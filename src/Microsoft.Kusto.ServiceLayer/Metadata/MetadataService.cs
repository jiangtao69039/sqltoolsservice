//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.SqlTools.Hosting.Protocol;
using Microsoft.Kusto.ServiceLayer.Connection;
using Microsoft.Kusto.ServiceLayer.Metadata.Contracts;
using Microsoft.Kusto.ServiceLayer.DataSource;
using Microsoft.Kusto.ServiceLayer.DataSource.Metadata;

namespace Microsoft.Kusto.ServiceLayer.Metadata
{
    /// <summary>
    /// Main class for Metadata Service functionality
    /// </summary>
    public sealed class MetadataService
    {
        private static ConnectionService _connectionService;
        private static readonly Lazy<MetadataService> LazyInstance = new Lazy<MetadataService>();
        public static MetadataService Instance => LazyInstance.Value;

        /// <summary>
        /// Initializes the Metadata Service instance
        /// </summary>
        /// <param name="serviceHost"></param>
        /// <param name="connectionService"></param>
        public void InitializeService(IProtocolEndpoint serviceHost, ConnectionService connectionService)
        {
            _connectionService = connectionService;
            serviceHost.SetRequestHandler(MetadataListRequest.Type, HandleMetadataListRequest);
        }

        /// <summary>
        /// Handle a metadata query request
        /// </summary>        
        internal async Task HandleMetadataListRequest(MetadataQueryParams metadataParams, RequestContext<MetadataQueryResult> requestContext)
        {
            try
            {
                var metadata = new List<ObjectMetadata>();
                Parallel.Invoke(() => metadata = LoadMetadata(metadataParams));
                
                await requestContext.SendResult(new MetadataQueryResult
                {
                    Metadata = metadata.ToArray()
                });
                
            }
            catch (Exception ex)
            {
                await requestContext.SendError(ex.ToString());
            }
        }

        private List<ObjectMetadata> LoadMetadata(MetadataQueryParams metadataParams)
        {
            _connectionService.TryFindConnection(metadataParams.OwnerUri, out ConnectionInfo connInfo);

            if (connInfo == null)
            {
                return new List<ObjectMetadata>();
            }

            connInfo.TryGetConnection(ConnectionType.Default, out ReliableDataSourceConnection connection);
            IDataSource dataSource = connection.GetUnderlyingConnection();
            
            var clusterMetadata = MetadataFactory.CreateClusterMetadata(connInfo.ConnectionDetails.ServerName);
            var databaseMetadata = MetadataFactory.CreateDatabaseMetadata(clusterMetadata, connInfo.ConnectionDetails.DatabaseName);
            var parentMetadata = dataSource.DataSourceType == DataSourceType.LogAnalytics ? clusterMetadata : databaseMetadata; 
            
            var databaseChildMetadataInfo = dataSource.GetChildObjects(parentMetadata, true);
            return MetadataFactory.ConvertToObjectMetadata(databaseChildMetadataInfo);
        }
    }
}
