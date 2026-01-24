<script setup lang="ts">
import type { EHRs, EhrLogoSize } from '~/types/ehrs';
import EpicLogo from '~/components/ehrs/logos/EpicLogo.vue';
import CernerLogo from '~/components/ehrs/logos/CernerLogo.vue';
import NextGenLogo from '~/components/ehrs/logos/NextGenLogo.vue';
import AthenaHealthLogo from '~/components/ehrs/logos/AthenaHealthLogo.vue';
import VeradigmLogo from '~/components/ehrs/logos/VeradigmLogo.vue';
import OtherEhrLogo from '~/components/ehrs/logos/OtherEhrLogo.vue';

const props = defineProps<{
    ehrType?: EHRs,
    size?: EhrLogoSize
}>();

const component = computed(() => {
    if (props.ehrType === 'Epic') { return EpicLogo }
    if (props.ehrType === 'Cerner') { return CernerLogo }
    if (props.ehrType === 'AthenaHealth') { return AthenaHealthLogo }
    if (props.ehrType === 'NextGen') { return NextGenLogo }
    if (props.ehrType === 'Veradigm') { return VeradigmLogo }
    else { return OtherEhrLogo }
})

</script>

<template>
  <component :is="component" :size="size" />
</template>
