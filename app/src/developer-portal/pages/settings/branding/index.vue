<script setup lang="ts">
import { MeldRxSubscription } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Enums/MeldRxSubscription';
import { Colors } from '~/types/ui/colors'

useHead({ title: 'Branding | MeldRx' });
const { $api } = useNuxtApp();
const { subscription } = useSubscription();
const { brandingConfiguration, loadBranding } = useBranding();
const confirmation = useConfirmation();
const config = useRuntimeConfig();
const formRef = ref<FormRef>();

if(subscription.value.subscriptionType != MeldRxSubscription.Enterprise) {
    navigateTo('/workspaces');
}

const allowedImageUploads = [
    { fileExtension: ['.jpg', '.png', '.jpeg'], fileDescription: 'Pictures' },
];

type BrandingConfig = {
  backgroundColor: string;
  menuColor: string;
  menuMouseOverColor: string;
  menuBackground: string;
  footerColor: string;
  newTitle?: string;
  newBackgroundColor?: string;
  newMenuBackground?: string;
  newMenuColor: string;
  newMenuMouseOverColor: string;
  newFooterColor?: string;
  newLogoImageFile?: FileList;
  tmpDisplayNewLogo?: string;
  uploadFileCache?: ArrayBuffer;
  existingLogoImageUri?: string;
};

const defaultConfig: Readonly<BrandingConfig> = {
    backgroundColor: '#014469',
    menuColor: '#004266',
    menuMouseOverColor: '#0c79ac',
    menuBackground: '#095e86',
    footerColor: '#02b689',
    newTitle: '',
    newBackgroundColor: '#014469',
    newMenuColor: '#004266',
    newMenuMouseOverColor: '#0c79ac',
    newMenuBackground: '#095e86',
    newFooterColor: '#02b689',
    newLogoImageFile: undefined,
    tmpDisplayNewLogo: undefined,
    uploadFileCache: undefined,
    existingLogoImageUri: undefined,
};

const state = reactive<{
  isLoading: boolean;
  errorMessage?: string;
  missingFileErrorMessage?: string;
  brandingConfigInfo: BrandingConfig;
  hoverIndex: number;
}>({
    isLoading: false,
    errorMessage: '',
    brandingConfigInfo: defaultConfig,
    hoverIndex: -1,
});

// Handles new logo image file selection
async function handleImageUploadEvent(files?: FileList) {
    if (!files || files.length === 0) {
        state.brandingConfigInfo.tmpDisplayNewLogo = '';
        return;
    }

    try {
        const file = files.item(0)!;
        state.brandingConfigInfo.tmpDisplayNewLogo = URL.createObjectURL(file)
        state.missingFileErrorMessage = undefined;
    } catch {
        notification({
            title: 'Error',
            description: 'Error reading logo file',
            displayTime: 3000
        })
    }
}

// Handles Reset Branding
async function handleResetClick() {
    state.isLoading = true;
    try {
        await $api.subscriptions.deleteCustomization();
        await loadBranding();
        state.brandingConfigInfo.newBackgroundColor = defaultConfig.backgroundColor;
        state.brandingConfigInfo.newMenuBackground = defaultConfig.menuBackground;
        state.brandingConfigInfo.newMenuColor = defaultConfig.menuColor;
        state.brandingConfigInfo.newMenuMouseOverColor = defaultConfig.menuMouseOverColor;
        state.brandingConfigInfo.newFooterColor = defaultConfig.footerColor;
        state.brandingConfigInfo.newTitle = '';
        state.brandingConfigInfo.tmpDisplayNewLogo = undefined;

        notification({
            title: 'Success',
            description: 'App header reset to default successfully',
            displayTime: 3000,
            variant: 'success',
        });
    } catch (error) {
        handleApiError(error, 'Unable to reset branding configuration');
    } finally {
        state.isLoading = false;
    }
}

// Handles Cancel Branding and return to workspaces
async function handleCancelClick() {
    const { isCancelled } = await confirmation(
        'Are you sure you want to discard the Branding changes?',
        'Discard Changes'
    );
    if (isCancelled) { return; }
    navigateTo('/workspaces');
}

// Handles Save Branding
async function handleSaveClick() {
    if (!formRef.value) {
        return;
    }

    const isFormValid = formRef.value?.validate();
    if (!isFormValid) {
        if(!state.brandingConfigInfo.newLogoImageFile && !state.brandingConfigInfo.existingLogoImageUri) {
            state.missingFileErrorMessage = 'Please upload a logo image';
        }
        state.errorMessage = 'Please fix the errors above.';
        return;
    }

    state.isLoading = true;
    try {
        const formData = new FormData();
        // Append existing logo image URI if available
        if (state.brandingConfigInfo.newLogoImageFile && state.brandingConfigInfo.newLogoImageFile[0]) {
            formData.append('LogoImageFile', state.brandingConfigInfo.newLogoImageFile[0], state.brandingConfigInfo.newLogoImageFile?.[0].name);
        } else if (!state.brandingConfigInfo.existingLogoImageUri) {
            state.missingFileErrorMessage = 'Please upload a logo image';
            return;
        }

        formData.append('BackgroundColorHex', state.brandingConfigInfo.newBackgroundColor ?? '');
        formData.append('Title', state.brandingConfigInfo.newTitle ?? '');
        formData.append('MenuBackgroundColor', state.brandingConfigInfo.newMenuBackground ?? '');
        formData.append('MenuButtonColor', state.brandingConfigInfo.newMenuColor ?? '');
        formData.append('MenuMouseOverColor', state.brandingConfigInfo.newMenuMouseOverColor ?? '');
        formData.append('FooterColor', state.brandingConfigInfo.newFooterColor ?? '');

        await $api.subscriptions.saveCustomization(formData);

        successNotification('Customization saved successfully, Refreshing the page')

        await loadBranding();
    } catch (error) {
        handleApiError(error, 'Unable to save branding configuration');
    } finally {
        state.isLoading = false;
    }
}

async function loadCustomization() {
    try {
        if(brandingConfiguration.value) {
            state.brandingConfigInfo.newTitle = brandingConfiguration.value.Title;
            state.brandingConfigInfo.newBackgroundColor = brandingConfiguration.value.BackgroundColorHex;
            if (brandingConfiguration.value.LogoImageUri) {
                state.brandingConfigInfo.tmpDisplayNewLogo = `${config.public.storageUrl}/${brandingConfiguration.value.LogoImageUri}`
                state.brandingConfigInfo.existingLogoImageUri = brandingConfiguration.value.LogoImageUri;
                state.brandingConfigInfo.newMenuBackground = brandingConfiguration.value.MenuBackgroundColor ?? defaultConfig.menuBackground;
                state.brandingConfigInfo.newMenuColor = brandingConfiguration.value.MenuButtonColor ?? defaultConfig.menuColor;
                state.brandingConfigInfo.newMenuMouseOverColor = brandingConfiguration.value.MenuMouseOverColor ?? defaultConfig.menuMouseOverColor;
                state.brandingConfigInfo.newFooterColor = brandingConfiguration.value.FooterColor ?? defaultConfig.footerColor;
            }
        }
    } catch (error) {
        console.error("Failed to load customization:", error);
    }
}

loadCustomization();

</script>

<template>
  <DsContainer>
    <DsLoadingOverlay :loading="state.isLoading" />

    <div>
      <!-- Title / Description -->
      <DsText size="2xl" weight="light">
        Branding
      </DsText>
      <div class="pb-5" />
      <DsText size="sm" weight="light">
        Customize the look and feel of customer facing portals.
      </DsText>
      <div class="pb-5" />

      <!-- Banner Logo & Background Color Details -->
      <div v-if="subscription" class="grid grid-cols-12 gap-14">
        <div class="col-span-4">
          <DsLabeledText
            label="Banner customization"
            text="Customize logo & background color of the header for all workspaces in your MeldRx subscription." />
        </div>

        <!-- Configuration Information -->
        <DsForm ref="formRef">
          <div class="col-span-3">
            <div class="flex flex-col">
              <!-- Change Logo -->
              <div class="space-y-2 m-4" >
                <DsText size="sm" weight="light">
                  Select logo for the header. (.png preferred)
                </DsText>
                <DsFileSelector
                  v-model="state.brandingConfigInfo.newLogoImageFile"
                  :required="false"
                  :allowed-extensions="allowedImageUploads.flatMap(x => x.fileExtension)"
                  :multiple=false
                  :hide-clear-button="true"
                  placeholder-text="Upload a new Logo in .png or .jpg format"
                  @update:model-value="handleImageUploadEvent" />
                  <DsText v-if="state.missingFileErrorMessage" size="sm" color="fire">
                    {{ state.missingFileErrorMessage }}
                  </DsText>
              </div>

              <!-- Change Title -->
              <div class="space-y-2 m-4" >
                <DsText size="sm" weight="light">
                  Change the title for the app bar.
                </DsText>
                <DsTextInput
                      v-model="state.brandingConfigInfo.newTitle"
                      required
                      :placeholder="subscription.organizationName"
                      :rules="[
                        [(v) => !/^\s*$/.test(v!), 'Custom Title is required'],
                        [(v) => v!.length <= 20, 'Custom title must be less than 20 characters']
                     ]"
                      type="text"
                    />
              </div>

              <!-- Change Header Background Color -->
              <div class="m-1 flex items-center">
                <DsText size="sm" weight="light" class="w-1/3">
                  Header background
                </DsText>
                <DsColorPicker v-model="state.brandingConfigInfo.newBackgroundColor" class="w-2/3" />
              </div>

              <!-- Change SideMenu Background Color -->
              <div class="m-1 flex items-center">
                <DsText size="sm" weight="light" class="w-1/3">
                  Side menu background
                </DsText>
                <DsColorPicker v-model="state.brandingConfigInfo.newMenuBackground" class="w-2/3" />
              </div>

              <!-- Change SideMenu Menu Item Color -->
              <div class="m-1 flex items-center">
                <DsText size="sm" weight="light" class="w-1/3">
                  Menu buttons
                </DsText>
                <DsColorPicker v-model="state.brandingConfigInfo.newMenuColor" class="w-2/3" />
              </div>

              <!-- Change SideMenu Menu Item Mouse Over Color -->
              <div class="m-1 flex items-center">
                <DsText size="sm" weight="light" class="w-1/3">
                  Menu mouse over
                </DsText>
                <DsColorPicker v-model="state.brandingConfigInfo.newMenuMouseOverColor" class="w-2/3" />
              </div>

              <!-- Change Footer Background Color -->
              <div class="m-1 flex items-center">
                <DsText size="sm" weight="light" class="w-1/3">
                  Footer menu background
                </DsText>
                <DsColorPicker v-model="state.brandingConfigInfo.newFooterColor"  class="w-2/3" />
              </div>
            </div>
          </div>
        </DsForm>

        <!-- Preview of the Logo and Background Overlay-->
        <div class=" pt-20 h-96 w-96 grid grid-cols-5 grid-rows-[3fr_6fr_1fr]" >
          <!-- Header Menu -->
          <div class="w-full flex items-center justify-start col-span-5"  :style="{ backgroundColor: state.brandingConfigInfo.newBackgroundColor }">
            <div v-if="state.brandingConfigInfo.tmpDisplayNewLogo!=undefined" class="items-center justify-start h-30 text-gray-500">
                <div class="flex items-center space-x-4 m-2">
                  <img :src="state.brandingConfigInfo.tmpDisplayNewLogo" alt="Logo Preview" class="object-contain h-20 w-auto">
                  <DsText size="2xl" weight="light" :color="Colors.white" class="text-4xl whitespace-nowrap overflow-ellipsis overflow-hidden">
                    {{ state.brandingConfigInfo.newTitle }}
                  </DsText>
                </div>
              </div>
              <div v-else class="flex items-center justify-start h-20 pl-4 text-gray-500">
                <div class="pb-[5px]">
                  <DarenaSolutionsLogo />
                </div>
                <DsText size="2xl" weight="light" :color="Colors.white" class="text-4xl pl-2 whitespace-nowrap overflow-ellipsis overflow-hidden">
                  <span class="font-normal">Darena Solutions</span> | MeldRx
                </DsText>
              </div>
          </div>

          <!-- Side Menu -->
          <div class="w-full flex items-center justify-center col-span-1" :style="{ backgroundColor: state.brandingConfigInfo.newMenuBackground }">
            <div class="border-b border-solid" :style="{ borderColor: state.brandingConfigInfo.newMenuBackground }">
              <div
                v-for="(menuItem, index) in ['Menu 1', 'Menu 2', 'Menu 3']"
                :key="index"
                class="text-base text-left font-light leading-5 p-2 mb-1 justify-between cursor-pointer text-white"
                :style="{ backgroundColor: state.hoverIndex === index ? state.brandingConfigInfo.newMenuMouseOverColor: state.brandingConfigInfo.newMenuColor }"
                @mouseover="state.hoverIndex = index"
                @mouseleave="state.hoverIndex = -1"
              >
                {{ menuItem }}
              </div>
            </div>
          </div>

          <!-- Footer -->
          <div class="w-full flex items-center justify-center col-span-5" :style="{ backgroundColor: state.brandingConfigInfo.newFooterColor }">
            <DsText size="md">Footer</DsText>
          </div>
        </div>
      </div>
      <DsDivider />

      <!-- Error Messages -->
      <div v-if="!!state.errorMessage">
        <div class="p-4">
          <DsText size="sm" color="text-fire">
            {{ state.errorMessage }}
          </DsText>
        </div>
      </div>

      <div class="justify-start items-start gap-5 inline-flex">
        <DsButton :color="Colors.white" :text-color='Colors.gray' variant="outline" @click="handleCancelClick">
          Cancel
        </DsButton>
        <DsButton :color="Colors.secondary" @click="handleResetClick">
          Reset to Default
        </DsButton>
        <DsButton :color="Colors.primary" @click="handleSaveClick">
          Save Changes
        </DsButton>
      </div>
    </div>
  </DsContainer>
</template>
