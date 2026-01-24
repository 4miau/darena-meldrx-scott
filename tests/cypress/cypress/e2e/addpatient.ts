import { Given, When, Then, DataTable } from '@badeball/cypress-cucumber-preprocessor';
import { faker } from '@faker-js/faker';

before(() => {
    Cypress.session.clearAllSavedSessions()
    cy.createDeveloperUser();
    cy.login();
    cy.createDeveloperWorkspace("My Standalone Workspace").then((response) => {
            expect(response.status).to.eq(200)
            Cypress.env('slug', response.body.fhirDatabaseDisplayName)
    })

    cy.logout();
});

beforeEach(() => {
    cy.session('login', () => {
        cy.login();
    });
});

Given(`I am on the workspace patients page`, () => {
    const slug = Cypress.env('slug');
    const targetUrl = `https://app.local.meldrx.com/workspaces/${slug}/patients`;

    cy.url().visit(targetUrl);
    cy.url().should('eq', targetUrl);
    cy.contains('View details about the patients in your workspace.').should('exist');

    // This is a hack because when container loads for first time, the patients page double loads once
    if(Cypress.env('firstLoad'))
    {
        cy.wait(10000);
        Cypress.env('firstLoad', false);
    }
});

When(`I click the Add Patient button`, () => {
    cy.get('#add-patient-button').click();
    cy.contains('Provide details to add or invite the patient').should('exist');
});

When(`I provide a first name: {string}`, (firstName: string) => {
    if (firstName == 'random') {
        cy.get('#first-name').type(faker.person.firstName());
    } else if (firstName != 'none') {
        cy.get('#first-name').type(firstName);
    }
});

When(`I provide a last name: {string}`, (lastName: string) => {
    if (lastName == 'random') {
        const lastName = faker.person.lastName();
        Cypress.env('lastName', lastName);
        cy.get('#last-name').type(lastName);
        cy
    } else if (lastName != 'none') {
        cy.get('#last-name').type(lastName);
    }
});

When(`I select a sex: {string}`, (sex: string) => {
    if (sex != 'none') {
        cy.get('#sex-select').click();
        cy.contains(sex).click({ force: true });
    }
});

When(`I select a Date of birth before today: {string}`, (dateOfBirth: string) => {
    if (dateOfBirth != 'none') {
        cy.get('#date-picker').type(dateOfBirth);
        cy.get('#date-picker').click();
    }
});

When(`I provide an email: {string}`, (email: string) => {
    if (email == 'random') {
        cy.get('#email-address').type(faker.internet.email());
    } else if (email != 'none') {
        cy.get('#email-address').type(email);
    }
});

When(`I click the Add Patient modal button`, () => {
    cy.get('.w-full > #add-patient-button').click();
});

Then(`The patient is added`, () => {
    cy.url().should('contain', '/patients');
    cy.contains(Cypress.env('lastName')).should('exist');
});

Then(`I am shown a field validation error: {string}`, (errorMessage: string) => {
    cy.contains(errorMessage).should('exist');
});