import type { DocumentReference } from 'fhir/r4';
import DateTime from '~/utils/DateTime'
import type { DocumentType } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/DocumentType'


export class DocumentReferenceExtensions {

    static getDisplayName(reference: DocumentReference): string {
        const builder: string[] = [];

        if (!reference.description) {
            if (reference.type) {
                if (reference.type.coding && reference.type.coding.length > 0) {
                    if (reference.type.coding[0].display) {
                        builder.push(reference.type.coding[0].display);
                    }
                }

                if (builder.length === 0 && reference.type.text) {
                    builder.push(reference.type.text);
                }
            }

            if (builder.length === 0) {
                builder.push(reference.id!);
            }
        } else {
            builder.push(reference.description);
        }

        return builder.join('');
    }

    static isXmlContentType(reference: DocumentReference): boolean {
        const contentType = reference.content && reference.content.length > 0 && reference.content[0].attachment.contentType;
        return contentType === 'text/xml' || contentType === 'application/xml';
    }

    static isImageContentType(reference: DocumentReference): boolean {
        const contentType = reference.content && reference.content.length > 0 && reference.content[0].attachment.contentType;
        return contentType === 'image/png' || contentType === 'image/gif' || contentType === 'image/jpeg' || contentType === 'image/bmp' || contentType === 'image/tiff';
    }

    static isVideoContentType(reference: DocumentReference): boolean {
        const contentType = reference.content && reference.content.length > 0 && reference.content[0].attachment.contentType
        return contentType === 'video/mp4' || contentType === 'video/mpeg' || contentType === 'video/x-msvideo'
    }

    static isDocumentContentType(reference: DocumentReference): boolean {
        const contentType = reference.content && reference.content.length > 0 && reference.content[0].attachment.contentType
        return contentType === 'application/msword' || contentType === 'application/vnd.openxmlformats-officedocument.wordprocessingml.document'
    }

    static isTextContentType(reference: DocumentReference): boolean {
        const contentType = reference.content && reference.content.length > 0 && reference.content[0].attachment.contentType;
        return contentType === 'text/plain' || (typeof contentType === 'string' && contentType.includes('text/plain')) || (typeof contentType === 'string' && contentType.includes('rtf'))
    }

    static isHtmlContentType(reference: DocumentReference): boolean {
        const contentType = reference.content && reference.content.length > 0 && reference.content[0].attachment.contentType;
        return contentType === 'text/html';
    }

    static isPdfContentType(reference: DocumentReference): boolean {
        const contentType = reference.content && reference.content.length > 0 && reference.content[0].attachment.contentType;
        return contentType === 'application/pdf';
    }

    static getAttachmentDocumentType(reference: DocumentReference): DocumentType {
        if (this.isXmlContentType(reference)) { return 'xml'; }
        if (this.isTextContentType(reference)) { return 'text'; }
        if (this.isPdfContentType(reference)) { return 'pdf'; }
        if (this.isImageContentType(reference)) { return 'image'; }
        if (this.isHtmlContentType(reference)) { return 'html'; }
        if (this.isVideoContentType(reference)) { return 'video' }
        if (this.isDocumentContentType(reference)) { return 'document' }
        return 'other';
    }

    static getAttachmentCreationDate(reference: DocumentReference): string {
        if (!reference.date) {
            return 'Unknown';
        }

        return DateTime.toDefaultDateString(reference.date)
    }
}
