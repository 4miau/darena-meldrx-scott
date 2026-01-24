<script setup lang="ts">
useHead({ title: 'Component Gallery | MeldRx' });
definePageMeta({ anonymous: true })
</script>

<template>
  <DsContainer>
    <div>
      <DsText size="2xl" weight="light">
        Component Gallery
      </DsText>
      <DsDivider />

      <DsText size="xl" weight="light">
        Basics
      </DsText>
      <DsDivider />

      <DsLink href="component-gallery/loaders">
        <DsText size="md">
          Loaders
        </DsText>
      </DsLink>
      <div class="pb-5" />

      <DsLink href="component-gallery/text">
        <DsText size="md">
          Text
        </DsText>
      </DsLink>
      <div class="pb-5" />
      
      <DsLink href="component-gallery/chai">
        <DsText size="md">
          CHAI Model Card
        </DsText>
      </DsLink>
      <div class="pb-5" />

      <DsLink href="component-gallery/buttons">
        <DsText size="md">
          Buttons
        </DsText>
      </DsLink>
      <div class="pb-5" />

      <DsLink href="component-gallery/button-groups">
        <DsText size="md">
          Button Groups
        </DsText>
      </DsLink>
      <div class="pb-5" />

      <DsLink href="component-gallery/text-inputs">
        <DsText size="md">
          Text Inputs
        </DsText>
      </DsLink>
      <div class="pb-5" />

      <DsLink href="component-gallery/checkboxes">
        <DsText size="md">
          Checkboxes
        </DsText>
      </DsLink>
      <div class="pb-5" />

      <DsLink href="component-gallery/cds-cards">
        <DsText size="md">
          CDS Cards
        </DsText>
      </DsLink>
      <div class="pb-5" />

      <DsLink href="component-gallery/document-viewer">
        <DsText size="md">
          Document Viewer
        </DsText>
      </DsLink>
      <div class="pb-5" />

      <DsLink href="component-gallery/selects">
        <DsText size="md">
          Selects (Dropdowns)
        </DsText>
      </DsLink>
      <div class="pb-5" />

      <DsLink href="component-gallery/redirect-url-list">
        <DsText size="md">
          RedirectUrlList
        </DsText>
      </DsLink>
      <div class="pb-5" />

      <DsLink href="component-gallery/tabs">
        <DsText size="md">
          Tabs
        </DsText>
      </DsLink>
      <div class="pb-5" />

      <DsLink href="component-gallery/ehr-logos">
        <DsText size="md">
          EHR Logos
        </DsText>
      </DsLink>
      <div class="pb-5" />

      <DsLink href="component-gallery/svgs">
        <DsText size="md">
          SVGs
        </DsText>
      </DsLink>
      <div class="pb-5" />

      <DsLink href="component-gallery/code-blocks">
        <DsText size="md">
          Code Blocks
        </DsText>
      </DsLink>
      <div class="pb-5" />

      <DsLink href="component-gallery/credential-input">
        <DsText size="md">
          Credentials (DsCredentialInput, DsPrivateKeyInput, DsJwksCredentialInput)
        </DsText>
      </DsLink>
      <div class="pb-5" />

      <DsLink href="component-gallery/form-validation">
        <DsText size="md">
          Form Validation
        </DsText>
      </DsLink>
      <div class="pb-5" />

      <DsLink href="component-gallery/fhir-url-validator">
        <DsText size="md">
          FHIR URL Validator
        </DsText>
      </DsLink>
      <div class="pb-5" />

      <DsLink href="component-gallery/modals">
        <DsText size="md">
          Modals
        </DsText>
      </DsLink>
      <div class="pb-5" />

      <DsLink href="component-gallery/toasts">
        <DsText size="md">
          Toasts
        </DsText>
      </DsLink>
      <div class="pb-5" />

      <DsLink href="component-gallery/drawer">
        <DsText size="md">
          Drawer
        </DsText>
      </DsLink>

      <!-- USER AUTH -->
      <DsDivider />

      <DsText size="xl" weight="light">
        User Auth
      </DsText>
      <DsDivider />

      <DsLink href="component-gallery/user/user-auth">
        <DsText size="md">
          User Auth Data
        </DsText>
      </DsLink>
      <div class="pb-5" />

      <DsLink href="component-gallery/user/user-menu">
        <DsText size="md">
          User Menu
        </DsText>
      </DsLink>
      <div class="pb-5" />

      <!-- APPS -->
      <DsDivider />
      <DsText size="xl" weight="light">
        Apps/Linked Apps Components
      </DsText>
      <DsDivider />

      <DsLink href="component-gallery/apps/new-linked-app">
        <DsText size="md">
          New Linked App (Modal and List)
        </DsText>
      </DsLink>
      <div class="pb-5" />

      <DsLink href="component-gallery/apps/create-meldrx-app">
        <DsText size="md">
          create-meldrx-app Instructions
        </DsText>
      </DsLink>
      <div class="pb-5" />

      <!-- WORKSPACES -->
      <DsDivider />
      <DsText size="xl" weight="light">
        Workspace Components
      </DsText>
      <DsDivider />

      <DsLink href="component-gallery/workspaces/workspace-created">
        <DsText size="md">
          Workspace Created Modal
        </DsText>
      </DsLink>
      <div class="pb-5" />
    </div>
  </DsContainer>
</template>
