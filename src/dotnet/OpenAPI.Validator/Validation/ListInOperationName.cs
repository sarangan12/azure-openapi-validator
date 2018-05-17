﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Text.RegularExpressions;
using AutoRest.Core.Logging;
using OpenAPI.Validator.Properties;
using OpenAPI.Validator.Validation.Core;
using OpenAPI.Validator.Model;
using OpenAPI.Validator.Model.Utilities;
using System.Linq;

namespace OpenAPI.Validator.Validation
{
    public class ListInOperationName : TypedRule<Dictionary<string, Dictionary<string, Operation>>>
    {
        private static readonly Regex ListRegex = new Regex(@".+_List([^_]*)$", RegexOptions.IgnoreCase);
        private static readonly string XmsPageableViolation = "x-ms-pageable";
        private static readonly string ArrayTypeViolation = "array";
        /// <summary>
        /// Id of the Rule.
        /// </summary>
        public override string Id => "R1003";

        /// <summary>
        /// Violation category of the Rule.
        /// </summary>
        public override ValidationCategory ValidationCategory => RulesMetaData.GetValidationCategory(this.Id);


        /// <summary>
        /// What kind of open api document type this rule should be applied to
        /// </summary>
        public override ServiceDefinitionDocumentType ServiceDefinitionDocumentType => RulesMetaData.GetServiceDefinitionDocumentType(this.Id);

        /// <summary>
        /// The rule runs on each operation in isolation irrespective of the state and can be run in individual state
        /// </summary>
        public override ServiceDefinitionDocumentState ValidationRuleMergeState => RulesMetaData.GetServiceDefinitionDocumentState(this.Id);

        public override IEnumerable<ValidationMessage> GetValidationMessages(Dictionary<string, Dictionary<string, Operation>> entity, RuleContext context)
        {
            // get all operation objects that are either of get or post type
            var serviceDefinition = (ServiceDefinition)context.Root;
            var listOperations = entity.Values.SelectMany(opDict => opDict.Where(pair => pair.Key.ToLower().Equals("get") || pair.Key.ToLower().Equals("post")));
            foreach (var opPair in listOperations)
            {
                // if the operation id is already of the type _list we can skip this check
                if (opPair.Value.OperationId == null || ListRegex.IsMatch(opPair.Value.OperationId))
                {
                    continue;
                }

                var violatingPath = ValidationUtilities.GetOperationIdPath(opPair.Value.OperationId, entity).Key;
                if (ValidationUtilities.IsXmsPageableResponseOperation(opPair.Value))
                {
                    yield return new ValidationMessage(new FileObjectPath(context.File, context.Path.AppendProperty(violatingPath).AppendProperty(opPair.Key).AppendProperty("operationId")), this, opPair.Value.OperationId, XmsPageableViolation);
                }
                else if (ValidationUtilities.IsArrayTypeResponseOperation(opPair.Value, serviceDefinition))
                {
                    yield return new ValidationMessage(new FileObjectPath(context.File, context.Path.AppendProperty(violatingPath).AppendProperty(opPair.Key).AppendProperty("operationId")), this, opPair.Value.OperationId, ArrayTypeViolation);
                }
            }
        }

        /// <summary>
        /// The template message for this Rule. 
        /// </summary>
        /// <remarks>
        /// This may contain placeholders '{0}' for parameterized messages.
        /// </remarks>
        public override string MessageTemplate => Resources.ListOperationsNamingWarningMessage;

        /// <summary>
        /// The severity of this message (ie, debug/info/warning/error/fatal, etc)
        /// </summary>
        public override Category Severity => RulesMetaData.GetSeverity(this.Id);
    }
}




