﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using OpenAPI.Validator.Validation.Core;
using System.Collections.Generic;
using OpenAPI.Validator.Properties;
using OpenAPI.Validator.Model;
using AutoRest.Core.Logging;

namespace OpenAPI.Validator.Validation
{
    public class RequiredReadOnlyProperties : TypedRule<Schema>
    {
        /// <summary>
        /// Id of the Rule.
        /// </summary>
        public override string Id => "R2056";

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
        public override string MessageTemplate => Resources.RequiredReadOnlyPropertiesValidation;

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
        /// Validates if a property is marked as required, it should not be read only.
        /// </summary>
        /// <param name="definition"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override IEnumerable<ValidationMessage> GetValidationMessages(Schema definition, RuleContext context)
        {
            if (definition.Properties != null)
            {
                foreach (KeyValuePair<string, Schema> property in definition.Properties)
                {
                    if (property.Value.ReadOnly && definition.Required?.Contains(property.Key) == true)
                    {
                        yield return new ValidationMessage(new FileObjectPath(context.File, context.Path.AppendProperty("properties").AppendProperty(property.Key)), this, property.Key);
                    }
                }
            }
        }
    }
}
