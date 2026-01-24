import type FhirApiProviderDto from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/DynamicRegistration/FhirApiProviderDto';

export default class FhirApiProviderUtils {
    public static getDisplayString(fhirProvider: FhirApiProviderDto): string {
        return `${fhirProvider.productName} by ${fhirProvider.organizationName} (${fhirProvider.version})`;
    }

    public static getDisplayStringFromId(fhirProviders: FhirApiProviderDto[], fhirApiProviderMeldRxIdentifier: string): string {
        const fhirApiProvider = fhirProviders.find(x => x.meldRxIdentifier === fhirApiProviderMeldRxIdentifier);
        if (!fhirApiProvider) { return ''; }
        return FhirApiProviderUtils.getDisplayString(fhirApiProvider);
    }
}
