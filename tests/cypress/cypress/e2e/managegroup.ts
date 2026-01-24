import { Given, When, Then, DataTable } from '@badeball/cypress-cucumber-preprocessor';
import { faker } from '@faker-js/faker';

before(() => {
    Cypress.session.clearAllSavedSessions()
    cy.createDeveloperUser();
    cy.login();
cy.createDeveloperWorkspace("My Standalone Workspace").then((response) => {
        Cypress.env('wsSlug', response.body.fhirDatabaseDisplayName)
    cy.apiRequest(
        'POST',
        `/api/fhir/${response.body.fhirDatabaseDisplayName}/group`,
        {
            "resourceType": "Group",
            "type": "person",
            "actual": true,
            "name": "Group1",
            "identifier": [
                {
                    "system": "meldrx",
                    "value": "Group1"
                }
            ]
        }
    )
    cy.apiRequest(
        'POST',
        `/api/fhir/${response.body.fhirDatabaseDisplayName}/group`,
        {
            "resourceType": "Group",
            "type": "person",
            "actual": true,
            "name": "Group2",
            "identifier": [
                {
                    "system": "meldrx",
                    "value": "Group2"
                }
            ]
        }
    )
    cy.apiRequest(
        'POST',
        `/api/fhir/${response.body.fhirDatabaseDisplayName}/patient`,
        {
            "resourceType": "Patient",
            "name": [
                {
                    "use": "usual",
                    "family": "Smith",
                    "given": [
                        "John"
                    ]
                }
            ],
            "active": true,
            "birthDate": "1980-01-01",
            "gender": "male",
            "telecom": [
                {
                    "system": "email",
                    "value": "test@email.com"
                }
            ]
        }
    )
    })

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

When(`I click on the Edit button: {string}`, (name: string) => {
    cy.get(`#edit-${name.toLowerCase().replaceAll(' ','-')}-button`).click();
});

When(`I change the group name to: {string}`, (group: string) => {
    cy.get('#name').clear({ force: true });
    cy.get('#name').type(group);
});

When(`I click the Edit Group modal button`, () => {
    cy.get('#edit-group-button').click();
});

Then(`the group name is updated`, () => {
    cy.url().should('contain', '/groups');
    cy.contains(('TestGroup')).should('exist');
});

When(`I provide a group name: {string}`, (groupName: string) => {
    if(groupName == 'none'){
        cy.get('#name').clear({ force: true });
    }
    else{
        cy.get('#name').clear({ force: true });
        cy.get('#name').type(groupName);
    }
});

Then(`I am shown a field validation error: {string}`, (validationError: string) => {
    cy.contains(validationError).should('exist');
});

Then(`I am shown a pop-up error: {string}`, (arg0: string) => {
    cy.contains('Unable to update group');
});

When(`I click on Patients`, () => {
    cy.contains('Patients').click();
});

When(`I am on the workspace patients page`, () => {
    cy.url().should('contain', '/patients');
});

When(`I select actions Add to Group`, () => {
    cy.get('#john-smith-actions').click();
    cy.contains('Add To Group').click({ force: true });
});

When(`I select group {string}`, (arg0: string) => {
    cy.get('#group-name-select').click();
    cy.contains('TestGroup').click({ force: true });
});

When(`I click the Add to Group modal button`, () => {
    cy.get('#add-to-group-button').click();
});

Then(`the user has been added to the group`, () => {
    cy.url().should('contain', '/patients');
});

When(`I click the name of the group`, () => {
    cy.contains('TestGroup').click();
});

When(`I am on the Manage Group page`, () => {
    cy.contains('Manage Group').should('exist');
});

When(`I click the Remove button`, () => {
    cy.get('#remove-button').click();
});

When(`I click the Remove Patient from Group modal button`, () => {
    cy.get('#remove-patient-from-group-confirm-button').click();
});

Then(`the patient is removed from the group`, () => {
    cy.contains('This Group is Empty').should('exist');
});

When(`I click the Delete button: {string}`, (name: string) => {
    cy.get(`#delete-${name.toLowerCase().replaceAll(' ','-')}-button`).click();
});

When(`I click the Delete Group modal button`, () => {
    cy.get('#delete-group-confirm-button').click();
});

Then(`the group is deleted`, () => {
    cy.url().should('contain', '/groups');
    cy.contains('TestGroup').should('not.exist');
});