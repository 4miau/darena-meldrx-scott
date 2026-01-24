import type { BrandingConfigurationDto } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/BrandingConfigurationDto';
import { MeldRxSubscription } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Enums/MeldRxSubscription';

export default function () {
    const brandingConfiguration = useState<BrandingConfigurationDto | undefined>('branding');

    async function loadBranding () {
        const { $api } = useNuxtApp();
        const config = useRuntimeConfig();
        const { subscription } = useSubscription();

        try {
            if(subscription.value.subscriptionType != MeldRxSubscription.Enterprise) {
                return;
            }

            const cdnCustomization = await $api.subscriptions.getCustomization(config.public.storageUrl as string, subscription.value.id as string);
            if (cdnCustomization) {
                brandingConfiguration.value = cdnCustomization;
            }
            return true;

        } catch {
            brandingConfiguration.value = undefined;
            return false;
        }
    }

    return {
        brandingConfiguration: computed(() => brandingConfiguration.value),
        menuBackgroundColor: computed(() => brandingConfiguration.value?.MenuBackgroundColor ?? '#095e86'),
        menuButtonColor: computed(() => brandingConfiguration.value?.MenuButtonColor ?? '#004266'),
        menuMouseOverColor: computed(() => brandingConfiguration.value?.MenuMouseOverColor ?? '#0c79ac'),
        footerColor: computed(() => brandingConfiguration.value?.FooterColor ?? '#02b689'),
        loadBranding
    };
}
