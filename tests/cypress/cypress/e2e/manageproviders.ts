import { Given, When, Then } from '@badeball/cypress-cucumber-preprocessor';
import { faker } from '@faker-js/faker';

before(() => {
    Cypress.session.clearAllSavedSessions()
    cy.createEnterpriseOrganization();
    cy.login();
    cy.createDeveloperWorkspace("Provider Test")
        .then((response) => {
            Cypress.env('wsSlug', response.body.fhirDatabaseDisplayName)
            const directoryName = faker.database.mongodbObjectId()
            Cypress.env('directoryName',directoryName)
            cy.apiRequest(
                'POST',
                `/api/workspaces/${response.body.fhirDatabaseDisplayName}/directories`,
                {
                    "active": false,
                    "displayName": `${directoryName}`,
                    "address1": "1st Street",
                    "address2": "",
                    "city": "New York",
                    "state": "NY",
                    "zip": "543215",
                    "organizationId": `${response.body.fhirDatabaseDisplayName}`
                }
            )
        })
})

beforeEach(() => {
    cy.session('login', () => {
        cy.login();
    });
});

Given(`I am on the providers page for my workspace`, () => {
    cy.visit(`https://app.local.meldrx.com/workspaces/${Cypress.env('wsSlug')}/providers`);
    cy.contains('Workspace Providers').should('exist');
});

When(`I click add provider`, () => {
    cy.wait(1000)
    cy.get('#add-provider\\(s\\)-button').click();
});

When(`I provide an NPI: {string}`, (npi: string) => {
    if(npi){
        cy.get('#npis-\\(single\\,-or-comma-seperated\\)').type(npi);
    }
});

When(`I select an activation date: {string}`, (activationDate: string) => {
    if (activationDate != 'none') {
        cy.get('#date-picker').type(activationDate);
        cy.get('#date-picker').click();
    }
});

When(`I click add`, () => {
    cy.get('#add-providers\\(s\\)-modal-button').click();
    cy.contains('Add Npis').should('not.exist');
});

Then(`I am shown a field validation error: {string}`, (validationError: string) => {
    cy.contains(validationError).should('exist');
});

When(`My providers are added: {string}`, (providersList: string) => {
    const providers = providersList.split(',');
    for(var provider of providers){
        cy.contains(provider).should('exist');
    }
});

Then(`I reach my provider limit`, () => {
    cy.contains('You have reached your Provider limit.').should('exist');
});

When(`I go to the directories page`, () => {
    cy.visit('https://app.local.meldrx.com/directory');
    cy.contains('MeldRx Directory').should('exist');
});

When(`I search for my directory`, () => {
    cy.get('#directory-search').type(Cypress.env('directoryName'));
    cy.get('#search-button').click();
    cy.contains(Cypress.env('directoryName')).should('exist');
});

When(`My provider count is: {string}`, (count: string) => {
    cy.get('.provider-count').contains(count).should('exist');
});

When(`I click on the details button`, () => {
    cy.get('#details-button').click();
    cy.contains('This page lists all of the providers that are HTI-1 certified').should('exist');
});

Then(`My active providers are displayed: {string}`, (providersList: string) => {
    const providers = providersList.split(',');
    for(var provider of providers){
        cy.contains(provider).should('exist');
    }
});

When(`I click edit provider: {string}`, (providerNpi: string) => {
    cy.get(`#edit-${providerNpi}-button`).click();
});

When(`I edit the provider name: {string}`, (dispalyName: string) => {
    cy.get('#provider-name').clear().type(dispalyName);
});

When(`I confirm edit`, () => {
    cy.get('#edit-provider-button').click();
});

When(`I click deactivate provider: {string}`, (providerNpi: string) => {
    cy.get(`#deactivate-${providerNpi}-button`).click();
});

When(`My providers are deactivated: {string}`, (providerNpi: string) => {
    cy.get(`#reactivate-${providerNpi}-button`).should('exist');
});

When(`I click confirm deactivate`, () => {
    cy.get('#deactivate-npi-confirm-button').click();
});

Then(`My deactivated providers are not displayed: {string}`, (providersList: string) => {
    const providers = providersList.split(',');
    for(var provider of providers){
        cy.contains(provider).should('not.exist');
    }
});

When(`My provider is updated: {string},{string}`, (providerName: string, activationDate: string) => {
    cy.contains(providerName).should('exist');
    cy.contains(activationDate).should('exist');
});
