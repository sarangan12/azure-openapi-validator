﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using AutoRest.Core.Logging;
using OpenAPI.Validator.Validation.Core;
using System;
using System.Collections.Generic;
using OpenAPI.Validator.Properties;

namespace OpenAPI.Validator.Validation
{
    public class GetInOperationName : OperationNameValidation
    {
        /// <summary>
        /// Id of the Rule.
        /// </summary>
        public override string Id => "R1005";

        /// <summary>
        /// The template message for this Rule.
        /// </summary>
        /// <remarks>
        /// This may contain placeholders '{0}' for parameterized messages.
        /// </remarks>
        public override string MessageTemplate => Resources.GetOperationNameNotValid;

        /// <summary>
        /// The severity of this message (ie, debug/info/warning/error/fatal, etc)
        /// </summary>
        public override Category Severity => RulesMetaData.GetSeverity(this.Id);

        /// <summary>
        /// Validates whether GET operation name is named correctly
        /// </summary>
        /// <param name="entity">Operation name to be verified.</param>
        /// <param name="context">Rule context.</param>
        /// <returns>ValidationMessage</returns>
        public override IEnumerable<ValidationMessage> GetValidationMessages(string operationId, RuleContext context)
        {
            string httpVerb = context?.Parent?.Key;
            if (!String.IsNullOrWhiteSpace(httpVerb) && httpVerb.EqualsIgnoreCase("GET") && !IsGetValid(operationId))
            {
                yield return new ValidationMessage(new FileObjectPath(context.File, context.Path), this, operationId);
            }
        }

    }
}
