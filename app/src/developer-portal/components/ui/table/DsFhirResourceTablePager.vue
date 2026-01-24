<script setup lang="ts">
import type { Bundle } from 'fhir/r4'
import { Colors } from '~/types/ui/colors'
import type { PagedResultInfo } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Models/PagedResult'

// Define props to accept a Bundle
const props = defineProps<{
  bundle: Bundle;
}>()

// Emit event for page navigation
const emit = defineEmits<{
  goToPage: [page: number]
}>()

const pagedResultInfo = computed<PagedResultInfo>(() => createPagedResultInfo(props.bundle))

const pagedResultRange = computed(() => {
    if (pagedResultInfo.value.total === 0) {
        return {
            from: 0,
            to: 0
        }
    }
    const pageOffset = pagedResultInfo.value.currentPage - 1;
    const lastPage = pagedResultInfo.value.currentPage === pagedResultInfo.value.totalPages
    return {
        from: 1 + (pageOffset * pagedResultInfo.value.pageSize),
        to: lastPage ? pagedResultInfo.value.total : (pagedResultInfo.value.pageSize + (pageOffset * pagedResultInfo.value.pageSize))
    }
})

type PageButton = {
  text?: string,
  icon?: string,
  action?: () => void,
  value: number
}
function createPagedResultInfo(bundle: Bundle): PagedResultInfo {
    const total = bundle.total ?? 0;
    const pageSize = 10;

    let currentPage = 1;
    let hasNextPage = false;

    const nextLink = bundle.link?.find(link => link.relation === 'next');
    const selfLink = bundle.link?.find(link => link.relation === 'self');

    if (selfLink) {
        const result = /page\/(?<page>\d+)\?/.exec(selfLink.url)
        if (result && result.groups) {
            currentPage = parseInt(result.groups.page)
        }
    }

    if (nextLink) {
        hasNextPage = true;
    }

    const totalPages = Math.ceil(total / pageSize);

    return {
        total,
        totalPages,
        hasNextPage,
        currentPage,
        pageSize
    };
}
function pageButtonFactory(page: number): PageButton {
    return page === pagedResultInfo.value.currentPage
        ? {
            text: page.toString(),
            value: page
        }
        : {
            text: page.toString(),
            value: page,
            action() {
                emit('goToPage', page)
            }
        }
}

const buttons = computed<PageButton[]>(() => {
    const currentPage = pagedResultInfo.value.currentPage;
    const result: PageButton[] = [
        {
            icon: 'heroicons:chevron-left',
            value: 0,
            action: currentPage === 1
                ? undefined
                : () => emit('goToPage', currentPage - 1)
        }
    ]

    // can display all page buttons
    if (pagedResultInfo.value.totalPages <= 7) {
        for (let i = 1; i <= pagedResultInfo.value.totalPages; i++) {
            result.push(pageButtonFactory(i))
        }
    // we are near start or end
    } else if (currentPage <= 2 || currentPage >= (pagedResultInfo.value.totalPages - 1)) {
        for (let i = 1; i <= 3; i++) {
            result.push(pageButtonFactory(i))
        }

        result.push({ text: '...', value: -2 })

        for (let i = pagedResultInfo.value.totalPages - 2; i <= pagedResultInfo.value.totalPages; i++) {
            result.push(pageButtonFactory(i))
        }
    // show nearest pages
    } else {
        if (currentPage > 2) {
            result.push(pageButtonFactory(1))
        }

        if (currentPage === 4) {
            result.push(pageButtonFactory(2))
        } else if (currentPage > 4) {
            result.push({ text: '...', value: -2 })
        }

        for (let i = currentPage - 1; i <= currentPage + 1; i++) {
            result.push(pageButtonFactory(i))
        }

        if (currentPage === pagedResultInfo.value.totalPages - 3) {
            result.push(pageButtonFactory(pagedResultInfo.value.totalPages - 1))
        } else if (currentPage < pagedResultInfo.value.totalPages - 2) {
            result.push({ text: '...', value: -2 })
        }

        if (currentPage < pagedResultInfo.value.totalPages - 1) {
            result.push(pageButtonFactory(pagedResultInfo.value.totalPages))
        }
    }

    result.push({
        icon: 'heroicons:chevron-right',
        value: -1,
        action: currentPage === pagedResultInfo.value.totalPages
            ? undefined
            : () => emit('goToPage', currentPage + 1)
    })
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
          padding='first:pl-3 last:pr-3'
          rounded
          size="sm"
          @click="btn.action"
        >
          <DsIcon v-if="btn.icon" :name='btn.icon' />
          {{ btn.text }}
        </DsButton>
      </DsButtonGroup>
    </div>
  </div>
</template>

<style scoped>
/* Add your styles here */
</style>
