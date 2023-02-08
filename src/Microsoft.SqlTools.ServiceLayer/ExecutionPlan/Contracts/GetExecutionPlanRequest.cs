//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//

#nullable disable

using System.Collections.Generic;
using Microsoft.SqlTools.Hosting.Protocol.Contracts;
using Microsoft.SqlTools.ServiceLayer.ExecutionPlan.Contracts;

namespace Microsoft.SqlTools.ServiceLayer.ExecutionPlan
{
    public class GetExecutionPlanParams
    {
        public ExecutionPlanGraphInfo GraphInfo { get; set; }
    }

    public class GetExecutionPlanResult
    {
        public List<ExecutionPlanGraph> Graphs { get; set; }
    }

    public class GetExecutionPlanRequest
    {
        public static readonly
        RequestType<GetExecutionPlanParams, GetExecutionPlanResult> Type = 
         RequestType<GetExecutionPlanParams, GetExecutionPlanResult>.Create("queryExecutionPlan/getExecutionPlan");
    }
}
