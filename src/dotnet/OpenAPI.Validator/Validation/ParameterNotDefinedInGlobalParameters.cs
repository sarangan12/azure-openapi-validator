﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using AutoRest.Core.Logging;
using OpenAPI.Validator.Properties;
using OpenAPI.Validator.Validation.Core;
using OpenAPI.Validator.Model;
using System.Collections.Generic;
using System.Linq;

namespace OpenAPI.Validator.Validation
{
    public class ParameterNotDefinedInGlobalParameters : TypedRule<Dictionary<string, SwaggerParameter>>
    {
        private const string SubscriptionId = "subscriptionid";
        private const string ApiVersion = "api-version";

        /// <summary>
        /// Id of the Rule.
        /// </summary>
        public override string Id => "R2015";

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
        /// <remarks>
        /// This may contain placeholders '{0}' for parameterized messages.
        /// </remarks>
        public override string MessageTemplate => Resources.ServiceDefinitionParametersMissingMessage;


        /// <summary>
        /// What kind of change implementing this rule can cause.
        /// </summary>
        public override ValidationChangesImpact ValidationChangesImpact => RulesMetaData.GetValidationChangesImpact(this.Id);

        /// <summary>
        /// What kind of open api document type this rule should be applied to
        /// </summary>
        public override ServiceDefinitionDocumentType ServiceDefinitionDocumentType => RulesMetaData.GetServiceDefinitionDocumentType(this.Id);


        /// <summary>
        /// Global parameters of each individual json need to be validated, hence individual state
        /// </summary>
        public override ServiceDefinitionDocumentState ValidationRuleMergeState => RulesMetaData.GetServiceDefinitionDocumentState(this.Id);

        /// <summary>
        /// This rule passes if the parameters section contains both subscriptionId and api-version parameters
        /// </summary>
        /// <param name="paths"></param>
        /// <returns></returns>
        public override IEnumerable<ValidationMessage> GetValidationMessages(Dictionary<string, SwaggerParameter> ParametersMap, RuleContext context)
        {
            var serviceDefinition = (ServiceDefinition)context.Root;
            // Check if subscriptionId is used but not defined in global parameters
            bool isSubscriptionIdReferenced = serviceDefinition.Paths.Keys.Any(key => key.ToLower().Contains("{" + SubscriptionId.ToLower() + "}"));
            if (isSubscriptionIdReferenced && (ParametersMap?.Values.Any(parameter => parameter.Name?.ToLower().Equals(SubscriptionId) == true)) == false)
            {
                yield return new ValidationMessage(new FileObjectPath(context.File, context.Path), this, SubscriptionId);
            }
            // For ARM specs, api version is almost always required, call it out if it isn't defined in the global params
            // We are not distinguishing between ARM and non-ARM specs currently, so let's apply this for all specs regardless
            // and make appropriate changes in the future so this gets applied only for ARM specs
            if (ParametersMap?.Values.Any(parameter => parameter.Name?.ToLower().Equals(ApiVersion) == true) == false)
            {
                yield return new ValidationMessage(new FileObjectPath(context.File, context.Path), this, ApiVersion);
            }
        }
    }
}
