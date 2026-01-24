<script setup lang="ts">
import { Colors } from '~/types/ui/colors';
import type { DirectoryListingDto } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/DirectoryListingDto';

const props = defineProps<{
    directoryListing: DirectoryListingDto
}>();

const emit = defineEmits<{
  'close': [];
  'save': [value: DirectoryListingDto];
}>();

// "Add/Edit Directory"...
function onAddDirectoryListing() {
    // Try to validate the form...
    if (!formRef.value) { return; }
    const isFormValid = formRef.value.validate();
    if (!isFormValid) { return; }

    emit('save', directoryForm.value);
    emit('close')
}

const formRef = ref<FormRef>();
const showAddressLine2 = ref<boolean>(false);
const directoryForm = ref<DirectoryListingDto>({
    id: props.directoryListing.id,
    active: props.directoryListing.active,
    displayName: props.directoryListing.displayName,
    address1: props.directoryListing.address1,
    address2: props.directoryListing.address2,
    city: props.directoryListing.city,
    state: props.directoryListing.state,
    zip: props.directoryListing.zip
})

watch(() => props.directoryListing, (v) => {
    directoryForm.value = {
        id: v.id,
        active: v.active,
        displayName: v.displayName,
        address1: v.address1,
        address2: v.address2,
        city: v.city,
        state: v.state,
        zip: v.zip
    }
})
</script>

<template>
  <DsModal :model-value="true" @close="$emit('close')">
    <DsModalProgressCard :total-steps="1" :current-step="1">
      <DsForm ref="formRef">
        <!-- Icon -->
        <div class="flex w-full py-8 bg-space justify-center items-center">
          <div class="flex w-full items-center justify-center">
            <div class="w-12 h-12 bg-silver rounded-full flex justify-center items-center">
              <MeldRxMail />
            </div>
          </div>
          <div class="flex-1" />
        </div>

        <div class="flex w-full p-4">
          <div class="flex flex-col w-full">
            <!-- Heading -->
            <div class="w-full flex-col justify-center items-center gap-5 flex">
              <DsText size="2xl" weight="light">
                Add Directory Listing
              </DsText>

              <DsText size="sm" weight="light" class="text-center">
                Provide details to add Directory Listing.
              </DsText>
            </div>

            <div class="grid grid-cols-1 gap-4">
              <!-- Display name -->
              <DsTextInput
                v-model="directoryForm.displayName"
                :required="true"
                :rules="[[v => !!v, 'Display name is required']]"
                type="text"
                label="Display name"
              />

              <!-- Address Line 1 -->
              <DsTextInput
                v-model="directoryForm.address1"
                :required="true"
                :rules="[[v => !!v, 'Address is required']]"
                type="text"
                label="Address Line 1"
              />

              <!-- Address Line 2 -->
              <DsTextInput
                v-if="showAddressLine2 || directoryForm.address2"
                v-model="directoryForm.address2"
                type="text"
                label="Address Line 2"
              >
                <template #right>
                  <DsButton
                    v-if="showAddressLine2 || directoryForm.address2"
                    variant="outline"
                    :color='Colors.gray'
                    :text-color="Colors.black"
                    @click="() => { directoryForm.address2 = ''; showAddressLine2 = false; }"
                  >X
                </DsButton>
                </template>
              </DsTextInput>

              <DsButton
                v-if="!showAddressLine2 && !directoryForm.address2"
                id="add-an-address-line"
                class="flex"
                variant="transparent"
                :text-color="Colors.fire"
                @click="() => { directoryForm.address2 = '';showAddressLine2 = true; }"
              >
                + Add an Address Line
              </DsButton>

              <!-- City -->
              <DsTextInput
                v-model="directoryForm.city"
                :required="true"
                :rules="[[v => !!v, 'City is required']]"
                type="text"
                label="City"
              />

              <!-- State -->
              <DsTextInput
                v-model="directoryForm.state"
                :required="true"
                :rules="[[v => !!v, 'State is required']]"
                type="text"
                label="State"
              />

              <!-- Postal Code -->
              <DsTextInput
                v-model="directoryForm.zip"
                :required="true"
                :rules="[[v => !!v, 'Postal Code is required']]"
                type="text"
                label="Postal Code"
              />
            </div>

            <!-- Buttons -->
            <DsContainer>
              <div class="flex justify-center w-full gap-5">
                <DsButton id='cancel-button' :color="Colors.secondary" :text-color='Colors.secondary' variant="outline" @click="$emit('close')">
                  Cancel
                </DsButton>
                <DsButton
                  id='save-button'
                  :color="Colors.secondary"
                  @click="onAddDirectoryListing"
                >
                Save
                </DsButton>
              </div>
            </DsContainer>
          </div>
        </div>
      </DsForm>
    </DsModalProgressCard>
  </DsModal>
</template>
