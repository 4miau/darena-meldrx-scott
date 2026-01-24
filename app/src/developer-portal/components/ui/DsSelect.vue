<script setup lang="ts" generic="T, TValue extends (T | T[])">
import type { IDsSelectItem } from '~/types/ui/DsSelect'
import { Colors } from '~/types/ui/colors'

const props = defineProps<{
	modelValue?: TValue
	items: IDsSelectItem<T>[]
	label?: string
	required?: boolean
	placeholder?: string
	searchable?: boolean
	searchOptions?: (query: string) => IDsSelectItem<string>[]
	searchPlaceholder?: string
	multiple?: boolean
	rules?: ValidationRule<TValue>[]
	disabled?: boolean
}>()

const emit = defineEmits<{
	'update:modelValue': [TValue]
}>()

const state = reactive<{
	isOpen: boolean
	searchFilter: string
    selectedValues: T[]
    highlightedValues: T[]
}>({
    isOpen: false,
    searchFilter: '',
    selectedValues: [props.modelValue as T],
    highlightedValues: []
})

const selectRef = ref<HTMLElement>()
const { dirty, validationResult } = useValidation<TValue>(
    () => props.modelValue as TValue,
    () => props.rules,
    () => props.required ?? false
)

async function toggleMenu() {
    if (state.isOpen) {
        state.isOpen = false
        return
    }

    state.isOpen = true

    await nextTick()
}

function onSelect(value: T) {
    if (props.multiple && Array.isArray(props.modelValue)) {
        if (props.modelValue.includes(value)) {
            return emit('update:modelValue', props.modelValue.filter((x) => x !== value) as TValue)
        }
        return emit('update:modelValue', [...props.modelValue, value] as TValue)
    }

    emit('update:modelValue', value as TValue)
    toggleMenu()
}

useClickOutsideElement(selectRef, () => {
    const selectedText = window.getSelection()?.toString()

    if (!state.isOpen) return
    if (!selectedText) {
        state.isOpen = false
        return
    }

    if (props.items.some(prop => selectedText?.includes(prop.title)) || state.searchFilter.includes(selectedText)) {
        state.isOpen = true
    } else {
        state.isOpen = false
    }
})
</script>

<template>
	<DsLabeledInput :label="label" :required="required">
		<div ref="selectRef" class="relative">
			<button :id="props.label?.replaceAll(' ', '-').toLowerCase() + '-select'" ref="buttonRef" :disabled="props.disabled" class="flex justify-between bg-white px-3 py-2 text-sm text-onyx w-full border border-silver max-h-[42px]" @click="toggleMenu">
				<slot name="button-text">
					<DsText v-if="Array.isArray(modelValue)" size="sm" class="text-nowrap text-ellipsis overflow-hidden">
						{{
							modelValue.length > 0
								? items
										.filter((x) => (modelValue as T[])?.includes(x.value))
										.map((x) => x.title)
										.join(', ')
								: placeholder
						}}
					</DsText>
					<DsText v-else size="sm" class="text-nowrap text-ellipsis overflow-hidden">
						{{ items.find((x) => x.value === modelValue)?.title ?? placeholder }}
					</DsText>
				</slot>
				<div>
					<DsIcon name="heroicons:chevron-down" size="sm" :color='Colors.onyx' />
				</div>
			</button>
			<div v-if="state.isOpen" ref="menuRef" class="absolute mt-1 bg-white shadow-lg border border-silver max-h-[250px] max-w-[400px] flex flex-col z-20">
				<slot name="menu-content">
					<div v-if="searchable" class="flex justify-between border-b border-silver">
						<input
							:id="props.label?.replaceAll(' ', '-').toLowerCase() + '-select-search'"
							:value="state.searchFilter"
							:placeholder="searchPlaceholder"
							type="text"
							class="text-onyx border-0 text-sm focus:outline-none focus:ring-0 focus:border-silver block w-full px-3 py-1"
							@input="(e) => state.searchFilter = (e.target as HTMLInputElement).value"
						>
						<div v-if="state.searchFilter" class="flex items-center cursor-pointer" @click="state.searchFilter = ''">
							<DsIcon name="heroicons:x-mark" size="sm" :color="Colors.onyx" aria-hidden="true" />
						</div>
					</div>
					<div class="overflow-y-auto h-full">
						<ul v-if="Array.isArray(props.items) && props.items.length">
							<li v-for="(item, i) in props.items.filter((x) => x.title.toLowerCase().includes(state.searchFilter.toLowerCase()))" :key="i + item.title">
								<div :id="item.title.replaceAll(' ', '-').toLowerCase() + '-option'" class="cursor-pointer justify-between px-3 py-1 hover:bg-bliss flex hover:text-wrap text-nowrap" @click="() => ((dirty = true), onSelect(item.value))">
									<DsText size="sm" class="text-ellipsis overflow-hidden">
										{{ item.title }}
									</DsText>
									<div class="flex items-center">
										<DsIcon v-if="multiple && (modelValue as T[])?.includes(item.value)" name="heroicons:check" size="xs" />
									</div>
								</div>
							</li>
						</ul>
					</div>
				</slot>
			</div>
			<div v-if="!validationResult.isValid" class="relative mb-6">
				<DsText size="xs" :color="Colors.fire" class="absolute top-0">
					{{ validationResult.error }}
				</DsText>
			</div>
		</div>
	</DsLabeledInput>
</template>
