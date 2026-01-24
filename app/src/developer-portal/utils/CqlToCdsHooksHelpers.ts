export type ElmMetadata = {
    prefetch: Record<string, string>,
    expressions: Record<string, string[]>,
    booleanExpressions: Record<string, string[]>,
    primaryFile: { id: string, version: string } | null
}

export function parseElmFiles (files: string[]) : ElmMetadata {
    const prefetch: Record<string, string> = {}
    const expressions: Record<string, string[]> = {}
    const booleanExpressions: Record<string, string[]> = {}
    const includesMap: Record<string, boolean> = {} // Track which files are included by others
    const allFiles: Record<string, { id: string, version: string }> = {} // Track all files with id and version

    files.forEach((fileContent) => {
        try {
            const elm = JSON.parse(fileContent)

            if (elm && elm.library && elm.library.identifier) {
                const key = `${elm.library.identifier.id}.${elm.library.identifier.version}`
                allFiles[key] = {
                    id: elm.library.identifier.id,
                    version: elm.library.identifier.version
                }

                if (elm.library.statements && elm.library.statements.def) {
                    expressions[key] = []
                    booleanExpressions[key] = []

                    elm.library.statements.def.forEach((expDef: any) => {
                        if (isReturnableExpression(expDef)) {
                            if (isBooleanExpression(expDef)) {
                                booleanExpressions[key].push(expDef.name);
                            } else {
                                expressions[key].push(expDef.name);
                            }
                            extractPrefetchAndExpressions(expDef.expression, prefetch, elm);
                        }
                    });
                }

                if (elm.library.includes && elm.library.includes.def) {
                    elm.library.includes.def.forEach((includeDef: any) => {
                        const includeKey = `${includeDef.path}.${includeDef.version}`
                        includesMap[includeKey] = true
                    })
                }
            }
        } catch (error) {
            console.error('Error parsing file:', error)
        }
    })

    // Determine the primary file
    let primaryFile: { id: string, version: string } | null = null
    Object.keys(allFiles).forEach((fileKey) => {
        if (!includesMap[fileKey]) {
            primaryFile = allFiles[fileKey]
        }
    })

    return {
        prefetch,
        expressions,
        primaryFile,
        booleanExpressions
    }
}

function isBooleanExpression(expression: any): boolean {
    return expression.resultTypeName === '{urn:hl7-org:elm-types:r1}Boolean';
}

function isReturnableExpression (expression: any): boolean {
    return (expression.type !== 'FunctionDef' && expression.accessLevel === 'Public' || expression.accessLevel === undefined) &&
        (expression.context === 'Patient' || expression.context === undefined)
}

function extractPrefetchAndExpressions (expression: any, prefetch: Record<string, string>, elm: any) {
    if (Array.isArray(expression)) {
        expression.forEach((e) => extractPrefetchAndExpressions(e, prefetch, elm))
    } else if (expression && typeof expression === 'object') {
        if (expression.type === 'Retrieve') {
            const query = buildPrefetchQuery(expression)
            if (query) {
                Object.assign(prefetch, query)
            } else {
                console.error(`Unsupported dataType: ${expression.dataType}`)
            }
        } else if (expression.type === 'ExpressionRef' || expression.type === 'FunctionRef') {
            const referencedExpression = findReferencedExpression(expression, elm)
            if (referencedExpression && isReturnableExpression(referencedExpression)) {
                extractPrefetchAndExpressions(referencedExpression, prefetch, elm)
            }
            if (expression.operand) {
                extractPrefetchAndExpressions(expression.operand, prefetch, elm)
            }
        } else {
            for (const val of Object.values(expression)) {
                extractPrefetchAndExpressions(val, prefetch, elm)
            }
        }
    }
}

function findReferencedExpression (expression: any, elm: any) {
    const libraryName = expression.libraryName || elm.library.identifier.id
    const referencedLibrary = elm.includes?.def.find((lib: any) => lib.localIdentifier === libraryName)
    if (referencedLibrary) {
        return referencedLibrary.library.statements.def.find((exp: any) => exp.name === expression.name)
    }
    return null
}

function buildPrefetchQuery (retrieve: any) {
    const match = /^(\{http:\/\/hl7.org\/fhir\})?([A-Z][a-zA-Z]+)$/.exec(retrieve.dataType)
    if (match) {
        const resource = match[2]

        switch (resource) {
            case 'Patient':
                return { Patient: 'Patient/{{context.patientId}}' }
            case 'Account': // new in STU3
            case 'AllergyIntolerance':
            case 'Appointment':
            case 'AppointmentResponse':
            case 'AuditEvent':
            case 'Basic':
            case 'BodySite':
            case 'BodyStructure': // new in R4
            case 'CarePlan':
            case 'CareTeam': // new in STU3
            case 'ChargeItem': // new in STU3
            case 'Claim':
            case 'ClinicalImpression':
            case 'Communication':
            case 'CommunicationRequest':
            case 'Composition':
            case 'Condition':
            case 'Consent': // new in STU3
            case 'Contract':
            case 'CoverageEligibilityRequest': // new in R4
            case 'CoverageEligibilityResponse': // new in R4
            case 'DetectedIssue':
            case 'Device':
            case 'DeviceRequest': // new in STU3
            case 'DeviceUseRequest':
            case 'DeviceUseStatement':
            case 'DiagnosticOrder':
            case 'DiagnosticReport':
            case 'DocumentManifest':
            case 'DocumentReference':
            case 'Encounter':
            case 'EnrollmentRequest':
            case 'EpisodeOfCare':
            case 'FamilyMemberHistory':
            case 'Flag':
            case 'Goal':
            case 'GuidanceResponse': // new in STU3
            case 'ImagingManifest': // new in STU3
            case 'ImagingObjectSelection':
            case 'ImagingStudy':
            case 'Immunization':
            case 'ImmunizationEvaluation': // new in R4
            case 'ImmunizationRecommendation':
            case 'Invoice': // new in R4
            case 'MeasureReport': // new in STU3
            case 'Media':
            case 'MedicationAdministration':
            case 'MedicationDispense':
            case 'MedicationOrder':
            case 'MedicationRequest': // new in STU3
            case 'MedicationStatement':
            case 'NutritionOrder':
            case 'Observation':
            case 'Order':
            case 'Person':
            case 'Procedure':
            case 'ProcedureRequest':
            case 'Provenance':
            case 'QuestionnaireResponse':
            case 'ReferralRequest':
            case 'RelatedPerson':
            case 'RequestGroup': // new in STU3
            case 'ResearchSubject': // new in STU3
            case 'RiskAssessment':
            case 'Sequence': // new in STU3
            case 'Specimen':
            case 'Substance':
            case 'SupplyRequest':
            case 'SupplyDelivery':
            case 'Task': // new in STU3
            case 'VisionPrescription':
                return { [resource]: `${resource}?patient={{context.patientId}}` }
            case 'AdverseEvent': // new in STU3
                return { AdverseEvent: 'AdverseEvent?subject={{context.patientId}}' }
            case 'DeviceComponent':
                return { DeviceComponent: 'DeviceComponent?source.patient={{context.patientId}}' }
            case 'DeviceMetric':
                return { DeviceMetric: 'DeviceMetric?source.patient={{context.patientId}}' }
            case 'OrderResponse':
                return { OrderResponse: 'OrderResponse?request.patient={{context.patientId}}' }
            case 'ActivityDefinition': // new in STU3
            case 'ChargeItemDefinition': // new in R4
            case 'CodeSystem': // new in STU3
            case 'DeviceDefinition': // new in R4
            case 'EffectEvidenceSynthesis': // new in R4
            case 'EventDefinition': // new in R4
            case 'Evidence': // new in R4
            case 'EvidenceVariable': // new in R4
            case 'HealthcareService': // new in STU3
            case 'InsurancePlan': // new in R4
            case 'Location':
            case 'Library': // new in STU3
            case 'Medication':
            case 'MedicationKnowledge': // new in R4
            case 'MedicinalProduct': // new in R4
            case 'MedicinalProductAuthorization': // new in R4
            case 'MedicinalProductContraindication': // new in R4
            case 'MedicinalProductIndication': // new in R4
            case 'MedicinalProductIngredient': // new in R4
            case 'MedicinalProductInteraction': // new in R4
            case 'MedicinalProductManufactured': // new in R4
            case 'MedicinalProductPackaged': // new in R4
            case 'MedicinalProductPharmaceutical': // new in R4
            case 'MedicinalProductUndesirableEffect': // new in R4
            case 'Measure': // new in STU3
            case 'MolecularSequence': // new in R4
            case 'OrganizationAffiliation': // new in R4
            case 'Organization': // new in R4
            case 'PlanDefinition': // new in STU3
            case 'ResearchDefinition': // new in R4
            case 'ResearchElementDefinition': // new in R4
            case 'ResearchStudy': // new in STU3
            case 'RiskEvidenceSynthesis': // new in R4
            case 'Questionnaire':
            case 'ServiceDefinition': // new in STU3
            case 'SpecimenDefinition': // new in R4
            case 'SubstancePolymer': // new in R4
            case 'SubstanceProtein': // new in R4
            case 'SubstanceReferenceInformation': // new in R4
            case 'SubstanceSpecification': // new in R4
            case 'SubstanceSourceMaterial': // new in R4
            case 'ValueSet':
                return { [resource]: resource }
            default:
                return null
        }
    }
    return null
}
