﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using AutoRest.Core.Logging;
using OpenAPI.Validator.Model;
using OpenAPI.Validator.Properties;
using System.Collections.Generic;
using OpenAPI.Validator.Validation.Core;
using OpenAPI.Validator.Model.Utilities;
using Newtonsoft.Json.Linq;

namespace OpenAPI.Validator.Validation
{
    public class XmsPageableListByRGAndSubscriptions : TypedRule<Dictionary<string, Schema>>
    {
        private static readonly string NextLinkName = "nextLinkName";

        /// <summary>
        /// Id of the Rule.
        /// </summary>
        public override string Id => "R3060";

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
        public override string MessageTemplate => Resources.XMSPagableListByRGAndSubscriptionsMismatch;

        /// <summary>
        /// What kind of open api document type this rule should be applied to
        /// </summary>
        public override ServiceDefinitionDocumentType ServiceDefinitionDocumentType => RulesMetaData.GetServiceDefinitionDocumentType(this.Id);

        /// <summary>
        /// The state of the document on which to run the validation rule
        /// </summary>
        public override ServiceDefinitionDocumentState ValidationRuleMergeState => RulesMetaData.GetServiceDefinitionDocumentState(this.Id);

        /// <summary>
        /// The severity of this message (ie, debug/info/warning/error/fatal, etc)
        /// </summary>
        public override Category Severity => RulesMetaData.GetSeverity(this.Id);

        private bool IsEqual(JObject obj1, JObject obj2)
        {
            if (obj1.Count != obj2.Count && obj1.Count != 1)
            {
                return false;
            }

            JToken value1 = obj1.GetValue(NextLinkName);
            JToken value2 = obj2.GetValue(NextLinkName);

            if (!JToken.DeepEquals(value1, value2))
            {
                return false;
            }

            return true;
        }

        public override IEnumerable<ValidationMessage> GetValidationMessages(Dictionary<string, Schema> definitions, RuleContext context)
        {
            IEnumerable<string> parentTrackedResourceModels = context.ParentTrackedResourceModels;

            foreach (string resourceModel in parentTrackedResourceModels)
            {
                Operation listByResourceGroupOperation = ValidationUtilities.GetListByResourceGroupOperation(resourceModel, definitions, context.Root);
                Operation listBySubscriptionOperation = ValidationUtilities.GetListBySubscriptionOperation(resourceModel, definitions, context.Root);

                if (listByResourceGroupOperation != null && listBySubscriptionOperation != null)
                {
                    JObject rgXMSPageableExtension = (JObject)listByResourceGroupOperation.Extensions[ValidationUtilities.XmsPageable];
                    JObject subXMSPageableExtension = (JObject)listBySubscriptionOperation.Extensions[ValidationUtilities.XmsPageable];
                    if (!IsEqual(rgXMSPageableExtension, subXMSPageableExtension))
                    {
                        yield return new ValidationMessage(new FileObjectPath(context.File, context.Path.AppendProperty(resourceModel)), this, resourceModel);
                    }
                }
            }
        }
    }
}
