import { Given, When, Then } from '@badeball/cypress-cucumber-preprocessor';

before(() => {
    Cypress.session.clearAllSavedSessions()
    cy.createDeveloperUser();
    cy.login();

    cy.createDeveloperWorkspace("My Workspace")
        .then((response) => Cypress.env('wsSlug', response.body.fhirDatabaseDisplayName))
    cy.apiRequest(
        'POST',
        '/api/apps/batch',
        {
            "clientName": "My App",
            "publisherUrl": "",
            "soFAppUserType": "System",
            "tokenEndpointAuthMethod": "none",
            "ehrLaunchUrl": "",
            "secretType": "ClientSecret",
            "jwksUri": "",
            "scope": "openid",
            "redirectUris": [],
            "postLogoutRedirectUris": [],
            "linkedApps": []
        }
    )

    cy.logout();
})

beforeEach(() => {
    cy.session('login', () => {
        cy.login();
    });
});

Given(`I am on the command center workspaces page`, () => {
    cy.url().visit('https://app.local.meldrx.com/workspaces');
});

Given(`I go the workspace system apps page`, () => {
    cy.url().visit(`https://app.local.meldrx.com/workspaces/${Cypress.env('wsSlug')}/apps`);
    cy.url().should('contain', 'https://app.local.meldrx.com/workspaces')
});

When(`I click the manage button {string}`, (workspaceName: string) => {
    cy.contains(workspaceName).click()
});

When(`I click system apps`, () => {
    cy.contains('System Apps').click({force:true});
});

When(`I click add workspace app`, () => {
    cy.get('#add-workspace-app-button').click();
});

When(`I select an app to add: {string}`, (appName: string) => {
    if(appName != 'none'){
    cy.get('#available-apps-select').click();
    var appNameFormatted = appName.toLowerCase().replace(' ','-');
    cy.get(`#${appNameFormatted}-option`).contains(appName).click({force:true});
    }
});

When(`I select the role for my app: {string}`, (appRole: string) => {
    if(appRole != 'none'){
    cy.get('#role-select').click();
    cy.get(`#${appRole.toLowerCase()}-option`).click({force:true});
    }
});

When(`I add my selected workspace app`, () => {
    cy.get('#add-new-workspace-app-button').click();
});

Then(`My workspace app is added: {string}`, (appName: string) => {
    cy.contains(appName).should('exist');
});

When(`I am shown a validation error: {string}`, (errorString: string) => {
    cy.contains(errorString).should('exist');
});

When(`I click the cancel button`, () => {
    cy.get('#cancel-button').click();
});

Then(`I am shown a server error: {string}`, (errorString: string) => {
    cy.contains(errorString).should('exist');
});

When(`I remove my existing workspace app`, () => {
    cy.get('#remove-button').click();
});

Then(`My workspace app is removed: {string}`, (appName: string) => {
    cy.contains(appName).should('not.exist');
});
