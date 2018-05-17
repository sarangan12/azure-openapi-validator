﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using AutoRest.Core.Logging;
using OpenAPI.Validator.Model;
using OpenAPI.Validator.Model.Utilities;
using OpenAPI.Validator.Properties;
using OpenAPI.Validator.Validation.Core;
using System.Collections.Generic;
using System.Linq;

namespace OpenAPI.Validator.Validation
{
    public class PutRequestResponseScheme : TypedRule<Dictionary<string, Dictionary<string, Operation>>>
    {
        /// <summary>
        /// Id of the Rule.
        /// </summary>
        public override string Id => "R2017";

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
        public override string MessageTemplate => Resources.PutOperationRequestResponseSchemaMessage;

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

        /// Verifies if a PUT operation request and response schemas match
        public override IEnumerable<ValidationMessage> GetValidationMessages(Dictionary<string, Dictionary<string, Operation>> paths, RuleContext context)
        {
            var serviceDefinition = (ServiceDefinition)context.Root;
            var ops = ValidationUtilities.GetOperationsByRequestMethod("put", serviceDefinition);
            foreach (var op in ops)
            {

                // if PUT operation does not have any request parameters, skip, let some other validation rule handle it
                // if no 200 response exists, skip, let some other validation rule handle empty PUT response operations
                if (op.Parameters?.Any() != true || op.Responses?.ContainsKey("200") != true || serviceDefinition.Definitions?.Any() != true)
                {
                    continue;
                }

                // look for the request body schema in the operation parameters section as well as the global parameters section
                string reqBodySchema = null;
                if (op.Parameters.Any(p => p.In == ParameterLocation.Body))
                {
                    reqBodySchema = op.Parameters.First(p => p.In == ParameterLocation.Body).Schema?.Reference?.StripDefinitionPath();
                }
                else
                {
                    var opGlobalParams = op.Parameters.Where(p => serviceDefinition.Parameters.ContainsKey(p.Reference?.StripParameterPath() ?? ""));
                    if (opGlobalParams.Any())
                    {
                        reqBodySchema = opGlobalParams.FirstOrDefault(p => p.In == ParameterLocation.Body)?.Schema?.Reference?.StripDefinitionPath();
                    }
                }

                // if no body parameters were found, skip, let some other validation handle an empty body put operation
                if (string.IsNullOrEmpty(reqBodySchema) || !serviceDefinition.Definitions.ContainsKey(reqBodySchema))
                {
                    continue;
                }
                var respModel = op.Responses["200"]?.Schema?.Reference?.StripDefinitionPath() ?? string.Empty;
                // if the 200 response schema does not match the request body parameter schema, flag violation
                if (respModel != reqBodySchema)
                {
                    var violatingPath = ValidationUtilities.GetOperationIdPath(op.OperationId, paths);
                    var violatingOpVerb = ValidationUtilities.GetOperationIdVerb(op.OperationId, violatingPath);
                    yield return new ValidationMessage(new FileObjectPath(context.File, context.Path.AppendProperty(violatingPath.Key).AppendProperty(violatingOpVerb)), this, op.OperationId, reqBodySchema, respModel);
                }
            }
        }
    }
}
