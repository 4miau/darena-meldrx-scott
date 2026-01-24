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

Given(`I am logged in`, () => {
    cy.url().visit('https://app.local.meldrx.com/workspaces');
    cy.url().should('contain', 'https://app.local.meldrx.com/workspaces')
});

When(`I click the profile image in the header`, () => {
    cy.get('#action-menu-button').click();
});

When(`I click Account Settings from the header menu`, () => {
    cy.contains('Account Settings').click();
});

When(`I am on the Account settings page`, () => {
    cy.url().should('contain', 'https://app.local.meldrx.com/Manage/AccountSettings')
});

When(`I click Edit my profile`, () => {
    cy.contains('Edit my profile').click();
});

When(`I am on the My profile page`, () => {
    cy.url().should('contain', 'https://app.local.meldrx.com/Manage')
});

When(`I provide a First name: {string}`, (firstName: string) => {
    if (firstName == 'none'){
        cy.get('#FirstName').clear({ force: true });
    }else if(firstName != 'none'){
        cy.get('#FirstName').clear({ force: true });
        cy.get('#FirstName').type(firstName);
    }
});

When(`I provide a Last name: {string}`, (lastName: string) => {
    if (lastName == 'none'){
        cy.get('#LastName').clear({ force: true });
    }else if(lastName != 'none'){
        cy.get('#LastName').clear({ force: true });
        cy.get('#LastName').type(lastName);
    }
});

When(`I click the Save button`, () => {
    cy.contains('Save').click();
});

Then(`my profile is updated`, () => {
    cy.get('.ModelValidationMessages').should('not.exist');
});

Then(`I am shown a field validation error: {string}`, (validationError: string) => {
    cy.contains(validationError).should('exist');
});

When(`I click Return to account settings`, () => {
    cy.contains('Return to account settings').click();
});

Then(`I am taken to the account settings page`, () => {
    cy.url().should('contain', 'https://app.local.meldrx.com/Manage/AccountSettings')
});

When(`I click Delete my account`, () => {
    cy.contains('Delete my account').click();
});

When(`I am on the Delete my account page`, () => {
    cy.url().should('contain', 'https://app.local.meldrx.com/Manage/DeletePersonalData')
});

When(`I provide the password: {string}`, (password: string) => {
    const passwordInput = cy.get('#Password');
    const password1 = Cypress.env('password');

    if (password === 'random') {
        passwordInput.type(password1);
    }
    else {
        if (password != 'none') {
            passwordInput.type(password);
        } else {
            passwordInput.clear();
        }
    }
});

When(`I click on the Delete data and close my acccount button`, () => {
    cy.get('#delete-user > .btn').click();
});

Then(`I am shown a validation error: {string}`, (validationError: string) => {
    cy.contains(validationError).should('exist');
});

Then(`I am taken to the login screen`, () => {
    cy.url().should('contain', 'https://app.local.meldrx.com/');
});

Then(`I try to log in with my old details`, () => {
    cy.get('#login-email').type(Cypress.env('username'));
    cy.get('#login-password').type(Cypress.env('password'));
    cy.get('#login-button').click();
});