<script setup lang="ts">

import type {ChaiModelCardGroup} from "~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/PublishedApps/ChaiModelCardForm";

const props = defineProps<{
  modelCardForm:  ChaiModelCardGroup[];
}>();

const computedForm = computed(() => {
    const lookup: Record<number, Record<number, string>> = {};

    props.modelCardForm.forEach(group => {
        lookup[group.id] = {};
        group.chaiModelCardItems.forEach(item => {
            lookup[group.id][item.id] = item.answer ?? '';
        });
    });

    return lookup;
});
</script>

<template>
  <div class="border b-black pb-5">
    <!-- Contact Details -->
    <div class="grid grid-cols-2 border b-black">
      <div class="grid grid-rows-2 border b-black p-1">
        <DsText size="sm">
          <strong>Name:</strong> {{ computedForm[1][1] }}
        </DsText>
        <DsText size="sm">
          <strong>Developer:</strong> {{ computedForm[1][2] }}
        </DsText>
      </div>
      <div class="border b-black p-1">
        <DsText size="sm">
          <strong>Inquires or to report an issue:</strong> {{ computedForm[1][3] }}
        </DsText>
      </div>
    </div>

    <!-- Version and Release Date -->
    <div class="grid border b-black gap-4">
      <div class="grid grid-cols-3 gap-4 p-1">
        <DsText size="sm">
          <strong>Release Stage:</strong> {{ computedForm[2][4] }}
        </DsText>
        <DsText size="sm">
          <strong>Release Date:</strong> {{ computedForm[2][5] }}
        </DsText>
        <DsText size="sm">
          <strong>Version:</strong> {{ computedForm[2][6] }}
        </DsText>
      </div>
      <div class="grid grid-cols-3 gap-4 p-1">
        <DsText size="sm">
          <strong>Global Availability:</strong> {{ computedForm[2][7] }}
        </DsText>
        <DsText size="sm">
          <strong>Regulatory Approval, If applicable:</strong> {{ computedForm[2][8] }}
        </DsText>
      </div>
    </div>
    
    <!-- Summary -->
    <div class="grid grid-cols-2 border b-black">
      <div class="flex flex-col border b-black p-1">
        <DsText size="sm">
          <strong>Summary:</strong> {{ computedForm[3][9] }}
        </DsText>
        <DsText size="sm">
          <strong>Keywords:</strong> {{ computedForm[3][10] }}
        </DsText>
      </div>
      
      <!-- Uses and Directions -->
      <div class="flex flex-col border b-black p-1">
        <DsText size="sm">
          <strong>Uses and Directions:</strong>
        </DsText>
        <DsText size="sm">
          <strong>• Intended use and workflow:</strong> {{ computedForm[4][11] }}
        </DsText>
        <DsText size="sm">
          <strong>• Primary intended users:</strong> {{ computedForm[4][12] }}
        </DsText>
        <DsText size="sm">
          <strong>• How to use:</strong> {{ computedForm[4][13] }}
        </DsText>
        <DsText size="sm">
          <strong>• Targeted patient population:</strong> {{ computedForm[4][14] }}
        </DsText>
        <DsText size="sm">
          <strong>• Cautioned out-of-scope settings and use cases:</strong> {{ computedForm[4][15] }}
        </DsText>
      </div>
    </div>

    <!-- Warnings -->
    <div class="grid grid-cols-1 border b-black">
      <div class="bg-black p-1">
        <DsText size="md" color="white">
          Warnings
        </DsText>
      </div>
      <div class="flex flex-col border b-black py-1 px-5">
        <DsText size="sm">
          <strong>• Known risks and limitations:</strong> {{ computedForm[5][16] }}
        </DsText>
        <DsText size="sm">
          <strong>• Known biases or ethical considerations:</strong> {{ computedForm[5][17] }}
        </DsText>
        <DsText size="sm">
          <strong>• Clinical risk level:</strong> {{ computedForm[5][18] }}
        </DsText>
      </div>
    </div>
    
    <!-- Trust Ingredients -->
    <div class="grid grid-cols-1 border b-black">
      <div class="bg-black p-1">
        <DsText size="md" color="white">
          Trust Ingredients
        </DsText>
      </div>
      <div class="flex flex-col border b-black p-1">
        <DsText size="sm" weight="bold">
          Uses and Directions:
        </DsText>
        <DsText class="px-4" size="sm">
          <strong>• Outcome(s) and output(s):</strong> {{ computedForm[6][19] }}
        </DsText>
        <DsText class="px-4" size="sm">
          <strong>• Model type:</strong> {{ computedForm[6][20] }}
        </DsText>
        <DsText class="px-4" size="sm">
          <strong>• Foundation models used in application, if applicable:</strong> {{ computedForm[6][21] }}
        </DsText>
        <DsText class="px-4" size="sm">
          <strong>• Input data source:</strong> {{ computedForm[6][22] }}
        </DsText>
        <DsText class="px-4" size="sm">
          <strong>• Output/Input data type:</strong> {{ computedForm[6][23] }}
        </DsText>
        <DsText class="px-4" size="sm">
          <strong>• Development data characterization:</strong> {{ computedForm[6][24] }}
        </DsText>
        <DsText class="px-4" size="sm">
          <strong>• Bias mitigation approaches:</strong> {{ computedForm[6][25] }}
        </DsText>
        <DsText class="px-4" size="sm">
          <strong>• Ongoing Maintenance:</strong> {{ computedForm[6][26] }}
        </DsText>
        <DsText class="px-4" size="sm">
          <strong>• Security and compliance environment practices or accreditations, if applicable:</strong> {{ computedForm[6][27] }}
        </DsText>
        <DsText class="px-4" size="sm">
          <strong>• Transparency, Intelligibility, and Accountability mechanisms, if applicable:</strong> {{ computedForm[6][28] }}
        </DsText>
        <DsText size="sm" weight="bold">
          Transparency Information:
        </DsText>
        <DsText class="px-4" size="sm">
          <strong>• Funding source of the technical implementation:</strong> {{ computedForm[7][29] }}
        </DsText>
        <DsText class="px-4" size="sm">
          <strong>• 3rd Party Information, If Applicable:</strong> {{ computedForm[7][30] }}
        </DsText>
        <DsText class="px-4" size="sm">
          <strong>• Stakeholders consulted during design of intervention (e.g. patients, providers):</strong> {{ computedForm[7][31] }}
        </DsText>
      </div>
    </div>

    <!-- Key Metrics -->
    <div class="grid grid-cols-1 border b-black">
      <div class="bg-black p-1">
        <DsText size="md" color="white">
          Key Metrics
        </DsText>
      </div>
      <div class="grid grid-cols-3 border b-black">
        <div class="bg-blue-100 border b-black">
          <div class="flex flex-col">
            <div class="text-center pb-3 border b-black">
              <DsText size="sm" weight="bold">
                Usefulness, Usability, and Efficacy
              </DsText>
            </div>
            <div class="p-1 border b-black">
              <DsText size="sm">
                <strong>Goal of metric(s): </strong> {{ computedForm[8][32] }}
              </DsText>
            </div>
            <div class="grid grid-cols-2 border b-black">
              <DsText class="border-r b-black p-1" size="sm">
                <strong>Result: </strong> {{ computedForm[8][33] }}
              </DsText>
              <DsText class="border-l b-black p-1" size="sm">
                <strong>Interpretation: </strong> {{ computedForm[8][34] }}
              </DsText>
            </div>
            <div class="p-1 border b-black">
              <DsText size="sm">
                <strong>Test Type:</strong> {{ computedForm[8][35] }}
              </DsText>
            </div>
            <div class="p-1 border b-black">
              <DsText size="sm">
                <strong>Testing Data Description: </strong> {{ computedForm[8][36] }}
              </DsText>
            </div>
            <div class="p-1 border b-black">
              <DsText size="sm">
                <strong>Validation Process and Justification: </strong> {{ computedForm[8][37] }}
              </DsText>
            </div>
          </div>
        </div>
        
        <div class="bg-red-100 border b-black">
          <div class="flex flex-col">
            <div class="text-center pb-3 border b-black">
              <DsText size="sm" weight="bold">
                Fairness and Equity
              </DsText>
            </div>
            <div class="p-1 border b-black">
              <DsText size="sm">
                <strong>Goal of metric(s): </strong> {{ computedForm[9][38] }}
              </DsText>
            </div>
            <div class="grid grid-cols-2 border b-black">
              <DsText class="border-r b-black p-1" size="sm">
                <strong>Result: </strong> {{ computedForm[9][39] }}
              </DsText>
              <DsText class="border-l b-black p-1" size="sm">
                <strong>Interpretation: </strong> {{ computedForm[9][40] }}
              </DsText>
            </div>
            <div class="p-1 border b-black">
              <DsText size="sm">
                <strong>Test Type:</strong> {{ computedForm[9][41] }}
              </DsText>
            </div>
            <div class="p-1 border b-black">
              <DsText size="sm">
                <strong>Testing Data Description: </strong> {{ computedForm[9][42] }}
              </DsText>
            </div>
            <div class="p-1 border b-black">
              <DsText size="sm">
                <strong>Validation Process and Justification: </strong> {{ computedForm[9][43] }}
              </DsText>
            </div>
          </div>
        </div>
        
        <div class="bg-green-100 border b-black">
          <div class="flex flex-col">
            <div class="text-center pb-3 border b-black">
              <DsText size="sm" weight="bold">
                Safety and Reliability
              </DsText>
            </div>
            <div class="p-1 border b-black">
              <DsText size="sm">
                <strong>Goal of metric(s): </strong> {{ computedForm[10][44] }}
              </DsText>
            </div>
            <div class="grid grid-cols-2 border b-black">
              <DsText class="border-r b-black p-1" size="sm">
                <strong>Result: </strong> {{ computedForm[10][45] }}
              </DsText>
              <DsText class="border-l b-black p-1" size="sm">
                <strong>Interpretation: </strong> {{ computedForm[10][46] }}
              </DsText>
            </div>
            <div class="p-1 border b-black">
              <DsText size="sm">
                <strong>Test Type:</strong> {{ computedForm[10][47] }}
              </DsText>
            </div>
            <div class="p-1 border b-black">
              <DsText size="sm">
                <strong>Testing Data Description: </strong> {{ computedForm[10][48] }}
              </DsText>
            </div>
            <div class="p-1 border b-black">
              <DsText size="sm">
                <strong>Validation Process and Justification: </strong> {{ computedForm[10][49] }}
              </DsText>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Resources -->
    <div class="grid grid-cols-1 border b-black">
      <div class="bg-black p-1">
        <DsText size="md" color="white">
          Resources
        </DsText>
      </div>
      <div class="flex flex-col border b-black py-1 px-5">
        <DsText size="sm">
          <strong>• Evaluation References, If Available:</strong> {{ computedForm[11][50] }}
        </DsText>
        <DsText size="sm">
          <strong>• Clinical Trial, If Available:</strong> {{ computedForm[11][51] }}
        </DsText>
        <DsText size="sm">
          <strong>• Peer Reviewed Publication(s):</strong> {{ computedForm[11][52] }}
        </DsText>
        <DsText size="sm">
          <strong>• Reimbursement status, if applicable:</strong> {{ computedForm[11][53] }}
        </DsText>
        <DsText size="sm">
          <strong>• Patient consent or disclosure required or suggested:</strong> {{ computedForm[11][54] }}
        </DsText>
        <DsText size="sm">
          <strong>• Stakeholders consulted during design of solution:</strong> {{ computedForm[11][55] }}
        </DsText>
      </div>
    </div>
    
  </div>
</template>
