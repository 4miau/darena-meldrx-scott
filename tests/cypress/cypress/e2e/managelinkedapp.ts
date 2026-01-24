import { Given, When, Then } from '@badeball/cypress-cucumber-preprocessor';
before(() => {
    Cypress.session.clearAllSavedSessions()
    cy.createDeveloperUser();
});

beforeEach(() => {
    cy.session('login', () => {
        cy.login();
    });
});

Given(`I am on step {int} app creation page with an existing linked app`, (arg0: number) => {
    cy.url().visit('https://app.local.meldrx.com/apps/new');
    cy.get(`#provider-button`).click();
    cy.get('#next-step-button').click();
    cy.get('#app-name').type('My App');
    cy.get('#ehr-launch-url').type('http://example.com');
    cy.get(`#public-button`).click();
    cy.get('#scopes-select').click();
    cy.contains('openid').click();
    cy.get('#scopes-select').click();
    cy.get('#add-a-redirect-url-button').click();
    cy.get('#redirect-url-0').type('http://example.com/callback');
    cy.get('#next-step-button').click();
    cy.get('#add-new-linked-app-button').click();
    cy.get(`#epic-button`).click();
    cy.get('#use-my-own-credentials-button').click();
    cy.get('#connection-name').type('Epic Linked App');
    cy.get('#client-id').type('Epic Client Id');
    cy.get('#scopes-select').click();
    cy.contains('openid').click();
    cy.get('#scopes-select').click();
    cy.get('#add-linked-app-button').click({ force: true });
    cy.get('#add-another-linked-app-button').should('be.visible');
    cy.get('#close-button').click();
});

When(`I click on my existing linked app: {string}`, (linkedAppName: string) => {
    cy.get(`#${linkedAppName.replaceAll(' ', '-').toLowerCase()}`).click();
});

When(`I am shown the linked app`, () => {
    cy.contains('Linked App Details').should('exist');
});

When(`I update connection name to: {string}`, (linkedAppName: string) => {
    cy.get('#connection-name').clear({ force: true });
    if(linkedAppName != 'blank'){
    cy.get('#connection-name').type(linkedAppName);
    }
});

When(`I update client id to: {string}`, (clientId: string) => {
    cy.get('.grow > #client-id').clear({ force: true });
    if(clientId != 'blank'){
        cy.get('.grow > #client-id').type(clientId);
    }
});

When(`I select scopes: {string}`, (scopes: string) => {
    if(scopes === 'all'){
        cy.get('#select-all-button').click();
    }
    if(scopes === 'none'){
        cy.get('#select-all-button').click();
        cy.get('#deselect-all-button').click();
    }
    else{
        cy.get('#scopes-select').click();
        scopes.split(' ').forEach(scope => {cy.get('.absolute > .overflow-y-auto').contains(scope).click({ force: true })});
        cy.get('#scopes-select').click();
    }
});

When(`I select linked app scopes: {string}`, (scopes: string) => {
    if(scopes === 'all'){
        cy.get('#select-all-button').click();
    }
    if(scopes === 'none'){
        cy.get(':nth-child(3) > .space-x-2 > #select-all-button').click();
        cy.get(':nth-child(3) > .space-x-2 > #deselect-all-button').click();
    }
    else{
        cy.get(':nth-child(3) > :nth-child(1) > .relative > #scopes-select').click();
        scopes.split(' ').forEach(scope => {cy.get('.absolute > .overflow-y-auto').contains(scope).click({ force: true })});
        cy.get(':nth-child(3) > :nth-child(1) > .relative > #scopes-select').click();
    }
});

When(`I try to save linked app`, () => {
    cy.get('#save-linked-app-button').click({ force: true });
});

When(`My linked app is saved: {string}`, (linkedAppName: string) => {
    cy.contains(linkedAppName).should('exist');
});

Then(`My linked app is updated: {string}, {string}, {string}`, (linkedAppName: string, clientId: string, scopes: string) => {
    cy.get(`#${linkedAppName.replaceAll(' ', '-').toLowerCase()}`).click();
    cy.contains('Linked App Details').should('exist');
    cy.get('#connection-name').should('have.value', linkedAppName);
    cy.get('.grow > #client-id').should('have.value', clientId);
    scopes.split(' ').forEach(scope => {
        cy.contains(scope).should('exist');
      });
});

When(`I click to delete my existing linked app: {string}`, (connectionName: string) => {
    cy.get(`#${connectionName} > #x-button`).click();
});

When(`I click confirm deletion`, () => {
    cy.get('#delete-linked-app-confirm-button').click();
});

Then(`My linked app is deleted`, () => {
    cy.contains('Epic Linked App').should('not.exist');
});

Then(`I am shown an error message: {string}`, (ErrorMessage: string) => {
    cy.contains(ErrorMessage).should('exist');
});

When(`I select authentication method: {string}`, (authMethod: string) => {
    if(authMethod == 'Confidential Client'){
        cy.get('#authentication-method-select').click();
        cy.contains(authMethod).click();
        cy.get('#client-secret-button').click();
        cy.get('#client-secret').clear();
    }
});

When(`I click the cancel button`, () => {
    cy.get('#cancel-edit-linked-app-button').click({ force: true });
});

Given(`I am on the command center apps page`, () => {
    cy.url().visit('https://app.local.meldrx.com/apps');
});

When(`I click the manage button {string}`, (appName: string) => {
    cy.get(`#manage-${appName.toLowerCase().replaceAll(' ','-')}-button`).click();
});

When(`I am on the app manage page`, () => {
    cy.contains('Manage App').should('exist');
});
When(`I create the app`, () => {
    cy.get('#cancel-edit-linked-app-button').click({ force: true });
    cy.contains('Epic Linked App Updated').should('exist');
    cy.get('#register-app-button').click();
    cy.contains('Application Successfully Registered').should('exist');
});