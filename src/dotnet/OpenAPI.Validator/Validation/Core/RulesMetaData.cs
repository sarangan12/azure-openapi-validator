// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using OpenAPI.Validator.Validation.Core;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using OpenAPI.Validator.Model;
using AutoRest.Core.Logging;
using System.Collections;
using Newtonsoft.Json;
using System.Net;

public class Metadata
{
    public string Id
    {
        get; set;
    }
    public string ValidationCategory
    {
        get; set;
    }

    public string Severity
    {
        get; set;
    }

    public List<string> ServiceDefinitionDocumentType
    {
        get; set;
    }

    public string ServiceDefinitionDocumentState
    {
        get; set;
    }

    public string ValidationChangesImpact
    {
        get; set;
    }
}

public class RulesMetaData
{
    private static Dictionary<string, Metadata> metaDataDictionary = new Dictionary<string, Metadata>();
    private static readonly string METADATA_URL = "https://raw.githubusercontent.com/sarangan12/dummy/master/metadata_rules.json";

    [MethodImpl(MethodImplOptions.Synchronized)]
    public static void PopulateMetaDataForRules()
    {
        try
        {
            WebClient webClient = new WebClient();
            string metaDataString = webClient.DownloadString(METADATA_URL);
            List<Metadata> metaDataList = JsonConvert.DeserializeObject<List<Metadata>>(metaDataString);

            foreach (Metadata metaData in metaDataList)
            {
                if (!metaDataDictionary.ContainsKey(metaData.Id))
                {
                    metaDataDictionary.Add(metaData.Id, metaData);
                }
            }
        }
        catch (WebException wex)
        {

        }
    }

    public static ValidationCategory GetValidationCategory(string Id)
    {
        Metadata metaData = null;
        metaDataDictionary.TryGetValue(Id, out metaData);
        if (metaData != null)
        {
            switch (metaData.ValidationCategory)
            {
                case "none":
                    return ValidationCategory.None;
                case "armviolation":
                    return ValidationCategory.ARMViolation;
                case "oneapiviolation":
                    return ValidationCategory.OneAPIViolation;
                case "sdkviolation":
                    return ValidationCategory.SDKViolation;
                case "documentation":
                    return ValidationCategory.Documentation;
                default:
                    return ValidationCategory.None;
            }
        }

        return ValidationCategory.None;
    }

    public static Category GetSeverity(string Id)
    {
        Metadata metaData = null;
        metaDataDictionary.TryGetValue(Id, out metaData);
        if (metaData != null)
        {
            switch (metaData.Severity)
            {
                case "info":
                    return Category.Info;
                case "warning":
                    return Category.Warning;
                case "error":
                    return Category.Error;
                default:
                    return Category.Info;
            }
        }

        return Category.Info;
    }

    public static ServiceDefinitionDocumentState GetServiceDefinitionDocumentState(string Id)
    {
        Metadata metaData = null;
        metaDataDictionary.TryGetValue(Id, out metaData);
        if (metaData != null)
        {
            switch (metaData.ServiceDefinitionDocumentState)
            {
                case "individual":
                    return ServiceDefinitionDocumentState.Individual;
                case "composed":
                    return ServiceDefinitionDocumentState.Composed;
                default:
                    return ServiceDefinitionDocumentState.Individual;
            }
        }

        return ServiceDefinitionDocumentState.Individual;

    }

    public static ValidationChangesImpact GetValidationChangesImpact(string Id)
    {
        Metadata metaData = null;
        metaDataDictionary.TryGetValue(Id, out metaData);
        if (metaData != null)
        {
            switch (metaData.ValidationChangesImpact)
            {
                case "none":
                    return ValidationChangesImpact.None;
                case "serviceimpactingchanges":
                    return ValidationChangesImpact.ServiceImpactingChanges;
                case "sdkimpactingchanges":
                    return ValidationChangesImpact.SDKImpactingChanges;
                default:
                    return ValidationChangesImpact.None;
            }
        }

        return ValidationChangesImpact.None;
    }

    public static ServiceDefinitionDocumentType GetServiceDefinitionDocumentType(string Id)
    {
        ServiceDefinitionDocumentType result = ServiceDefinitionDocumentType.Default;

        Metadata metaData = null;
        metaDataDictionary.TryGetValue(Id, out metaData);
        if (metaData != null)
        {
            List<string> serviceDefinitionDocumentTypeList = metaData.ServiceDefinitionDocumentType;
            foreach (string serviceDefinitionDocumentType in serviceDefinitionDocumentTypeList)
            {
                switch (serviceDefinitionDocumentType)
                {
                    case "default":
                        result = result | ServiceDefinitionDocumentType.Default;
                        break;
                    case "arm":
                        result = result | ServiceDefinitionDocumentType.ARM;
                        break;
                    case "dataplane":
                        result = result | ServiceDefinitionDocumentType.DataPlane;
                        break;
                    default:
                        result = result | ServiceDefinitionDocumentType.Default;
                        break;
                }
            }
        }

        return result;
    }
}