import type FhirApiProviderDto from '../meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/DynamicRegistration/FhirApiProviderDto';
import type SmartConfigurationResponse from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Fhir/SmartOnFhir/SmartConfigurationResponse';

export interface FhirProvidersApi{
    list: () => Promise<FhirApiProviderDto[]>
    get: (id: string) => Promise<FhirApiProviderDto>
    validate: (url: string) => Promise<SmartConfigurationResponse>
}
