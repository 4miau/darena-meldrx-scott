import type FhirApiProviderDto from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/DynamicRegistration/FhirApiProviderDto';

export default function(): Ref<FhirApiProviderDto[]> {
    const fhirApiProviders = ref<FhirApiProviderDto[]>([])
    const { $api } = useNuxtApp()

    $api.fhirProviders
        .list()
        .then((providers) => { fhirApiProviders.value = providers })

    return fhirApiProviders
}
