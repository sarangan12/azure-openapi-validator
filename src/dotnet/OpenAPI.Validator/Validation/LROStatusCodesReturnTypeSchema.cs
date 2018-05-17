﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using AutoRest.Core.Logging;
using OpenAPI.Validator.Model.Utilities;
using System.Collections.Generic;
using OpenAPI.Validator.Model;
using System.Linq;
using OpenAPI.Validator.Validation.Core;

namespace OpenAPI.Validator.Validation
{
    public class LROStatusCodesReturnTypeSchema : TypedRule<Dictionary<string, Dictionary<string, Operation>>>
    {

        /// <summary>
        /// Id of the Rule.
        /// </summary>
        public override string Id => "R2064";

        /// <summary>
        /// Violation category of the Rule.
        /// </summary>
        public override ValidationCategory ValidationCategory => RulesMetaData.GetValidationCategory(this.Id);

        /// <summary>
        /// What kind of change implementing this rule can cause.
        /// </summary>
        public override ValidationChangesImpact ValidationChangesImpact => RulesMetaData.GetValidationChangesImpact(this.Id);

        /// <summary>
        /// The template message for this Rule. 
        /// </summary>
        /// <remarks>
        /// This may contain placeholders '{0}' for parameterized messages.
        /// </remarks>
        public override string MessageTemplate => "200/201 Responses of long running operations must have a schema definition for return type. OperationId: '{0}', Response code: '{1}'";

        /// <summary>
        /// The severity of this message (ie, debug/info/warning/error/fatal, etc)
        /// </summary>
        public override Category Severity => RulesMetaData.GetSeverity(this.Id);

        /// <summary>
        /// What kind of open api document type this rule should be applied to
        /// </summary>
        public override ServiceDefinitionDocumentType ServiceDefinitionDocumentType => RulesMetaData.GetServiceDefinitionDocumentType(this.Id);

        /// <summary>
        /// The rule runs on each operation in isolation irrespective of the state and can be run in individual state
        /// </summary>
        public override ServiceDefinitionDocumentState ValidationRuleMergeState => RulesMetaData.GetServiceDefinitionDocumentState(this.Id);

        /// <summary>
        /// Verifies whether an LRO PUT operation returns response models for 
        /// 200/201 status codes
        /// </summary>
        /// <param name="definitions"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override IEnumerable<ValidationMessage> GetValidationMessages(Dictionary<string, Dictionary<string, Operation>> paths, RuleContext context)
        {
            var serviceDefinition = (ServiceDefinition)context.Root;

            foreach (var path in paths)
            {
                var lroPutOps = path.Value.Where(val => val.Key.ToLower().Equals("put")).Select(val => val.Value);
                foreach (var op in lroPutOps)
                {
                    if (op.Responses == null)
                    {
                        // if put operation has no response model, let some other validation rule handle the violation
                        continue;
                    }
                    foreach (var resp in op.Responses)
                    {
                        if (resp.Key == "200" || resp.Key == "201")
                        {
                            var modelRef = resp.Value?.Schema?.Reference ?? string.Empty;
                            if (!serviceDefinition.Definitions.ContainsKey(modelRef.StripDefinitionPath()))
                            {
                                var violatingVerb = ValidationUtilities.GetOperationIdVerb(op.OperationId, path);
                                yield return new ValidationMessage(new FileObjectPath(context.File, context.Path.AppendProperty(path.Key).AppendProperty(violatingVerb).AppendProperty("responses").AppendProperty(resp.Key)),
                                    this, op.OperationId, resp.Key);
                            }
                        }
                    }
                }
            }
        }
    }
}
