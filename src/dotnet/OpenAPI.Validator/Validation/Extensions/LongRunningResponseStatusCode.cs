﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using AutoRest.Core.Logging;
using OpenAPI.Validator.Model;
using OpenAPI.Validator.Properties;
using OpenAPI.Validator.Validation.Core;

namespace OpenAPI.Validator.Validation.Extensions
{
    public class LongRunningResponseStatusCode : ExtensionRule
    {
        private static readonly IEnumerable<string> DeleteAllowedCodes = new List<string>() { "200", "204" };
        private static readonly IEnumerable<string> PostAllowedCodes = new List<string>() { "200", "201", "204" };
        private static readonly IEnumerable<string> PutPatchAllowedCodes = new List<string>() { "200", "201" };

        protected override string ExtensionName => "x-ms-long-running-operation";

        /// <summary>
        /// Id of the Rule.
        /// </summary>
        public override string Id => "R2005";

        /// <summary>
        /// Violation category of the Rule.
        /// </summary>
        public override ValidationCategory ValidationCategory => RulesMetaData.GetValidationCategory(this.Id);

        /// <summary>
        /// The severity of this message (ie, debug/info/warning/error/fatal, etc)
        /// </summary>
        public override Category Severity => RulesMetaData.GetSeverity(this.Id);

        /// <summary>
        /// The template message for this Rule.
        /// </summary>
        public override string MessageTemplate => Resources.LongRunningResponseNotValid;

        protected static bool AreValidResponseCodes(RuleContext context, IEnumerable<string> statusCodes)
            => statusCodes.Any(statusCode => context?.GetFirstAncestor<Operation>()?.Responses?.GetValueOrNull(statusCode) != null);

        /// <summary>
        /// Returns http verb for the given Rule context.
        /// </summary>
        /// <param name="context">Rule context.</param>
        /// <returns>http verb as string</returns>
        protected static string GetHttpVerb(RuleContext context) => context?.Parent?.Parent?.Key;

        /// <summary>
        /// What kind of open api document type this rule should be applied to
        /// </summary>
        public override ServiceDefinitionDocumentType ServiceDefinitionDocumentType => RulesMetaData.GetServiceDefinitionDocumentType(this.Id);

        /// <summary>
        /// The rule runs on each operation in isolation irrespective of the state and can be run in individual state
        /// </summary>
        public override ServiceDefinitionDocumentState ValidationRuleMergeState => RulesMetaData.GetServiceDefinitionDocumentState(this.Id);

        /// <summary>
        /// An x-ms-long-running-operation extension passes this rule if the operation that this extension has a valid response defined.
        /// </summary>
        /// <param name="longRunning">long running extension object.</param>
        /// <param name="context">Rule context.</param>
        /// <param name="formatParameters">list of parameters for message formatter.</param>
        /// <returns><c>true</c> if operation has valid response code modeled, otherwise <c>false</c>.</returns>
        /// <remarks>This rule corresponds to M2005.</remarks>
        public override bool IsValid(object longRunning, RuleContext context, out object[] formatParameters)
        {
            bool isValid = true;
            var operation = context?.GetFirstAncestor<Operation>();
            formatParameters = new string[3];
            string httpVerb = GetHttpVerb(context);

            if ("delete".EqualsIgnoreCase(httpVerb))
            {
                isValid = AreValidResponseCodes(context, DeleteAllowedCodes);
                formatParameters[2] = string.Join(" or ", DeleteAllowedCodes);
            }
            else if ("post".EqualsIgnoreCase(httpVerb))
            {
                isValid = AreValidResponseCodes(context, PostAllowedCodes);
                formatParameters[2] = string.Join(" or ", PostAllowedCodes);
            }
            else if ("put".EqualsIgnoreCase(httpVerb) || "patch".EqualsIgnoreCase(httpVerb))
            {
                isValid = AreValidResponseCodes(context, PutPatchAllowedCodes);
                formatParameters[2] = string.Join(" or ", PutPatchAllowedCodes);
            }

            formatParameters[0] = httpVerb.ToUpper();
            formatParameters[1] = operation.OperationId ?? string.Empty;
            return isValid;
        }
    }
}
