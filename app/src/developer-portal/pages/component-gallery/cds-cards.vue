<template>
  <div class="m-5 space-y-3">
    <div v-for="card in cards.cards" :key="card.uuid">
      <DsCard
        :show-card="true"
        :card="card as Card"
        workspace-slug="{workspaceSlug}"
        @dismiss="(userComment,overrideReason,cardId) => console.log('userComment:',userComment,'overrideReason',overrideReason,'cardId',cardId)"
        @smart-link-selected="(link,cardId) => console.log(link,cardId)"
        @action-selected="(action,cardId) => console.log(action,cardId)"
    />
    </div>
  </div>
</template>

<script setup lang="ts">


import type {Card} from "~/types/cds-hooks/CDSCards";

const cards = {
    "cards": [
        {
            "uuid":"123",
            "summary": "Critical Drug Interaction Alert",
            "indicator": "critical",
            "detail": "The patient is currently prescribed both Warfarin and Aspirin. This combination significantly increases the risk of bleeding. Immediate review and intervention are required.",
            "source": {
                "label": "Pharmacy Department",
                "url": "https://pharmacy.example-hospital.com/drug-interactions",
                "icon": "https://pharmacy.example-hospital.com/icons/pharmacy.png"
            },
            "links": [
                {
                    "label": "View Drug Interaction Details",
                    "url": "https://pharmacy.example-hospital.com/warfarin-aspirin-interaction",
                    "type": "absolute"
                },
                {
                    "label": "Review Patient's Medication List",
                    "url": "https://ehr.example-hospital.com/patient/medication-list",
                    "type": "absolute"
                }
            ],
            "suggestions": [
                {
                    "label": "Discontinue Aspirin",
                    "uuid": "discontinue-aspirin",
                    "actions": [
                        {
                            "type": "create",
                            "description": "Discontinue the Aspirin prescription to avoid dangerous interaction.",
                            "resource": {
                                "resourceType": "MedicationRequest",
                                "status": "stopped",
                                "medicationCodeableConcept": {
                                    "coding": [
                                        {
                                            "system": "http://www.nlm.nih.gov/research/umls/rxnorm",
                                            "code": "1191",
                                            "display": "Aspirin"
                                        }
                                    ],
                                    "text": "Aspirin"
                                },
                                "subject": {
                                    "reference": "Patient/12345"
                                }
                            }
                        }
                    ]
                },
                {
                    "label": "Review Warfarin Therapy",
                    "uuid": "review-warfarin-therapy",
                    "actions": [
                        {
                            "type": "create",
                            "description": "Review the Warfarin therapy to adjust the dose or consider alternative anticoagulation.",
                            "resource": {
                                "resourceType": "MedicationRequest",
                                "status": "active",
                                "medicationCodeableConcept": {
                                    "coding": [
                                        {
                                            "system": "http://www.nlm.nih.gov/research/umls/rxnorm",
                                            "code": "855332",
                                            "display": "Warfarin 5 MG Oral Tablet"
                                        }
                                    ],
                                    "text": "Warfarin"
                                },
                                "subject": {
                                    "reference": "Patient/12345"
                                }
                            }
                        }
                    ]
                }
            ],
            "selectionBehavior": "at-most-one",
            "overrideReasons": [
                {
                    "code": "clinical-judgment",
                    "display": "Clinical Judgment",
                    "reason": "The patient has been on this combination under close monitoring with no adverse effects."
                },
                {
                    "code": "benefit-risk",
                    "display": "Benefit vs. Risk",
                    "reason": "The benefits of combination therapy outweigh the risks for this specific patient."
                }
            ]
        },
        {
            "summary": "Annual Flu Vaccine Reminder",
            "indicator": "info",
            "detail": "Patient has not yet received their annual flu vaccination. It is recommended to administer the flu vaccine during the current visit.",
            "source": {
                "label": "Immunization Department",
                "url": "https://healthcare.example-hospital.com/flu-vaccine-guidelines",
                "icon": "https://healthcare.example-hospital.com/icons/immunization.png"
            },
            "links": [
                {
                    "label": "View Flu Vaccine Recommendations",
                    "url": "https://healthcare.example-hospital.com/flu-vaccine-info",
                    "type": "absolute"
                },
                {
                    "label": "Review Patient's Immunization History",
                    "url": "https://ehr.example-hospital.com/patient/immunization-history",
                    "type": "absolute"
                }
            ],
            "suggestions": [
                {
                    "label": "Order Flu Vaccine",
                    "uuid": "order-flu-vaccine",
                    "actions": [
                        {
                            "type": "create",
                            "description": "Order the annual flu vaccine for the patient.",
                            "resource": {
                                "resourceType": "ImmunizationRecommendation",
                                "recommendation": [
                                    {
                                        "vaccineCode": {
                                            "coding": [
                                                {
                                                    "system": "http://hl7.org/fhir/sid/cvx",
                                                    "code": "140",
                                                    "display": "Influenza, seasonal, injectable"
                                                }
                                            ],
                                            "text": "Flu Vaccine"
                                        },
                                        "forecastStatus": {
                                            "coding": [
                                                {
                                                    "system": "http://terminology.hl7.org/CodeSystem/immunization-recommendation-status",
                                                    "code": "due",
                                                    "display": "Due"
                                                }
                                            ]
                                        }
                                    }
                                ],
                                "patient": {
                                    "reference": "Patient/12345"
                                }
                            }
                        }
                    ]
                }
            ],
            "selectionBehavior": "at-most-one"
        },
        {
            "summary": "High-Risk Medication Alert",
            "indicator": "warning",
            "detail": "Patient is prescribed Warfarin, which is a high-risk medication. Ensure the patient's INR is within the therapeutic range.",
            "source": {
                "label": "Pharmacy Department",
                "url": "https://pharmacy.example-hospital.com/warfarin-guidelines",
                "icon": "https://pharmacy.example-hospital.com/icons/pharmacy.png"
            },
            "links": [
                {
                    "label": "View Warfarin Dosing Guidelines",
                    "url": "https://pharmacy.example-hospital.com/warfarin-dosing",
                    "type": "absolute"
                },
                {
                    "label": "Check Patient's INR History",
                    "url": "https://ehr.example-hospital.com/patient/INR-history",
                    "type": "absolute"
                }
            ],
            "suggestions": [
                {
                    "label": "Order INR Test",
                    "uuid": "order-inr-test",
                    "actions": [
                        {
                            "type": "create",
                            "description": "Order a stat INR test to monitor Warfarin therapy.",
                            "resource": {
                                "resourceType": "ServiceRequest",
                                "code": {
                                    "coding": [
                                        {
                                            "system": "http://loinc.org",
                                            "code": "34714-6",
                                            "display": "Prothrombin time (INR) [Time]"
                                        }
                                    ],
                                    "text": "INR Test"
                                },
                                "subject": {
                                    "reference": "Patient/12345"
                                },
                                "status": "active"
                            }
                        }
                    ]
                },
                {
                    "label": "Adjust Warfarin Dose",
                    "uuid": "adjust-warfarin-dose",
                    "actions": [
                        {
                            "type": "create",
                            "description": "Adjust the Warfarin dose based on the latest INR result.",
                            "resource": {
                                "resourceType": "MedicationRequest",
                                "medicationCodeableConcept": {
                                    "coding": [
                                        {
                                            "system": "http://www.nlm.nih.gov/research/umls/rxnorm",
                                            "code": "855332",
                                            "display": "Warfarin 5 MG Oral Tablet"
                                        }
                                    ],
                                    "text": "Warfarin"
                                },
                                "subject": {
                                    "reference": "Patient/12345"
                                },
                                "dosageInstruction": [
                                    {
                                        "text": "Adjust dose as per INR result."
                                    }
                                ],
                                "status": "active"
                            }
                        }
                    ]
                }
            ],
            "selectionBehavior": "at-most-one",
            "overrideReasons": [
                {
                    "code": "clinical-judgment",
                    "display": "Clinical Judgment",
                    "reason": "The patient's INR was checked recently, and no adjustment is needed."
                },
                {
                    "code": "patient-preference",
                    "display": "Patient Preference",
                    "reason": "Patient prefers to avoid additional blood tests at this time."
                }
            ]
        },
        {
            "summary": "Critical Drug Interaction Alert",
            "indicator": "critical",
            "detail": "Patient is prescribed Warfarin, which has a known severe interaction with the newly prescribed drug, Fluconazole. Immediate action is required to address this potential risk.",
            "source": {
                "label": "Zika Virus Management",
                "url": "https://example.com/cdc-zika-virus-mgmt",
                "topic": {
                    "system": "http://example.org/cds-services/fhir/CodeSystem/topics",
                    "code": "12345",
                    "display": "Mosquito born virus"
                }
            },
            "links": [
                {
                    "type": "smart",
                    "label": "View Interaction Details",
                    "url": "https://druginteraction.example.com/interactions/Warfarin-Fluconazole"
                }
            ],
            "suggestions": [
                {
                    "label": "Review Medication Plan",
                    "uuid": "suggestion-1",
                    "actions": [
                        {
                            "type": "create",
                            "description": "Discuss alternative medications or adjust the dosage to mitigate the interaction risk."
                        }
                    ]
                }
            ],
            "uuid": "a5d64564-da41-4b30-a372-beffad680ebf",
            "serviceUrl": "https://localhost:5000/api/35d00214-c60a-443e-a5a6-32e1747360a0/cds-services/meldrxcdshooks-democards",
            "extension": {}
        },
        {
            "summary": "Eligible for Clinical Trial",
            "indicator": "warning",
            "detail": "Patient is eligible for the ABC123 clinical trial based on current health records. Please review the trial details and consider enrolling the patient.",
            "source": {
                "label": "Clinical Trials Database",
                "icon": "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTPPQ09YvU2WG_QPX4kxNMvMlPAWEODFbNON1aMiIl6IQ&s",
                "url": "https://clinicaltrials.example.com"
            },
            "links": [
                {
                    "type": "smart",
                    "label": "View Trial Details",
                    "url": "https://clinicaltrials.example.com/trials/ABC123"
                }
            ],
            "suggestions": [
                {
                    "label": "Review Trial Eligibility",
                    "uuid": "suggestion-1",
                    "actions": [
                        {
                            "type": "create",
                            "description": "Schedule an appointment with the research coordinator to discuss trial participation."
                        },
                        {
                            "type": "update",
                            "description": "Update the order to record the appropriateness score",
                            "resource": {
                                "resourceType": "MedicationRequest",
                                "id": "order-123",
                                "status": "draft",
                                "intent": "order",
                                "medicationCodeableConcept": {
                                    "coding": [
                                        {
                                            "system": "http://www.nlm.nih.gov/research/umls/rxnorm",
                                            "code": "104894",
                                            "display": "Ondansetron 4 MG Disintegrating Oral Tablet"
                                        }
                                    ],
                                    "text": "Ondansetron 4 MG Disintegrating Oral Tablet"
                                },
                                "subject": {
                                    "reference": "Patient/smart-1288992"
                                },
                                "dosageInstruction": [
                                    {
                                        "timing": {
                                            "repeat": {
                                                "frequency": 1,
                                                "period": 1,
                                                "periodUnit": "d"
                                            }
                                        }
                                    }
                                ]
                            }
                        },
                        {
                            "type": "delete",
                            "description": "Remove the inappropriate order",
                            "resourceId": "ServiceRequest/procedure-request-1"
                        }
                    ]
                }
            ],
            "uuid": "1638a01a-e1af-4df0-b075-6ef38f30daf5",
            "serviceUrl": "https://localhost:5000/api/35d00214-c60a-443e-a5a6-32e1747360a0/cds-services/meldrxcdshooks-democards",
            "extension": {}
        },
        {
            "summary": "Generic drug available",
            "indicator": "info",
            "detail": "Cost: $1274.38. Save $1218.02 with a generic medication.",
            "source": {
                "label": "CMS Public Use Files",
                "url": "https://example1.com"
            },
            "links": [
                {
                    "type": "absolute",
                    "label": "View Trial 1 Details",
                    "url": "https://clinicaltrials.example.com/trials/ABC123"
                },
                {
                    "type": "absolute",
                    "label": "View Trial 2 Details",
                    "url": "https://clinicaltrials.example.com/trials/ABC123"
                },
                {
                    "type": "absolute",
                    "label": "View Trial 3 Details",
                    "url": "https://clinicaltrials.example.com/trials/ABC123"
                }
            ],
            "suggestions": [
                {
                    "label": "Change to Generic",
                    "uuid": "suggestion-1",
                    "isRecommended": "true",
                    "actions": [
                        {
                            "type": "create",
                            "description": "Create action 1"
                        }
                    ]
                },
                {
                    "label": "Find Alternatives",
                    "uuid": "ff19d441-0b0d-4d3a-841d-c7f7f6c1b437",
                    "actions": [
                        {
                            "type": "create",
                            "description": "Create a resource with the newly suggested medication",
                            "resource": {
                                "resourceType": "MedicationRequest",
                                "id": "order-123",
                                "status": "draft",
                                "intent": "order",
                                "medicationCodeableConcept": {
                                    "coding": [
                                        {
                                            "system": "http://www.nlm.nih.gov/research/umls/rxnorm",
                                            "code": "104894",
                                            "display": "Ondansetron 4 MG Disintegrating Oral Tablet"
                                        }
                                    ],
                                    "text": "Ondansetron 4 MG Disintegrating Oral Tablet"
                                },
                                "subject": {
                                    "reference": "Patient/smart-1288992"
                                },
                                "dosageInstruction": [
                                    {
                                        "timing": {
                                            "repeat": {
                                                "frequency": 1,
                                                "period": 1,
                                                "periodUnit": "d"
                                            }
                                        }
                                    }
                                ]
                            }
                        }
                    ]
                }
            ],
            "uuid": "f2028b0e-3b6e-45c5-8cda-23fb427fc8fa",
            "serviceUrl": "https://localhost:5000/api/35d00214-c60a-443e-a5a6-32e1747360a0/cds-services/meldrxcdshooks-democards",
            "overrideReasons": [
                {
                    "display": "Patient Requested Brand Product",
                    "code": "patient-requested-brand",
                    "system": "http://terminology.cds-hooks.org/CodeSystem/OverrideReasons"
                },
                {
                    "display": "Generic Drug Out of Stock or Unavailable",
                    "code": "generic-drug-unavailable",
                    "system": "http://terminology.cds-hooks.org/CodeSystem/OverrideReasons"
                }
            ],
            "extension": {}
        }
    ]
}
</script>
