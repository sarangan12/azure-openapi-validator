﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using AutoRest.Core.Logging;
using OpenAPI.Validator.Model;
using OpenAPI.Validator.Validation.Core;
using System.Text.RegularExpressions;

namespace OpenAPI.Validator.Validation
{
    public abstract class OperationNameValidation : TypedRule<string>
    {
        private static readonly Regex GET_NOUN_VERB_PATTERN = new Regex(@"^(\w+)_(Get|List)", RegexOptions.IgnoreCase);
        private static readonly Regex GET_VERB_PATTERN = new Regex(@"^(Get|List)", RegexOptions.IgnoreCase);
        private static readonly Regex PUT_NOUN_VERB_PATTERN = new Regex(@"^(\w+)_(Create)", RegexOptions.IgnoreCase);
        private static readonly Regex PUT_VERB_PATTERN = new Regex(@"^(Create)", RegexOptions.IgnoreCase);
        private static readonly Regex PATCH_NOUN_VERB_PATTERN = new Regex(@"^(\w+)_(Update)", RegexOptions.IgnoreCase);
        private static readonly Regex PATCH_VERB_PATTERN = new Regex(@"^(Update)", RegexOptions.IgnoreCase);
        private static readonly Regex DELETE_NOUN_VERB_PATTERN = new Regex(@"^(\w+)_(Delete)", RegexOptions.IgnoreCase);
        private static readonly Regex DELETE_VERB_PATTERN = new Regex(@"^(Delete)", RegexOptions.IgnoreCase);

        /// <summary>
        /// No operationId is valid if it is null or empty
        /// </summary>
        private bool IsOperationIdValid(string operationId) => !string.IsNullOrEmpty(operationId);

        /// <summary>
        /// Violation category of the Rule.
        /// </summary>
        public override ValidationCategory ValidationCategory => RulesMetaData.GetValidationCategory(this.Id);

        /// <summary>
        /// The severity of this message (ie, debug/info/warning/error/fatal, etc)
        /// </summary>
        /// <remarks>
        /// This rule corresponds to M1005, M1006, M1007 & M1009.
        /// </remarks>
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
        /// This rule passes if the operation id of HTTP Method confirms to M1005.
        ///   e.g. For Get method User_Get or User_List are valid names.
        /// </summary>
        /// <param name="entity">Operation name to be verified.</param>
        /// <returns><c>true</c> if operation name confirms to GET rule, otherwise <c>false</c>.</returns>
        /// <remarks>
        /// Message will be shown at the path level.
        /// </remarks>
        protected bool IsGetValid(string entity)
        {
            return IsOperationIdValid(entity) && (GET_NOUN_VERB_PATTERN.IsMatch(entity) || GET_VERB_PATTERN.IsMatch(entity));
        }

        /// <summary>
        /// This rule passes if the operation id of HTTP Method confirms to M1006.
        ///   e.g. For PUT method User_Create is valid name.
        /// </summary>
        /// <param name="entity">Operation name to be verified.</param>
        /// <returns><c>true</c> if operation name confimes to PUT rule, otherwise <c>false</c>.</returns>
        /// <remarks>
        /// Message will be shown at the path level.
        /// </remarks>
        protected bool IsPutValid(string entity)
        {
            return IsOperationIdValid(entity) && (PUT_NOUN_VERB_PATTERN.IsMatch(entity) || PUT_VERB_PATTERN.IsMatch(entity));
        }

        /// <summary>
        /// This rule passes if the operation id of HTTP Method confirms to M1007.
        ///   e.g. For PUT method User_Update is valid name.
        /// </summary>
        /// <param name="entity">Operation name to be verified.</param>
        /// <returns><c>true</c> if operation name confimes to PATCH rule, otherwise <c>false</c>.</returns>
        /// <remarks>
        /// Message will be shown at the path level.
        /// </remarks>
        protected bool IsPatchValid(string entity)
        {
            return IsOperationIdValid(entity) && (PATCH_NOUN_VERB_PATTERN.IsMatch(entity) || PATCH_VERB_PATTERN.IsMatch(entity));
        }

        /// <summary>
        /// This rule passes if the operation id of HTTP Method confirms to M1009.
        ///   e.g. For PUT method User_Delete is valid name.
        /// </summary>
        /// <param name="entity">Operation name to be verified.</param>
        /// <returns><c>true</c> if operation name confimes to DELETE rule, otherwise <c>false</c>.</returns>
        /// <remarks>
        /// Message will be shown at the path level.
        /// </remarks>
        protected bool IsDeleteValid(string entity)
        {
            return IsOperationIdValid(entity) && (DELETE_NOUN_VERB_PATTERN.IsMatch(entity) || DELETE_VERB_PATTERN.IsMatch(entity));
        }
    }
}
