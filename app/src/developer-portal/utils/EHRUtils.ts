import type { EHRs } from '~/types/ehrs';
import EHR_METADATA from '~/types/EhrMetadata';
import type { IEhrMetadata, ISandboxServer } from '~/types/EhrMetadata';
import type { SoFAppUserType } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/DynamicRegistration/SoFAppUserType';

export default class EHRUtils {
    // Returns the EHR that is associated with the given FHIR Provider ID, if possible.
    // Otherwise, returns "Other"
    public static getEhrFromFhirApiProviderMeldRxId(fhirApiProviderMeldRxIdentifier?: string): EHRs {
        if (!fhirApiProviderMeldRxIdentifier) { return 'Other'; }
        if (fhirApiProviderMeldRxIdentifier === '') { return 'Other'; }

        // Search EHR_METADATA...
        const matchingEhrMetadata = EHR_METADATA.find((ehrMetadata) => {
            return ehrMetadata.associatedChplIds.includes(fhirApiProviderMeldRxIdentifier);
        });

        if (matchingEhrMetadata) { return matchingEhrMetadata.ehr; }
        return 'Other';
    }

    // Given an EHR, returns the CHPL ID that we want to use for this EHR.
    // The reason for this is that we are trying to simplify some of the higher-priority EHRs by
    //     just selecting a FHIR Provider for the user.
    public static getFhirApiProviderMeldRxIdForEhr(ehr?: EHRs): string {
        if (!ehr) { return ''; }

        // Search EHR_METADATA...
        const ehrMetadata = EHR_METADATA.find((ehrMetadata) => { return ehrMetadata.ehr === ehr; });
        if (ehrMetadata && ehrMetadata.primaryChplId) { return ehrMetadata.primaryChplId; }

        return '';
    }

    // Given a FHIR Provider ID, returns the metadata for that EHR.
    public static getMetadataForFhirProvider(fhirapiProviderMeldRxIdentifier?: string) : IEhrMetadata | undefined {
        if (!fhirapiProviderMeldRxIdentifier) { return undefined; }

        // Search EHR_METADATA...
        const ehrMetadata = EHR_METADATA.find((ehrMetadata) => {
            return ehrMetadata.associatedChplIds.includes(fhirapiProviderMeldRxIdentifier);
        });

        return ehrMetadata;
    }

    // Get all Sandbox FHIR URLs for the given FHIR Provider. Optionally filter by user type.
    public static getSandboxFhirUrls(fhirApiProviderMeldRxIdentifier?: string, userType? : SoFAppUserType) : ISandboxServer[] {
        if (!fhirApiProviderMeldRxIdentifier) { return []; }

        // Get metadata...
        const ehrMetadata = EHRUtils.getMetadataForFhirProvider(fhirApiProviderMeldRxIdentifier);
        if (!ehrMetadata) { return []; }

        // Filter...
        if (ehrMetadata && ehrMetadata.primaryChplId) {
            return ehrMetadata.sandboxServers
                .filter(x => !userType || x.userTypes.includes(userType));
        }

        return [];
    }
}
