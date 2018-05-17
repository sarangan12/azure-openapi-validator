﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using AutoRest.Core.Logging;
using OpenAPI.Validator.Model;
using OpenAPI.Validator.Properties;
using OpenAPI.Validator.Validation.Core;
using System.Collections.Generic;
using System.Linq;

namespace OpenAPI.Validator.Validation
{
    public class LongRunningOperationsWithLongRunningExtension : TypedRule<Operation>
    {
        /// <summary>
        /// Id of the Rule.
        /// </summary>
        public override string Id => "R2007";

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
        public override string MessageTemplate => Resources.LongRunningOperationsWithLongRunningExtension;

        /// <summary>
        /// The severity of this message (ie, debug/info/warning/error/fatal, etc)
        /// </summary>
        public override Category Severity => RulesMetaData.GetSeverity(this.Id);

        /// <summary>
        /// What kind of open api document type this rule should be applied to
        /// </summary>
        public override ServiceDefinitionDocumentType ServiceDefinitionDocumentType => RulesMetaData.GetServiceDefinitionDocumentType(this.Id);

        /// <summary>
        /// What kind of change implementing this rule can cause.
        /// </summary>
        public override ValidationChangesImpact ValidationChangesImpact => RulesMetaData.GetValidationChangesImpact(this.Id);

        /// <summary>
        /// The rule could be violated by a model referenced by many jsons belonging to the same
        /// composed state, to reduce duplicate messages, run validation rule in composed state
        /// </summary>
        public override ServiceDefinitionDocumentState ValidationRuleMergeState => RulesMetaData.GetServiceDefinitionDocumentState(this.Id);

        /// <summary>
        /// Validates if the long running operation has long running extension enabled.
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="context"></param>
        /// <returns>true if the long running operation has long running extension enabled. false otherwise.</returns>
        public override IEnumerable<ValidationMessage> GetValidationMessages(Operation operation, RuleContext context)
        {
            if (operation.Responses?.Any(response => response.Key.Equals("202")) == true &&
              operation.Extensions?.Any(extension => extension.Key.Equals("x-ms-long-running-operation") && (bool)extension.Value) == false)
            {
                yield return new ValidationMessage(new FileObjectPath(context.File, context.Path), this, operation.OperationId);
            }
        }
    }
}
