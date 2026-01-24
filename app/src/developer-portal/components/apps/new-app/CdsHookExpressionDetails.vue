<script setup lang="ts">
import type { ElmMetadata } from '~/utils/CqlToCdsHooksHelpers';

const props = defineProps<{
  metadata: ElmMetadata
}>();

const filteredExpressions = computed(() => {
    return Object.fromEntries(
        Object.entries(props.metadata.expressions)
            .filter(
                ([key, expressionList]) => key.includes(props.metadata.primaryFile!.id) && expressionList.length > 0
            )
    );
});

const booleanExpressions = computed(() => {
    return Object.fromEntries(
        Object.entries(props.metadata.booleanExpressions)
            .filter(
                ([key, expressionList]) => key.includes(props.metadata.primaryFile!.id) && expressionList.length > 0
            )
    );
});

</script>

<template>

  <div class="grid md:grid-cols-12 gap-8">
    <div class="col-span-4">
      <DsLabeledText
        label="Available Expressions"
        text="The expressions that can be used to display alerts in cards."
      />
    </div>

    <div class="col-span-8 space-y-5">
      <DsLabeledText label="Available Expressions">
        <ul>
          <li v-for="(expressionList, index) in filteredExpressions" :key="`exp-${index}`">
            <div>
              <ul>
                <li v-for="expression in expressionList" :key="expression">
                  {{ expression }}
                </li>
              </ul>
            </div>
          </li>
        </ul>
      </DsLabeledText>
    </div>
  </div>

  <DsDivider class="my-8" />

  <div class="grid md:grid-cols-12 gap-8">
    <div class="col-span-4">
      <DsLabeledText
        label="Conditional Expressions"
        text="The expressions that can be used to trigger cards."
      />
    </div>

    <div class="col-span-8 space-y-5">
      <DsLabeledText label="Conditional Expressions">
        <ul>
          <li v-for="(expressionList, index) in booleanExpressions" :key="`exp-bool-${index}`">
            <div>
              <ul>
                <li v-for="expression in expressionList" :key="expression">
                  {{ expression }}
                </li>
              </ul>
            </div>
          </li>
        </ul>
      </DsLabeledText>
    </div>
  </div>
</template>
