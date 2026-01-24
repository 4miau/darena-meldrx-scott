// cypress/support/types.d.ts
declare namespace Cypress {
    interface Chainable {
        login(): Chainable<Element>;
        logout(): Chainable<Element>;
        loginAsAdmin(): Chainable<Element>;
        clearInbox(): Chainable<Element>;
        countUnreadEmails(): Chainable<Element>;
        getEmailContent(position: string): Chainable<any>;
        createDeveloperApp(appName: string, appType: string, scope: string, ehrLaunchUrl?: string, redirectUris?: string[]): Chainable<any>
        createCdsHookApp(appName: string, cdsHookServiceUrl: string): Chainable<any>
        createDeveloperUser(): Chainable<Element>;
        createDeveloperWorkspace(workspaceName: string): Chainable<any>
        createEnterpriseWorkspace(workspaceName: string, workspaceSlug: string, workspaceIdentifier: string): Chainable<any>
        createPatientInWorkspace(workspaceSlug: string, givenName: string, familyName: string, dateOfBirth: string, gender: ('male'|'female'|'other'|'unknown'), email?: string): Chainable<any>
        createEnterpriseOrganization(): Chainable<Element>;
        apiRequest(method:string, endpoint:string, requestBody: {}): Chainable<any>;
    }
}
