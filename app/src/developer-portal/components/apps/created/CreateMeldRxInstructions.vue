<script setup lang="ts">
defineProps<{
    clientId: string;
}>();

</script>

<template>
  <DsText size="sm" weight="light">
    <ol class="list-decmial pl-4">
      <li class="list-decimal">
        <DsText size="sm" weight="light">
          Install npm on your machine
        </DsText>
        <div class="pb-2" />
      </li>

      <li class="list-decimal">
        <DsText size="sm" weight="light">
          Install/Update the create-meldrx-tool
        </DsText>
        <DsCodeblock
          :language="'bash'"
          :code="`$ npm i -g create-meldrx-app`"
          :code-to-copy="`npm i -g create-meldrx-app`"
          :show-toast-on-copy="true"
        />
        <div class="pb-2" />
      </li>

      <li class="list-decimal">
        <DsText size="sm" weight="light">
          Create the app template in a directory on your local machine
        </DsText>
        <DsCodeblock
          :language="'bash'"
          :code="`$ npx create-meldrx-app --meldrx-app-id ${clientId} --workspace-url '{Your Workspace URL}'`"
          :code-to-copy="`npx create-meldrx-app --meldrx-app-id ${clientId} --workspace-url '{Your Workspace URL}'}`"
          :show-toast-on-copy="true"
        />
        <div class="pb-2" />
      </li>

      <li class="list-decimal">
        <DsText size="sm" weight="light">
          Follow the prompts and provide information about the type of app you want to
          build (System, Patient, Provider) and the platform you are targeting (Web, Mobile, System, etc.).
        </DsText>
        <div class="pb-2" />
      </li>

      <li class="list-decimal">
        <DsText size="sm" weight="light">
          Depending on the template you chose in the previous step, navigate to the newly created
          directory and follow the instructions found in the respective README.md file. <br>
          The default redirect URL for the NextJS app is `http://localhost:8100/login-callback`.
          You can add it on the App Manage page.
        </DsText>
        <div class="pb-2" />
      </li>

      <li class="list-decimal">
        <DsText size="sm" weight="light">
          Launch your app!
        </DsText>
        <div class="pb-2" />
      </li>
    </ol>
  </DsText>
</template>
