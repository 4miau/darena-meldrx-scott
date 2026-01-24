export type VirtualWorkspaceOperationPatientPrefetchPayload = {
  snapshot: VirtualWorkspaceSnapshot.PatientPrefetch;
  name?: string;
  patientId: string;
  hook: string;
}

export type VirtualWorkspaceOperationPatientEverythingPayload = {
  snapshot: VirtualWorkspaceSnapshot.PatientEverything;
  name?: string;
  patientId: string;
}

export type VirtualWorkspaceOperationPayload =
  VirtualWorkspaceOperationPatientPrefetchPayload
  | VirtualWorkspaceOperationPatientEverythingPayload

export enum VirtualWorkspaceSnapshot {
  PatientPrefetch = 'patient-prefetch',
  PatientEverything = 'patient-everything',
}
