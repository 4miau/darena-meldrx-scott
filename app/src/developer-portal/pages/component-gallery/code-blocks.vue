<script setup lang="ts">
useHead({ title: 'Component Gallery | MeldRx' });
</script>

<template>
  <DsContainer>
    <div>
      <DsText size="2xl" weight="light">
        Code Blocks
      </DsText>
      <div class="pb-5" />

      <DsText size="xl">
        DsCodeBlock (Bash)
      </DsText>
      <DsCodeblock
        :code="`ls . | grep 'code'`"
        :language="'bash'"
      />
      <DsDivider />

      <DsText size="xl">
        DsCodeBlock (JavaScript)
      </DsText>
      <DsCodeblock
        :code="`function hello() {
  console.log('Hello, world!');
}`"
        :language="'javascript'"
      />
      <DsDivider />
    </div>
  </DsContainer>
</template>
