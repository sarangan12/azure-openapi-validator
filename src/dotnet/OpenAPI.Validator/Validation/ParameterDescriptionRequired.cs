﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using OpenAPI.Validator.Model;
using OpenAPI.Validator.Validation.Core;

namespace OpenAPI.Validator.Validation
{
    public class ParameterDescriptionRequired : DescriptionRequired<SwaggerParameter>
    {
        private static readonly string ParameterTypeFormatter = "'{0}' parameter";

        /// <summary>
        /// What kind of open api document type this rule should be applied to
        /// </summary>
        public override ServiceDefinitionDocumentType ServiceDefinitionDocumentType => RulesMetaData.GetServiceDefinitionDocumentType(this.Id);

        /// <summary>
        /// The rule could be violated by a parameter referenced by many jsons belonging to the same
        /// composed state, to reduce duplicate messages, run validation rule in composed state
        /// </summary>
        public override ServiceDefinitionDocumentState ValidationRuleMergeState => RulesMetaData.GetServiceDefinitionDocumentState(this.Id);

        /// <summary>
        /// This rule fails if the description is null and the reference is null (since the reference could have a description)
        /// </summary>
        /// <param name="entity">Entity being validated</param>
        /// <param name="context">Rule context</param>
        /// <param name="formatParameters">formatted parameters</param>
        /// <returns><c>true</c> if entity contains description, <c>false</c> otherwise</returns>
        public override bool IsValid(SwaggerParameter entity, RuleContext context, out object[] formatParameters)
        {
            formatParameters = new string[] { string.Format(ParameterTypeFormatter, entity.Name) };
            return !string.IsNullOrWhiteSpace(entity.Description) || !string.IsNullOrWhiteSpace(entity.Reference);
        }
    }
}
