import { Given, When, Then, DataTable } from '@badeball/cypress-cucumber-preprocessor';
import { faker } from '@faker-js/faker';

before(() => {
    cy.createEnterpriseOrganization();
    cy.login();
})

Given(`I am on the workspace creation page`, () => {
    cy.visit('https://app.local.meldrx.com/workspaces/new');
});

Given(`I select {string} workspace type`, (workspaceType: string) => {
    cy.get(`#${workspaceType}-workspace-button`).click();
});

Given(`I click the next button`, () => {
    cy.get('#next-button').click();
});

Given(`I provide a workspace name: {string}`, (workspaceName: string) => {
    cy.get('#workspace-name').type(workspaceName);
});

Given(`I provide a workspace identifier`, () => {
    cy.get('#workspace-identifier').type(faker.database.mongodbObjectId());
});

Given(`I provide a workspace slug`, () => {
    const workspaceSlug = faker.database.mongodbObjectId()
    cy.get('#workspace-url-slug').type(workspaceSlug);

  Cypress.env('workspaceSlug', workspaceSlug);
});

Given(`I click no workspace admin`, () => {
    cy.get('#no-button').click();
});

Given(`I click create workspace`, () => {
    cy.get('#create-workspace-button').click();
});

Given(`I am shown a workspace created modal`, () => {
    cy.get('#workspace-url', { timeout: 10000 }).should('exist').invoke('val').then((wsUrl: string) => {
        const url = new URL(wsUrl);
        Cypress.env('wsId', url.pathname.split('/').pop());
    });
});

Given(`I click Go to Workspace button`, () => {
    cy.get('#go-to-workspace-button').click();
});

Then(`My workspace slug is set correctly`, () => {
    cy.contains(Cypress.env('workspaceSlug'))
});

Given(`I navigate to the workspace manage page`, () => {
    cy.contains('Settings').click();
    cy.contains('General').click();
});