import { Given, When, Then, DataTable, Before } from '@badeball/cypress-cucumber-preprocessor';
import { faker } from '@faker-js/faker';

before(() => {
    Cypress.session.clearAllSavedSessions()
    const org1Name = faker.company.name();
    const email1 = faker.internet.email();
    const firstName1 = faker.person.firstName();
    const lastName1 = faker.person.lastName();
    const password1 = faker.internet.password({ length: 20, prefix: '!1aA' });

    cy.visit('https://app.local.meldrx.com/OrganizationRequest?returnUrl=%2F');
    cy.get('#OrganizationName').type(org1Name);
    cy.get('#Email').type(email1);
    cy.get('#FirstName').type(firstName1);
    cy.get('#LastName').type(lastName1);
    cy.get('#Password').type(password1);
    cy.get('#ConfirmPassword').type(password1);
    cy.get('#register-submit').click();

    Cypress.env('org1Name', org1Name);
    Cypress.env('email1', email1);

    const org2Name = faker.company.name();
    const email2 = faker.internet.email();
    const firstName2 = faker.person.firstName();
    const lastName2 = faker.person.lastName();
    const password2 = faker.internet.password({ length: 20, prefix: '!1aA' });

    cy.visit('https://app.local.meldrx.com/OrganizationRequest?returnUrl=%2F');
    cy.get('#OrganizationName').type(org2Name);
    cy.get('#Email').type(email2);
    cy.get('#FirstName').type(firstName2);
    cy.get('#LastName').type(lastName2);
    cy.get('#Password').type(password2);
    cy.get('#ConfirmPassword').type(password2);
    cy.get('#register-submit').click();

    Cypress.env('org2Name', org2Name);
    Cypress.env('email2', email2);
})

beforeEach(() => {
    cy.session('login', () => {
        cy.loginAsAdmin();
    });
});

Given(`I am on the users page`, () => {
    cy.visit('https://app.local.meldrx.com/Admin/Identity/Users');
});

When(`I select an existing user`, () => {
    cy.get('.form-control').type(`${Cypress.env('email1')}{enter}`);
    cy.get('tr > :nth-child(1) > .btn').click();
});

When(`I click manage organizations`, () => {
    cy.get('#ManageOrganizations').click();
});

When(`I select an organization to add {string}`, (organizationName: string) => {
    if (organizationName != 'Empty') {
        const organization2Name: string = Cypress.env('org2Name');
        cy.get('#select2-user-organization-container').type(organization2Name)
        cy.get('.select2-results__option').contains(organization2Name).should('exist');
        cy.get('.select2-results__option').contains(organization2Name).click();
    }
});

When(`I select a role {string}`, (role: string)=> {
    if (role != 'Empty'){
        cy.get('#Role').select(role);
    }
});

When(`I click assign`, () => {
    cy.get('#assign-button').click();
});

Then(`The organization is added to the user`, () => {
    cy.contains(Cypress.env('org2Name')).should('exist');
});

Then(`I am shown an error pop-up that contains {string}`, (errorText: string) => {
    cy.contains(errorText).should('exist');
});