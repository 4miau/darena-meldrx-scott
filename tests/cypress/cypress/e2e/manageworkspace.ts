import {  When, Then } from '@badeball/cypress-cucumber-preprocessor';

before(() => {
    Cypress.session.clearAllSavedSessions()
    cy.createDeveloperUser();
    cy.login();
    cy.createDeveloperWorkspace("My Standalone Workspace")
    cy.apiRequest(
        'POST',
        '/api/workspaces',
        {
            "name": "My Linked Workspace",
            "validationOption": "Enabled",
            "linkedFhirApi": {
                "fhirApiProviderMeldRxIdentifier": "11340",
                "baseUrl": "https://fhir.epic.com/interconnect-fhir-oauth/api/FHIR/R4",
                "patientStrategy": "Default"
            }
        }
    )
    cy.logout();
});

beforeEach(() => {
    cy.session('login', () => {
        cy.login();
    });
});

When(`I am on the command center workspaces page`, () => {
    cy.url().visit('https://app.local.meldrx.com/workspaces');
});


Then(`I head to the workspace manage page: {string}`, (workspaceName: string) => {
    cy.contains(workspaceName).click()
    cy.contains('System Apps').should('exist')

    cy.contains('Settings').click()
    cy.contains('General').click()


    cy.contains('General Settings').should('exist')
    cy.get('#workspace-id')
        .invoke('text')
        .then((workspaceId) => {
            Cypress.env('workspaceId', workspaceId)
        })
})

Then(`I delete the workspace`, () => {
    cy.get('#delete-workspace-button').click()
    cy.get('#delete-workspace-confirm-button').click()
});

Then(`My workspace is deleted`, () => {
    cy.url().should('contain', 'https://app.local.meldrx.com/workspaces');
    // Wait a second because the DOM has to refresh before checking
    cy.wait(1000);
    cy.contains(Cypress.env('workspaceId')).should('not.exist');
});

When(`I change the workspace name to blank`, () => {
    cy.get('#workspace-name').clear();
});

When(`I try to save workspace`, () => {
    cy.get(`#save-button`).click({ force: true });
});

Then(`I am shown a validation error: {string}`, (validationString: string) => {
    cy.contains(validationString).should('exist');
});

When(`I change the workspace name to: {string}`, (workspaceName: string) => {
    if(workspaceName == 'blank'){
        cy.get('#workspace-name').clear({ force: true });
    }
    else{
        cy.get('#workspace-name').clear({ force: true });
        cy.get('#workspace-name').type(workspaceName);
    }
});

Then(`My workspace is updated: {string}`, (workspaceName: string) => {
    cy.url().visit('https://app.local.meldrx.com/workspaces');
    cy.contains(workspaceName).should('exist');
});


When(`I change the fhir api url: {string}`, (fhirApiUrl: string) => {
    cy.get('#fhir-api-url').clear().type(fhirApiUrl);
});

When(`I click validate: {string}`, (validate: string) => {
    if(validate == 'true'){
        cy.get('#validate-button').click();
    }
    else{

    }
});

When(`The fhir api url is validated`, () => {
    cy.wait(500);
    cy.contains('Validated successfully!').should('exist');
});
