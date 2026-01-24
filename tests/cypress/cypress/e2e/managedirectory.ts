import { Given, When, Then, DataTable } from '@badeball/cypress-cucumber-preprocessor';
import { faker } from '@faker-js/faker';


before(() => {
    Cypress.session.clearAllSavedSessions()
    cy.createEnterpriseOrganization();
    cy.login();
    cy.createDeveloperWorkspace("Directories Test")
        .then((response) => Cypress.env('wsSlug', response.body.fhirDatabaseDisplayName))
})

beforeEach(() => {
    cy.session('login', () => {
        cy.login();
    });
});

Given(`I am on the directory page for my workspace`, () => {
    cy.visit(`https://app.local.meldrx.com/workspaces/${Cypress.env('wsSlug')}/directory`);
    cy.contains('Directory Settings').should('exist');
});

When(`I click add directory`, () => {
    cy.get('#\\+-add-directory-listing-button').click();
});

When(`I provide a display name: {string}`, (displayName: string) => {
    if(displayName != 'blank'){
        if(displayName == 'random'){
           const randomDisplayName = faker.database.mongodbObjectId()
           Cypress.env('directoryname',randomDisplayName)
           cy.get('#display-name').clear().type(Cypress.env('directoryname'));
        }
        else {

            cy.get('#display-name').clear().type(displayName);
        }
    }
});

When(`I provide a adressline1: {string}`, (addressLine1: string) => {
    if(addressLine1 != 'blank'){
        cy.get('#address-line-1').clear().type(addressLine1);
    }
});

When(`I provide a city: {string}`, (city: string) => {
    if(city != 'blank'){
        cy.get('#city').clear().type(city);
    }
});

When(`I provide a state: {string}`, (state: string) => {
    if(state != 'blank'){
        cy.get('#state').clear().type(state);
    }
});

When(`I provide a postal code: {string}`, (postalCode: string) => {
    if(postalCode != 'blank'){
        cy.get('#postal-code').clear().type(postalCode);
    }
});

When(`I click the Save button`, () => {
    cy.get('#save-button').click();
});

When(`My directory is created: {string}`, (displayName: string) => {
    if(displayName == 'random'){
        cy.contains(Cypress.env('directoryname')).should('exist');
    }
    else{
        cy.contains(displayName).should('exist');
    }
});

When(`I go to the directories page`, () => {
    cy.visit('https://app.local.meldrx.com/directory');
    cy.contains('MeldRx Directory').should('exist');
});

When(`I search for my directory: {string}`, (displayName: string) => {
    if(displayName == 'random'){
        cy.get('#directory-search').type(Cypress.env('directoryname'));
        cy.get('#search-button').click();
    }
    else{
        cy.get('#directory-search').type(displayName);
        cy.get('#search-button').click();
    }
});

Then(`My directory is displayed: {string}`, (displayName: string) => {
    if(displayName == 'random'){
        cy.contains(Cypress.env('directoryname')).should('exist');
    }
    else{
        cy.contains(displayName).should('exist');
    }
});

When(`I toggle the visibility of my directory`, () => {
    cy.get(`#toggle-${Cypress.env('directoryname')}-button`).click();
});

When(`The visibility changes to: {string}`, (visibilityStatus: string) => {
    cy.contains(visibilityStatus).should('exist')
});

Then(`My directory is not displayed: {string}`, (displayName: string) => {
    if(displayName == 'random'){
        cy.contains(Cypress.env('directoryname')).should('not.exist');
    }
    else{
        cy.contains(displayName).should('not.exist');
    }
});

Then(`I am shown a field validation error: {string}`, (validationError: string) => {
    cy.contains(validationError).should('exist')
});

When(`My directory is updated: {string}`, (displayName: string) => {
    if(displayName == 'random'){
        cy.contains(Cypress.env('directoryname')).should('exist');
    }
    else{
        cy.contains(displayName).should('exist');
    }
});

When(`I click edit directory`, () => {
    cy.get(`#edit-${Cypress.env('directoryname')}-button`).click();
});