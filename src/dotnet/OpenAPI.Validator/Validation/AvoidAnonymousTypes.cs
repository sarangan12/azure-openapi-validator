﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using AutoRest.Core.Logging;
using OpenAPI.Validator.Properties;
using OpenAPI.Validator.Validation.Core;
using OpenAPI.Validator.Model;

namespace OpenAPI.Validator.Validation
{
    public class AvoidAnonymousTypes : TypedRule<SwaggerObject>
    {
        /// <summary>
        /// Id of the Rule.
        /// </summary>
        public override string Id => "R2026";

        /// <summary>
        /// Violation category of the Rule.
        /// </summary>
        public override ValidationCategory ValidationCategory => RulesMetaData.GetValidationCategory(this.Id);

        /// <summary>
        /// The severity of this message (ie, debug/info/warning/error/fatal, etc)
        /// </summary>
        public override Category Severity => RulesMetaData.GetSeverity(this.Id);

        /// <summary>
        /// The template message for this Rule. 
        /// </summary>
        /// <remarks>
        /// This may contain placeholders '{0}' for parameterized messages.
        /// </remarks>
        public override string MessageTemplate => Resources.AnonymousTypesDiscouraged;

        /// <summary>
        /// What kind of open api document type this rule should be applied to
        /// </summary>
        public override ServiceDefinitionDocumentType ServiceDefinitionDocumentType => RulesMetaData.GetServiceDefinitionDocumentType(this.Id);

        /// <summary>
        /// The rule runs on each operation's parameters in isolation irrespective of the state and can be run in individual state
        /// </summary>
        public override ServiceDefinitionDocumentState ValidationRuleMergeState => RulesMetaData.GetServiceDefinitionDocumentState(this.Id);

        /// <summary>
        /// An <paramref name="entity"/> fails this rule if it doesn't have a reference (meaning it's defined inline)
        /// </summary>
        /// <param name="entity">The entity to validate</param>
        /// <returns></returns>
        public override bool IsValid(SwaggerObject entity) => (entity?.GetType() == typeof(Schema) &&
            ((((Schema)entity).Properties == null) || ((Schema)entity).Properties?.Count == 0));

    }
}