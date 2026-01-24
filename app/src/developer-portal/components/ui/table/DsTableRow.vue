<script setup lang="ts">
const props = defineProps<{
  row: string;
  columnCount?: number;
  disableStripe?: boolean;
  disableHover?: boolean;
  highlight?: boolean;
  customCss?: string;
}>()

const slots = defineSlots<{
  default(): any
}>()

function columns() {
    const slotsDefault = slots.default()
    if (slotsDefault.length === 1 && !!slotsDefault[0].children) {
        if (slotsDefault[0].children.length !== props.columnCount) {
            return slotsDefault[0].children[0].children
        }
        return slotsDefault[0].children
    }

    throw new Error('DsTable requires a single default slot.')
}
</script>

<template>
  <tr
    :class="{
      'hover:bg-seafoam': !disableHover,
      'even:bg-bliss': !disableStripe,
      'border-2 border-dark-cyan bg-seafoam': highlight
    }"
  >
    <td
      v-for="(slot, j) in columns()"
      :key="`table-${row}-${j}`"
      :class="customCss ? customCss : 'px-6 py-2'"
    >
      <component :is="slot" />
    </td>
  </tr>
</template>
