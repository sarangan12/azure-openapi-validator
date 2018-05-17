﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using AutoRest.Core.Logging;
using OpenAPI.Validator.Validation.Core;
using OpenAPI.Validator.Model;
using System.Collections.Generic;

namespace OpenAPI.Validator.Validation
{
    public class AvoidNestedProperties : TypedRule<Schema>
    {
        private static readonly string ClientFlattenExtensionName = "x-ms-client-flatten";


        /// <summary>
        /// Id of the Rule.
        /// </summary>
        public override string Id => "R2001";

        /// <summary>
        /// Violation category of the Rule.
        /// </summary>
        public override ValidationCategory ValidationCategory => RulesMetaData.GetValidationCategory(this.Id);

        /// <summary>
        /// An <paramref name="entity" /> fails this rule if it 
        /// Intentionally ignore the value assigned to the extension
        /// If it has been set to false, we assume the author has
        /// explicitly chosen to avoid flattening
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public override bool IsValid(Schema schema, RuleContext context)
            => context.Key != "properties" || IsClientFlattenUsed(schema.Extensions);

        private static bool IsClientFlattenUsed(Dictionary<string, object> extensions)
            => extensions.ContainsKey(ClientFlattenExtensionName)
            && extensions[ClientFlattenExtensionName] is bool;

        /// <summary>
        ///     The template message for this Rule.
        /// </summary>
        /// <remarks>
        ///     This may contain placeholders '{0}' for parameterized messages.
        /// </remarks>
        public override string MessageTemplate => "Consider using x-ms-client-flatten to provide a better end user experience";

        /// <summary>
        ///     The severity of this message (ie, debug/info/warning/error/fatal, etc)
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
    }
}