export enum NonFhirResourceType {
  BackgroundJob,
  FhirServerSettings,
  ApiCallAudit,
  Transaction
}

export interface FhirServerSettings {
  organizationName: string,
  emailSubject: string,
  inviteTitle: string,
  inviteText: string,
  thankYouPageUrl?: string,
  hasLogo: boolean,
  nonFhirResourceType: NonFhirResourceType,
  id: string
}
