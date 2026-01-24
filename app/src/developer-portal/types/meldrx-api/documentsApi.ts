import type { DocumentReference } from "fhir/r4";
import type { UploadDocumentDto } from "../meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/UploadDocumentDto";

export interface DocumentsApi {
    uploadDocument: (workspaceSlug: string, uploadmodel: UploadDocumentDto, fileContent: ArrayBuffer, fileName: string) => Promise<DocumentReference>;
}