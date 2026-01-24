<!--
    Usage:
      <DsTable
        :headers="['heading1', 'heading2', 'heading3']"
        :items="[{id: 1, text: 'a'}, {id: 2, text: 'b'}, {id: 3, text: 'c'}]"
        :id-selector="item => item.id"
      >
        <template #default="{item}">
          <div>
            Column 1 - {{item.text}}
          </div>

          <div>
            Column 2 - {{item.text}}
          </div>

          <div>
            Column 3 - {{item.text}}
          </div>
        </template>
        <template #footer>
          <div>I go in the footer</div>
        </template>
      </DsTable>
 -->

<script setup lang="ts" generic="T">
import type { ApiFilter } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/ApiFilter'

type Header = string | {title:string, class?:string, orderBy?: string}

const props =
  withDefaults(
      defineProps<{
      headers: Header[],
      items: T[],
      idSelector: (item: T) => string,
      apiFilter?: ApiFilter
      disableStripe?: boolean;
      disableHover?: boolean;
      customHeaderCss?: string;
      customTableRowCss?: string;
      fullWidth?: boolean;
      borderless?: boolean;
      fixed?: boolean;
      highlightSelector?: (item: T) => boolean;
    }>(),
      {
          fullWidth: true,
          apiFilter: undefined,
          customHeaderCss: '',
          customTableRowCss: '',
          highlightSelector: (_item: T) => false
      }
  )

defineSlots<{
  default(props: { item: T }): any;
  footer(): any;
}>()

const emit = defineEmits<{
  'update:apiFilter': [apiFilter:ApiFilter]
}>()

function onOrderBy(orderBy: string) {
    if (!props.apiFilter) {
        return
    }

    emit(
        'update:apiFilter',
        props.apiFilter.orderBy !== orderBy
            ? { ...props.apiFilter, orderBy, descending: false }
            : { ...props.apiFilter, descending: !props.apiFilter.descending }
    )
}

</script>

<template>
  <div class="relative" :class="{'border border-silver p-2 rounded': !borderless}">
    <table class="text-sm text-left" :class="{'table-fixed': fixed, 'table-auto': !fixed, 'w-full': fullWidth}">
      <thead class="border-none bg-bliss">
        <th
          v-for="(header, i) in headers"
          :key="`table-header-${i}`"
          scope="col"
          :class="customHeaderCss ? customHeaderCss : 'px-6 py-3 first:rounded-tl last:rounded-tr'"
        >
          <template v-if="typeof header === 'string'">
            <DsText size="md" weight="normal">
              {{ header }}
            </DsText>
          </template>
          <template v-else-if="header.orderBy">
            <div class="cursor-pointer flex items-center" :class="header.class" @click="onOrderBy(header.orderBy)">
              <div class="mr-auto">
                <DsText size="md" weight="normal">
                  {{ header.title }}
                </DsText>
              </div>
              <DsIcon
                v-if="apiFilter && apiFilter.orderBy === header.orderBy"
                :name="apiFilter.descending ? 'heroicons:chevron-up' : 'heroicons:chevron-down'"
              />
            </div>
          </template>
          <template v-else>
            <div :class="header.class">
              <DsText size="md" weight="normal">
                {{ header.title }}
              </DsText>
            </div>
          </template>
        </th>
      </thead>
      <tbody class="border border-bliss border-separate border-spacing-0">
        <DsTableRow
          v-for="item in items"
          :key="`row-${idSelector(item)}`"
          :row="idSelector(item) as string"
          :column-count="headers.length"
          :disable-stripe="disableStripe"
          :disable-hover="disableHover"
          :custom-css="customTableRowCss"
          :highlight="highlightSelector ? highlightSelector(item) as boolean : false"
        >
          <slot :item="item" />
        </DsTableRow>
      </tbody>
    </table>
    <div v-if="$slots.footer" class="pt-2">
      <slot name="footer" />
    </div>
  </div>
</template>
