import { Given, When, Then } from '@badeball/cypress-cucumber-preprocessor';
import { faker } from '@faker-js/faker';

before(() => {
    Cypress.session.clearAllSavedSessions()
    cy.createDeveloperUser();
    cy.login();
cy.createDeveloperWorkspace("My Standalone Workspace").then((response) => Cypress.env('wsSlug', response.body.fhirDatabaseDisplayName))

    cy.logout();
});

beforeEach(() => {
    cy.session('login', () => {
        cy.login();
    });
});

Given(`I am on the workspaces page`, () => {
    cy.url().visit('https://app.local.meldrx.com/workspaces');
    cy.url().should('contain', 'https://app.local.meldrx.com/workspaces')
});

Given(`I go the workspace groups page`, () => {
    cy.url().visit(`https://app.local.meldrx.com/workspaces/${Cypress.env('wsSlug')}/groups`);
    cy.url().should('contain', 'https://app.local.meldrx.com/workspaces')
});

When(`I click on the name of the standalone workspace`, () => {
    cy.contains('My Standalone Workspace').click();
});

When(`I am on the workspace manage page`, () => {
    cy.contains('General Settings').should('exist');
});

When(`I click on Groups`, () => {
    cy.contains('Groups').click();
});

When(`I am on the workspace groups page`, () => {
    cy.url().should('contain', '/groups');
});

When(`I click the Create Group button`, () => {
    cy.get('#create-group-button').click();
});

When(`I provide a group name: {string}`, (groupName: string) => {
    if (groupName == 'random') {
        const groupName = faker.word.noun()
        Cypress.env('groupName', groupName);
        cy.get('#name').type(groupName);
        cy
    } else if (groupName != 'none') {
        cy.get('#name').type(groupName);
    }
});

When(`I click the Create Group modal button`, () => {
    cy.get('#create-group-modal-button').click();
});

Then(`The group is created`, () => {
    cy.url().should('contain', '/groups');
    cy.contains(Cypress.env('groupName')).should('exist');
});


Then(`I am shown a field validation error: {string}`, (errorMessage: string) => {
    cy.contains(errorMessage).should('exist');
});

Then(`I am shown a pop-up error: {string}`, (errorMessage: string) => {
    cy.contains(errorMessage);
});