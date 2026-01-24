import { Given, When, Then } from "@badeball/cypress-cucumber-preprocessor";

before(() => {
    Cypress.session.clearAllSavedSessions()
    cy.createDeveloperUser()
});

beforeEach(() => {
    cy.session('login', () => {
        cy.login();
    });
});

Given(`I am on the command center workspaces page`, () => {
    cy.url().visit('https://app.local.meldrx.com/workspaces');
    cy.get('#create-workspace-button', { timeout: 10000 }).should('exist');
});

When(`I click the create workspace button`, () => {
    cy.wait(1000)
    cy.get('#create-workspace-button').click()
    cy.url().should('eq', 'https://app.local.meldrx.com/workspaces/new');
});

When(`I select {string} workspace type`, (workspaceType: string) => {
    if (workspaceType == 'standalone') {
        cy.get('#standalone-workspace-button').click();
    }
    else if (workspaceType == 'linked') {
        cy.get('#linked-workspace-button').click();
    }
});

When(`I click the next button`, () => {
    cy.get('#next-button').click();
});

When(`I provide a workspace name: {string}`, (workspaceName: string) => {
    if (workspaceName == 'blank') {
        cy.get('#workspace-name').clear();
    }
    else {
        cy.get('#workspace-name').type(workspaceName);
    }
});

When(`I click create workspace`, () => {
    cy.get('#create-workspace-button').click();
});

When(`I select a fhir provider: {string}`, (fhirProvider: string) => {
    if (fhirProvider == 'none') {
    }
    else {
        cy.get(`#${fhirProvider}-button`).click();
    }
});

When(`I provide a fhir api url: {string}`, (fhirApiUrl: string) => {
    cy.get('#fhir-api-url').type(fhirApiUrl);
});

When(`The fhir api url is validated`, () => {
    cy.wait(500);
    cy.contains('Validated successfully!').should('exist');
});

When(`I am shown a workspace created modal`, () => {
    cy.get('#workspace-url').should('exist').invoke('val').then((wsUrl: string) => {
        const url = new URL(wsUrl);
        Cypress.env('wsId', url.pathname.split('/').pop());
    });
});

When(`I click Go to Workspace button`, () => {
    cy.get('#go-to-workspace-button').click();
});

Then(`My workspace is accessible`, () => {
    cy.url().should('contain', Cypress.env('wsId'));
});


Then(`I am shown a validation error: {string}`, (validationString: string) => {
    cy.contains(validationString).should('exist');
});

When(`I return to workspaces screen`, () => {
    cy.contains('Return to Home').click()
});

Then(`I am not able to create more workspaces`, () => {
    cy.visit('https://app.local.meldrx.com/workspaces');
    cy.get('#create-workspace-button').should('be.disabled');
    cy.contains('You have reached your Workspace limit.').should('exist');
});

When(`I create one more workspace`, () => {
    cy.url().visit('https://app.local.meldrx.com/workspaces/new');
    cy.get('#standalone-workspace-button').click();
    cy.get('#next-button').click();
    cy.get('#workspace-name').type('My Standalone Workspace')
    cy.get('#create-workspace-button').click();
});