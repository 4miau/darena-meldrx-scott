export interface EhrLaunchContextDto {
    id: string;
    expiresAt: string;
    workspaceSlug: string;
    patientId: string;
    needsPatientBanner: boolean | null;
    smartStyleUrl: string;
    patientIdentifier: string | null;
}
export interface CreateEhrLaunchContextCommand {
    workspaceSlug: string;
    patientId: string;
}
