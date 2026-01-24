<script setup lang="ts">
import { Colors } from '~/types/ui/colors'
import type { PagedResultInfo } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Models/PagedResult'

const props = defineProps<{
  pagedResultInfo: PagedResultInfo;
}>()

const emit = defineEmits<{
  goToPage: [page: number]
}>()

const pagedResultRange = computed(() => {
    if (props.pagedResultInfo.total === 0) {
        return {
            from: 0,
            to: 0
        }
    }
    const pageOffset = props.pagedResultInfo.currentPage - 1;
    const lastPage = props.pagedResultInfo.currentPage === props.pagedResultInfo.totalPages
    return {
        from: 1 + (pageOffset * props.pagedResultInfo.pageSize),
        to: lastPage ? props.pagedResultInfo.total : (props.pagedResultInfo.pageSize + (pageOffset * props.pagedResultInfo.pageSize))
    }
})

type PageButton = {
  text?: string,
  icon?: string,
  action?: () => void,
  value: number
}

function pageButtonFactory(page: number, text?: string) : PageButton {
    return page === props.pagedResultInfo.currentPage
        ? {
            text: text ?? page.toString(),
            value: page
        }
        : {
            text: text ?? page.toString(),
            value: page,
            action() {
                emit('goToPage', page)
            }
        }
}

const buttons = computed<PageButton[]>(() => {
    const currentPage = props.pagedResultInfo.currentPage;
    const result : PageButton[] = [
        {
            icon: 'heroicons:chevron-left',
            value: 0,
            action: currentPage === 1
                ? undefined
                : () => emit('goToPage', currentPage - 1)
        }
    ]

    // can display all page buttons
    if (props.pagedResultInfo.totalPages <= 7) {
        for (let i = 1; i <= props.pagedResultInfo.totalPages; i++) {
            result.push(pageButtonFactory(i))
        }
    // we are near start or end
    } else if (currentPage <= 2 ||
        currentPage >= (props.pagedResultInfo.totalPages - 1)) {
        for (let i = 1; i <= 3; i++) {
            result.push(pageButtonFactory(i))
        }

        result.push(pageButtonFactory(Math.round(props.pagedResultInfo.totalPages / 2), '...'))

        for (let i = props.pagedResultInfo.totalPages - 2; i <= props.pagedResultInfo.totalPages; i++) {
            result.push(pageButtonFactory(i))
        }
    // show nearest pages
    } else {
        if (currentPage > 2) {
            result.push(pageButtonFactory(1))
        }

        if (currentPage === 4) {
            result.push(pageButtonFactory(2))
        }
        else if (currentPage > 4) {
            result.push(pageButtonFactory(Math.round(currentPage / 2), '...'))
        }

        if (currentPage === props.pagedResultInfo.totalPages - 2){
            result.push(pageButtonFactory(currentPage - 2))
        }

        for (let i = currentPage - 1; i <= currentPage + 1; i++) {
            result.push(pageButtonFactory(i))
        }

        if(currentPage === 3) {
            result.push(pageButtonFactory(currentPage + 2))
        }

        if (currentPage === props.pagedResultInfo.totalPages - 3) {
            result.push(pageButtonFactory(props.pagedResultInfo.totalPages - 1))
        }
        else if (currentPage < props.pagedResultInfo.totalPages - 2) {
            result.push(
                pageButtonFactory(
                    Math.round((props.pagedResultInfo.totalPages + currentPage) / 2),
                    '...'
                )
            )
        }

        if (currentPage < props.pagedResultInfo.totalPages - 1) {
            result.push(pageButtonFactory(props.pagedResultInfo.totalPages))
        }
    }

    result.push(
        {
            icon: 'heroicons:chevron-right',
            value: -1,
            action: currentPage === props.pagedResultInfo.totalPages
                ? undefined
                : () => emit('goToPage', currentPage + 1)
        }
    )
    return result;
})
</script>

<template>
  <div class="flex items-center">
    <DsText class="mr-auto" size="md" weight="light">
      Showing <strong>{{ pagedResultRange.from }}</strong> to <strong>{{ pagedResultRange.to }}</strong> of <strong>{{ pagedResultInfo.total }}</strong> results
    </DsText>
    <div v-if="pagedResultInfo.total > 0">
      <DsButtonGroup
        :model-value="pagedResultInfo.currentPage"
        :active-color="Colors.primary"
      >
        <DsButton
          v-for="btn in buttons"
          :key="`page-btn-${btn.value}`"
          :text-color='Colors.gray'
          :value="btn.value"
          rounded
          padding='first:pl-3 last:pr-3'
          size="sm"
          @click="btn.action"
        >
          <DsIcon v-if="btn.icon" :name="btn.icon" />
          {{ btn.text }}
        </DsButton>
      </DsButtonGroup>
    </div>
  </div>
</template>

<style scoped>

</style>
