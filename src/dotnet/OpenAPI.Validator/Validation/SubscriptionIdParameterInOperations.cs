﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using AutoRest.Core.Logging;
using OpenAPI.Validator.Properties;
using OpenAPI.Validator.Validation.Core;
using OpenAPI.Validator.Model;

namespace OpenAPI.Validator.Validation
{
    public class SubscriptionIdParameterInOperations : TypedRule<SwaggerParameter>
    {
        private const string SubscriptionId = "subscriptionid";

        /// <summary>
        /// Id of the Rule.
        /// </summary>
        public override string Id => "R2014";

        /// <summary>
        /// Violation category of the Rule.
        /// </summary>
        public override ValidationCategory ValidationCategory => RulesMetaData.GetValidationCategory(this.Id);


        /// <summary>
        /// What kind of open api document type this rule should be applied to
        /// </summary>
        public override ServiceDefinitionDocumentType ServiceDefinitionDocumentType => RulesMetaData.GetServiceDefinitionDocumentType(this.Id);

        /// <summary>
        /// The rule runs on each operation's parameters section in isolation irrespective of the state and can be run in individual state
        /// </summary>
        public override ServiceDefinitionDocumentState ValidationRuleMergeState => RulesMetaData.GetServiceDefinitionDocumentState(this.Id);

        /// <summary>
        /// This rule passes if the parameters are not subscriptionId or api-version
        /// </summary>
        /// <param name="paths"></param>
        /// <returns></returns>
        public override bool IsValid(SwaggerParameter Parameter) =>
           (!string.IsNullOrEmpty(Parameter.Reference) || Parameter.Schema != null || Parameter.Name?.ToLower().Equals(SubscriptionId) == false);

        /// <summary>
        /// The template message for this Rule. 
        /// </summary>
        /// <remarks>
        /// This may contain placeholders '{0}' for parameterized messages.
        /// </remarks>
        public override string MessageTemplate => Resources.OperationParametersNotAllowedMessage;

        /// <summary>
        /// The severity of this message (ie, debug/info/warning/error/fatal, etc)
        /// </summary>
        public override Category Severity => RulesMetaData.GetSeverity(this.Id);


        /// <summary>
        /// What kind of change implementing this rule can cause.
        /// </summary>
        public override ValidationChangesImpact ValidationChangesImpact => RulesMetaData.GetValidationChangesImpact(this.Id);

    }
}
