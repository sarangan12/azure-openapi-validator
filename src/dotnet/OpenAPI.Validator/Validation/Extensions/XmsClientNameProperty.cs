﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using AutoRest.Core.Logging;
using OpenAPI.Validator.Validation.Core;
using OpenAPI.Validator.Properties;
using System.Collections.Generic;
using OpenAPI.Validator.Model;

namespace OpenAPI.Validator.Validation.Extensions
{
    /// <summary>
    /// Validates if the name of property and x-ms-client-name(if exists) does not match.
    /// </summary>
    public class XmsClientNameProperty : TypedRule<Dictionary<string, Schema>>
    {
        private static readonly string extensionToCheck = "x-ms-client-name";

        /// <summary>
        /// Id of the Rule.
        /// </summary>
        public override string Id => "R2013";

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
        public override string MessageTemplate => Resources.XmsClientNameInvalid;

        /// <summary>
        /// The severity of this message (ie, debug/info/warning/error/fatal, etc)
        /// </summary>
        public override Category Severity => RulesMetaData.GetSeverity(this.Id);

        /// <summary>
        /// What kind of open api document type this rule should be applied to
        /// </summary>
        public override ServiceDefinitionDocumentType ServiceDefinitionDocumentType => RulesMetaData.GetServiceDefinitionDocumentType(this.Id);

        /// <summary>
        /// The rule could be violated by a property of a model referenced by many jsons belonging to the same
        /// composed state, to reduce duplicate messages, run validation rule in composed state
        /// </summary>
        public override ServiceDefinitionDocumentState ValidationRuleMergeState => RulesMetaData.GetServiceDefinitionDocumentState(this.Id);

        /// <summary>
        /// Validates if the name of property and x-ms-client-name(if exists) does not match.
        /// </summary>
        /// <param name="properties">Properties.</param>
        /// <param name="context">Rule context.</param>
        /// <returns></returns>
        public override IEnumerable<ValidationMessage> GetValidationMessages(Dictionary<string, Schema> properties, RuleContext context)
        {
            foreach (KeyValuePair<string, Schema> property in properties)
            {
                if ((property.Value?.Extensions?.Count ?? 0) != 0)
                {
                    string valueOfXmsExtensionProperty = (string)property.Value.Extensions.GetValueOrNull(extensionToCheck);
                    if (valueOfXmsExtensionProperty != null && valueOfXmsExtensionProperty.Equals(property.Key))
                    {
                        yield return new ValidationMessage(new FileObjectPath(context.File, context.Path), this, property.Key);
                    }
                }
            }
        }
    }
}
