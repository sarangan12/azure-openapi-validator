﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using AutoRest.Core.Logging;
using OpenAPI.Validator.Properties;
using OpenAPI.Validator.Validation.Core;
using System.Collections.Generic;
using OpenAPI.Validator.Model;

namespace OpenAPI.Validator.Validation.Extensions
{
    /// <summary>
    /// Validates if the resource definition has x-ms-azure-resource extension set to true.
    /// </summary>
    public class ResourceHasXMsResourceEnabled : TypedRule<Dictionary<string, Schema>>
    {
        private static readonly string requiredExtension = "x-ms-azure-resource";

        /// <summary>
        /// Id of the Rule.
        /// </summary>
        public override string Id => "R2019";

        /// <summary>
        /// Violation category of the Rule.
        /// </summary>
        public override ValidationCategory ValidationCategory => RulesMetaData.GetValidationCategory(this.Id);

        /// <summary>
        /// The template message for this Rule.
        /// </summary>
        /// <remarks>
        /// This may contain placeholders '{0}' for parameterized messages.
        /// </remarks>
        public override string MessageTemplate => Resources.ResourceIsMsResourceNotValid;

        /// <summary>
        /// The severity of this message (ie, debug/info/warning/error/fatal, etc)
        /// </summary>
        public override Category Severity => RulesMetaData.GetSeverity(this.Id);

        /// <summary>
        /// What kind of open api document type this rule should be applied to
        /// </summary>
        public override ServiceDefinitionDocumentType ServiceDefinitionDocumentType => RulesMetaData.GetServiceDefinitionDocumentType(this.Id);

        /// <summary>
        /// The rule could be violated by a model referenced by many jsons belonging to the same
        /// composed state, to reduce duplicate messages, run validation rule in composed state
        /// </summary>
        public override ServiceDefinitionDocumentState ValidationRuleMergeState => RulesMetaData.GetServiceDefinitionDocumentState(this.Id);


        /// <summary>
        /// An <paramref name="definitions"/> fails this rule if the resource definition does not have 
        /// x-ms-azure-resource extension set to true.
        /// </summary>
        /// <param name="definitions">Operation Definition to validate</param>
        /// <returns>true if the validation succeeds. false otherwise.</returns>
        public override bool IsValid(Dictionary<string, Schema> definitions)
        {
            foreach (string key in definitions.Keys)
            {
                if (key.ToLower().Equals("resource"))
                {
                    Schema resourceSchema = definitions.GetValueOrNull(key);
                    if (resourceSchema == null ||
                        resourceSchema.Extensions == null ||
                        resourceSchema.Extensions.Count <= 0 ||
                        resourceSchema.Extensions.GetValueOrNull(requiredExtension) == null ||
                        (bool)resourceSchema.Extensions.GetValueOrNull(requiredExtension) == false)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
