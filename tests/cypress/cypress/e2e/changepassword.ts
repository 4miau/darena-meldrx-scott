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

Given(`I am on the change password screen`, () => {
    cy.visit("https://app.local.meldrx.com/Manage/ChangePassword");
});

When(`I provide the old password {string}`, (oldPassword: string) => {
    const oldPasswordInput = cy.get('#OldPassword');
    const password = Cypress.env('password');

    if (oldPassword === 'random') {
        oldPasswordInput.type(password);
    }
    else {
        if (oldPassword) {
            oldPasswordInput.type(oldPassword);
        } else {
            oldPasswordInput.clear();
        }
    }
});

When(`I provide the new password {string}`, (newPassword: string) => {
    const newPasswordInput = cy.get('#NewPassword');
    const password = Cypress.env('password');

    if (newPassword === 'random') {
        newPasswordInput.type(password);
    }
    else {
        if (newPassword) {
            newPasswordInput.type(newPassword);
        } else {
            newPasswordInput.clear();
        }
    }
});

When(`I provide the confirmation password {string}`, (confirmationPassword: string) => {
    const confirmPasswordInput = cy.get('#ConfirmPassword');
    const password = Cypress.env('password');

    if (confirmationPassword === 'random') {
        confirmPasswordInput.type(password);
    }
    else {
        if (confirmationPassword) {
            confirmPasswordInput.type(confirmationPassword);
        } else {
            confirmPasswordInput.clear();
        }
    }
});

When(`I click the update password button`, () => {
    cy.get('#change-password-submit').click();
});

Then(`I am shown an error on the {string} field`, (field: string) => {
    cy.get(`#${field}-error`).should('be.visible');
});

Then(`I am shown a validation error that contains {string}`, (errorMessage: string) => {
    cy.get('#validation-summary-error')
        .children()
        .children()
        .filter((index, element) => {
            return RegExp(errorMessage, 'i').test(element.textContent);
        })
        .should('exist');
});

Then(`I am shown a success alert after updating my password`, () => {
    cy.get('#status-message-alert')
        .should('be.visible')
        .contains('Your password has been changed.');
});

Given(`I am on the login screen`, () => {
    cy.logout();
    cy.visit('https://app.local.meldrx.com/Account/Login');
});

When(`I provide my email address`, () => {
    cy.get('#login-email').type(Cypress.env('username'));
});

When(`I provide the new password`, () => {
    cy.get('#login-password').type('Hunter2!');
});

When(`I click on the Login button`, () => {
    cy.get('#login-button').click();
});

Then(`I am logged in successfully`, () => {
    cy.url().should('eq', 'https://app.local.meldrx.com/workspaces');
});