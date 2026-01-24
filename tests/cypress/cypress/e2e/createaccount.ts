import { Given, When, Then } from '@badeball/cypress-cucumber-preprocessor';
import { faker } from '@faker-js/faker/locale/en';

Given(`I am on the create account screen`, () => {
    cy.visit("https://app.local.meldrx.com/OrganizationRequest")
});

When(`I click on the back button`, () => {
    cy.get('#register-back-to-login').click()
});

Then(`I am on the sign in screen`, () => {
    cy.visit('https://app.local.meldrx.com/Account/Login');
});

When(`I provide an organization name: {string}`, (orgName: string) => {
    const orgNameInput = cy.get('#OrganizationName');
    Cypress.env('orgName', faker.company.name());

    if (orgName === 'random') {
        orgNameInput.type(Cypress.env('orgName'));
    }
    else {
        if (orgName) {
            orgNameInput.type(orgName);
        } else {
            orgNameInput.clear();
        }
    }
});

When(`I provide an email: {string}`, (email: string) => {
    const emailInput = cy.get('#Email');
    Cypress.env('email', faker.internet.email());

    if (email === 'random') {
        emailInput.type(Cypress.env('email'));
    }
    else {
        if (email) {
            emailInput.type(email);
        } else {
            emailInput.clear();
        }
    }
});

When(`I provide a first name: {string}`, (firstName: string) => {
    const firstNameInput = cy.get('#FirstName');
    Cypress.env('firstName', faker.person.firstName());

    if (firstName === 'random') {
        firstNameInput.type(Cypress.env('firstName'));
    }
    else {
        if (firstName) {
            firstNameInput.type(firstName);
        } else {
            firstNameInput.clear();
        }
    }
});

When(`I provide a last name: {string}`, (lastName: string) => {
    const lastNameInput = cy.get('#LastName');
    Cypress.env('lastName', faker.person.lastName());

    if (lastName === 'random') {
        lastNameInput.type(Cypress.env('lastName'));
    }
    else {
        if (lastName) {
            lastNameInput.type(lastName);
        } else {
            lastNameInput.clear();
        }
    }
});

When(`I provide a password: {string}`, (password: string) => {
    const passwordInput = cy.get('#Password');
    Cypress.env('password', password);

    if (password) {
        passwordInput.type(password);
    } else {
        passwordInput.clear();
    }
});

When(`I provide a confirmation password: {string}`, (confirmPassword: string) => {
    const confirmPasswordInput = cy.get('#ConfirmPassword');

    if (confirmPassword) {
        confirmPasswordInput.type(confirmPassword);
    } else {
        confirmPasswordInput.clear();
    }
});

When(`I click on the create account button`, () => {
    cy.get('#register-submit').click();
});

Then(`I am on the organization request sent screen`, () => {
    cy.url().should('eq', 'https://app.local.meldrx.com/OrganizationRequest/Status');
    cy.get('#create-account-success').should('contain.text', 'Your account has been successfully created.');
});

Then(`I am shown a server error which contains: {string}`, (errorMessage: string) => {
    cy.get('.field-validation-error').contains(errorMessage);
});

Then(`I am shown errors on the {string} field`, (field: string) => {
    cy.get(`.field-validation-error[data-valmsg-for="${field}"]`).should('be.visible');
});


When(`I receive a confirmation email`, () => {
    cy.getEmailContent('first').then((emailBody) => {
        expect(emailBody).to.not.be.empty;
        expect(emailBody.actionLink).to.exist;
        expect(emailBody.actionLink).to.not.be.empty;
    });
});

When(`I go to the link in the email`, () => {
    cy.getEmailContent('first').then((emailBody) => {
        cy.visit(emailBody.actionLink);
    });
});

When(`I am on the email confirmation screen`, () => {
    cy.url().should('contain', 'ConfirmEmail')
});

When(`I click on the return to sign in`, () => {
    cy.get('#return-to-sign-in').click();
});

When(`I attempt to sign in with the new account`, () => {
    cy.get('#login-email').type(Cypress.env('email'));
    cy.get('#login-password').type(Cypress.env('password'));
    cy.get('#login-button').click();
});

When(`I am logged in`, () => {
    cy.url().should('eq', 'https://app.local.meldrx.com/workspaces')
});

When(`I click on the user menu`, () => {
    cy.get('#action-menu-button').click();
});

Then(`My email and organization name appear in the menu`, () => {
    cy.contains(Cypress.env('email'))
    cy.contains(Cypress.env('orgName'))
});
