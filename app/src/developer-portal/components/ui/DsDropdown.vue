<script setup lang="ts" generic="T extends string | number">
import {Colors} from '~/types/ui/colors'

interface Option<T> {
  value: T;
  title: string;
  hide?: boolean;
}

interface OptionGroup<T> {
  title: string;
  options: Option<T>[];
  hide?: boolean;
}

const props = defineProps<{
  label: string;
  options:(Option<T> | OptionGroup<T>)[];
}>();

const emit = defineEmits<{
  'select': [T]
}>();

const showMenu = ref(false);
const styles = ref<{
  bottom?: string
}>({});

const buttonRef = ref<HTMLElement>();
const menuRef = ref<HTMLElement>();
const dropdownRef = ref<HTMLElement>();

async function toggleMenu() {
    if (showMenu.value) {
        showMenu.value = false;
        return;
    }

    showMenu.value = true;

    await nextTick();

    calculateOffset()
}

function calculateOffset(){
    const buttonRect = buttonRef.value?.getBoundingClientRect();
    if(!buttonRect || !menuRef.value){
        return;
    }
    const spaceBelow = window.innerHeight - buttonRect.bottom;
    const menuHeight = menuRef.value.offsetHeight;

    if (spaceBelow < menuHeight + buttonRect.height) {
        styles.value.bottom = `${buttonRect.height}px`;
    } else {
        styles.value.bottom = undefined;
    }
}

useOnResize({onResizeEnd: calculateOffset})

function selectOption(option: Option<T>) {
    emit('select', option.value);
    toggleMenu();
}

function isOptionGroup(option: Option<T> | OptionGroup<T>): option is OptionGroup<T> {
    return 'options' in option;
}

useClickOutsideElement(dropdownRef, () => { showMenu.value = false });
</script>

<template>
  <div ref="dropdownRef" class="relative">
    <button
      ref="buttonRef"
      class="inline-flex w-full justify-between gap-x-4 rounded-md bg-white px-3 py-2 text-sm text-onyx shadow-sm ring-1 ring-inset ring-silver"
      @click="toggleMenu"
    >
      <slot name="button-text">
        {{ props.label }}
      </slot>
      <DsIcon name="heroicons:chevron-down" size='sm' :color='Colors.gray' aria-hidden="true" />
    </button>
    <div
      v-if="showMenu"
      ref="menuRef"
      :style="styles"
      class="absolute z-10 w-auto rounded-md bg-white shadow-lg ring-1 ring-black ring-opacity-5 my-2"
    >
      <div class="py-1 whitespace-nowrap">
        <slot
          name="menu-content"
        >
          <ul v-if="Array.isArray(props.options) && props.options.length">
            <li v-for="option in props.options.filter(x => !x.hide)" :key="isOptionGroup(option) ? option.title : option.value">
              <template v-if="isOptionGroup(option)">
                  <div class="px-2 py-1 font-light text-xs">
                    {{ option.title }}
                  </div>
                  <ul>
                    <li
                      v-for="subOption in option.options.filter(x => !x.hide)"
                      :key="subOption.value"
                      class="cursor-pointer px-2 py-1 hover:bg-bliss text-sm"
                      @click="selectOption(subOption)"
                    >
                      {{ subOption.title }}
                    </li>
                  </ul>
              </template>
              <template v-else>
                <div
                  class="cursor-pointer px-2 py-1 hover:bg-bliss"
                  @click="selectOption(option)"
                >
                  {{ option.title }}
                </div>
              </template>
              <div v-if="isOptionGroup(option) && props.options.indexOf(option) < props.options.length - 1" class="border-t border-bliss my-1" />
            </li>
          </ul>
        </slot>
      </div>
    </div>
  </div>
</template>
