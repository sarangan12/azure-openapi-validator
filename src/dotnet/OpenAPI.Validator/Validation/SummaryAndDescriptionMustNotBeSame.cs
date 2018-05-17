﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using AutoRest.Core.Logging;
using OpenAPI.Validator.Properties;
using OpenAPI.Validator.Model;
using OpenAPI.Validator.Validation.Core;
using System;
using System.Collections.Generic;

namespace OpenAPI.Validator.Validation
{
    public class SummaryAndDescriptionMustNotBeSame : TypedRule<Operation>
    {
        /// <summary>
        /// Id of the Rule.
        /// </summary>
        public override string Id => "R2023";

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
        public override string MessageTemplate => Resources.SummaryDescriptionVaidationError;

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
        /// The severity of this message (ie, debug/info/warning/error/fatal, etc)
        /// </summary>
        public override Category Severity => RulesMetaData.GetSeverity(this.Id);

        public override IEnumerable<ValidationMessage> GetValidationMessages(Operation operation, RuleContext context)
        {
            if (operation.Description != null && operation.Summary != null && operation.Description.Trim().Equals(operation.Summary.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                yield return new ValidationMessage(new FileObjectPath(context.File, context.Path), this, new object[0]);
            }
        }
    }
}
