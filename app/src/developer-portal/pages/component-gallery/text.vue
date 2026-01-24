<script setup lang="ts">
useHead({ title: 'Component Gallery | MeldRx' });
</script>

<template>
  <DsContainer>
    <div>
      <DsText size="2xl" weight="light">
        Text
      </DsText>
      <div class="pb-5" />

      <DsText size="xl">
        DsText; Different Sizes (default weight)
      </DsText>

      <DsText size="xs" class="block">
        xs
      </DsText>
      <DsText size="sm" class="block">
        sm
      </DsText>
      <DsText size="md" class="block">
        md
      </DsText>
      <DsText size="lg" class="block">
        lg
      </DsText>
      <DsText size="xl" class="block">
        xl
      </DsText>
      <DsText size="2xl" class="block">
        2xl
      </DsText>
      <DsDivider />

      <DsText size="xl">
        DsText; Different Weights
      </DsText>

      <DsText size="lg" weight="light" class="block">
        light
      </DsText>
      <DsText size="lg" weight="normal" class="block">
        normal
      </DsText>
      <DsText size="lg" weight="bold" class="block">
        bold
      </DsText>
      <DsDivider />

      <DsText size="xl">
        DsTextWithCopyButton
      </DsText>

      <DsTextWithCopyButton size="sm" text="Copy Me" />
      <DsTextWithCopyButton size="sm" text="Something different will be copied" text-to-copy="Hello" />
    </div>
  </DsContainer>
</template>
