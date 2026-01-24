<template>

  <div class="space-y-5">

    <div>
      <DsText size="2xl" weight="light">Welcome to MeldRx</DsText>
    </div>
    <div>
      <DsText weight="light">
        Your healthcare provider has shared your health records with you.
        You can view these records in this app or in any app of your choice, on either desktop or mobile devices.
        To learn more about how this works, watch
        <DsLink href="https://www.youtube.com/watch?v=UMiPW831b1o" target="_blank">
          <DsText color="text-secondary">this video</DsText>
        </DsLink>.
      </DsText>
    </div>


    <div>
      <DsText size="2xl" weight="light">Frequently Asked Questions</DsText>
    </div>

    <div class="space-y-5">

      <div>
        <DsText>Q: Why did my healthcare provider share my health records with me?</DsText>
      </div>

      <div>
        <DsText weight="light">
          A: Your healthcare provider shared your health records with you to empower you to manage your health better. You can use your records in apps of your
          choice, share them with other providers, and participate in clinical trials,enhancing your personal care and treatment options.
        </DsText>
      </div>

      <div>
        <DsText>Q: How do I use my health records in other apps?</DsText>
      </div>

      <div>
        <DsText weight="light">
          A: To use your health records in other apps, first ensure the app supports FHIR, the standard format for exchanging health information. Then, follow
          the app’s instructions to connect your health records—usually by logging in with your healthcare provider’s credentials or scanning a QR code. Once
          connected, you can view, manage, and share your health data within the app, giving you more control over your personal health information.
        </DsText>
      </div>

      <div>
        <DsText>Q: How can I get access to more health records than the ones shown here?</DsText>
      </div>

      <div>
        <DsText weight="light">
          A: Contact your healthcare provider and request your full health records. By law, you have the right to access your records in the FHIR format, which
          can be viewed in this app and others. If your provider doesn’t comply, you may have legal options. For more information,
          <DsLink href="https://www.healthit.gov/sites/default/files/YourHealthInformationYourRights_Infographic-Web.pdf" target="_blank">
            <DsText color="text-secondary">click here</DsText>
          </DsLink>.
        </DsText>
      </div>

      <div>
        <DsText>Q: Is my health data safe when shared with apps or other providers?</DsText>
      </div>

      <div>
        <DsText weight="light">
          A: Yes, your health data is protected by strict privacy and security standards, such as encryption and authentication protocols, when shared with apps
          or other providers. However, it’s important to use reputable apps that comply with healthcare regulations like HIPAA. Always review the app’s privacy
          policy and permissions before sharing your data to ensure it’s handled securely.
        </DsText>
      </div>

    </div>

  </div>

</template>
