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

Given(`I am on the command center apps page`, () => {
    cy.url().visit('https://app.local.meldrx.com/apps')
    cy.get('#register-app-button', { timeout: 10000 }).should('exist');
});

Then("I click create app",() =>{
    cy.get('#register-app-button').click()
    cy.url().should('eq', 'https://app.local.meldrx.com/apps/new')
})

When(`I provide app name: {string}`, (appName: string) => {
    cy.get('#app-name').type(appName);
});

When(`I provide app launch URL: {string}`, (appLaunchUrl: string) => {
    if(appLaunchUrl !== 'none'){
        cy.get('.grow > #ehr-launch-url').type(appLaunchUrl);
    }
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
        cy.get('.scopeselector').type(`${scopes}{enter}`);
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

Given(`I click next step`, () => {
    cy.get('#next-step-button').click();
});

Given(`I click register app`, () => {
    cy.get('#register-app-button').click();
});

Then(`My app is created: {string}`, (authType: string) => {
    cy.contains('Application Successfully Registered').should('exist');
    cy.get('#app-id').should('exist');

    if(authType == 'confidential' || authType == 'system'){
        cy.get('#client-secret').should('exist');
    }
});
