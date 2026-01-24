<script setup lang="ts">
useHead({ title: 'Component Gallery | MeldRx' });
const form = ref({
    checked: false
})
</script>

<template>
  <DsContainer>
    <div>
      <DsText size="2xl" weight="light">
        CheckBoxes
      </DsText>
      <div class="pb-5" />

      <DsText size="xl">
        DsCheckbox
      </DsText>

      <div class="pb-5" />

      <DsCheckbox
        v-model="form.checked"
        label="checkbox label"
      />

      <div class="py-2">
        Checkbox selected? - {{ form.checked }}
      </div>
    </div>
  </DsContainer>
</template>
