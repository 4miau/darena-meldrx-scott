import { Given, When, Then } from "@badeball/cypress-cucumber-preprocessor";

before(() => {
    Cypress.session.clearAllSavedSessions() 
    cy.createDeveloperUser();
});

beforeEach(() => {
    cy.session('login', () => {
        cy.login();
    });
});

Given(`I am on the command center apps page`, () => {
    cy.url().visit('https://app.local.meldrx.com/apps');
    cy.get('#register-app-button', { timeout: 10000 }).should('exist');
});

Then("I click create app",() =>{
    cy.get('#register-app-button').click();
    cy.url().should('eq', 'https://app.local.meldrx.com/apps/new');
})

When(`I provide app name: {string}`, (appName: string) => {
    cy.get('#app-name').type(appName);
});

When(`I select user type: {string}`, (userType: string) => {
    cy.get(`#${userType}-button`).click();
});

When(`I select auth type: {string}`, (authType: string) => {
    if(authType == 'system'){

    }
    else{
        cy.get(`#${authType}-button`).click();
    }
});

When(`I select scopes: {string}`, (scopes: string) => {
    if(scopes == 'all'){
        cy.get('#select-all-button').click();
    }
    else{
        cy.get('#scopes-select').click();
        scopes.split(' ').forEach(scope => {cy.contains(scope).click({ force: true })});
        cy.get('#scopes-select').click();
    }
});

When(`I provide a redirect url: {string}`, (redirectUrl: string) => {
    if(redirectUrl == 'none'){

    }
    else{
        cy.get('#add-a-redirect-url-button').click();
        cy.get('#redirect-url-0').type(redirectUrl);
    }
});

When(`I click next step`, () => {
    cy.get('#next-step-button').click();
});

When(`I click create linked app`, () => {
    cy.get('#add-new-linked-app-button').click({force: true});
});

When(`I select a linked app provider: {string}`, (linkedAppProvider: string) => {
    cy.get(`#${linkedAppProvider}-button`).click();

    if(linkedAppProvider == 'other')
    {
        cy.get('#select-a-fhir-api-provider-select').click();
        cy.contains('1Life by 1Life Healthcare, Inc').click();
    }
});

When(`I select to use my own credentials`, () => {
    cy.get('#use-my-own-credentials-button').click();
});

When(`I provide a connection name: {string}`, (connectionName: string) => {
    cy.get('#connection-name').type(connectionName);
});

When(`I provide a client id: {string}`, (clientId: string) => {
    cy.get('#client-id').type(clientId);
});

When(`I provide a client secret: {string}`, (clientSecret: string) => {
    if(clientSecret.includes('none')){

    }
    else if(clientSecret.includes('system')){
        cy.get('#hosted-jwks-button').click()

    }
    else{
        cy.get('#client-secret-button').click();
        cy.get('#client-secret').type(clientSecret);
    }
});

When(`I click add linked app`, () => {
    cy.get('#add-linked-app-button').click({force: true});
    cy.get('#add-another-linked-app-button').should('be.visible');
});

When(`I close the linked app modal`, () => {
    cy.get('#close-button').click();
});

When(`I click register app`, () => {
    cy.get('#register-app-button').click();
});

Then(`My app is created`, () => {
    cy.contains('Application Successfully Registered').should('exist');
    cy.get('#app-id').should('exist');
});

When(`My linked app is created: {string}`, (appName: string) => {
    cy.contains(appName).should('exist');
});
