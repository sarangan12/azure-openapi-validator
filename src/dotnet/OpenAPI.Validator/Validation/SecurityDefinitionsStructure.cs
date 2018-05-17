﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using AutoRest.Core.Logging;
using OpenAPI.Validator.Properties;
using OpenAPI.Validator.Model;
using OpenAPI.Validator.Validation.Core;
using System;
using System.Linq;
using System.Collections.Generic;

namespace OpenAPI.Validator.Validation
{
    public class SecurityDefinitionsStructure : TypedRule<Dictionary<string, SecurityDefinition>>
    {
        /// <summary>
        /// Id of the Rule.
        /// </summary>
        public override string Id => "R2054";

        /// <summary>
        /// Violation category of the Rule.
        /// </summary>
        public override ValidationCategory ValidationCategory => RulesMetaData.GetValidationCategory(this.Id);

        /// <summary>
        /// What kind of change implementing this rule can cause.
        /// </summary>
        public override ValidationChangesImpact ValidationChangesImpact => RulesMetaData.GetValidationChangesImpact(this.Id);

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
        public override string MessageTemplate => Resources.SecurityDefinitionsStructureValidation;

        /// <summary>
        /// What kind of open api document type this rule should be applied to
        /// </summary>
        public override ServiceDefinitionDocumentType ServiceDefinitionDocumentType => RulesMetaData.GetServiceDefinitionDocumentType(this.Id);

        /// <summary>
        /// Security definitions per json need to be validated
        /// </summary>
        public override ServiceDefinitionDocumentState ValidationRuleMergeState => RulesMetaData.GetServiceDefinitionDocumentState(this.Id);

        /// <summary>
        /// Checks for the presence and existence of the security definiton
        /// </summary>
        /// <param name="securityDefinitions"></param>
        /// <param name="context">The rule context</param>
        /// <returns>List of ValidationMessages</returns>
        public override IEnumerable<ValidationMessage> GetValidationMessages(Dictionary<string, SecurityDefinition> securityDefinitions, RuleContext context)
        {
            if (securityDefinitions.Count != 1)
            {
                yield return new ValidationMessage(new FileObjectPath(context.File, context.Parent.Path), this);
            }
            else if (!securityDefinitions.Any(sdPair =>
                 sdPair.Key.Equals("azure_auth", StringComparison.CurrentCultureIgnoreCase)
                 && IsSecurityDefinitionModelValid(sdPair.Value)))
            {
                yield return new ValidationMessage(new FileObjectPath(context.File, context.Path), this);
            }
        }

        private bool IsSecurityDefinitionModelValid(SecurityDefinition securityDefinition)
        {
            return (
                securityDefinition.SecuritySchemeType == SecuritySchemeType.OAuth2 &&
                securityDefinition.AuthorizationUrl.Equals("https://login.microsoftonline.com/common/oauth2/authorize", StringComparison.CurrentCultureIgnoreCase) &&
                securityDefinition.Flow == OAuthFlow.Implicit &&
                !String.IsNullOrEmpty(securityDefinition.Description) &&
                IsSecurityDefinitionScopesValid(securityDefinition.Scopes)
              );
        }

        private bool IsSecurityDefinitionScopesValid(Dictionary<string, string> scopes)
        {
            if (scopes?.Count != 1)
            {
                return false;
            }

            return scopes.Any(scope =>
                    scope.Key.Equals("user_impersonation", StringComparison.CurrentCultureIgnoreCase)
                && !String.IsNullOrEmpty(scope.Value)
            );
        }
    }
}