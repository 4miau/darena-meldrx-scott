import { Given, When, Then } from '@badeball/cypress-cucumber-preprocessor';

before(() => {
    cy.createDeveloperUser();
});

Given(`I am logged in as a user`, () => {
    cy.login();
    cy.url().visit('https://app.local.meldrx.com/workspaces');
    cy.contains('A MeldRx workspace is a fully-loaded FHIR server with additional capabilities.').should('exist');
});

Given(`I am on the forgot password screen`, () => {
    cy.visit('https://app.local.meldrx.com/Account/ForgotPassword');
});

When(`I click on the back button`, () => {
    cy.get('#return-to-sign-in > a').click();
    cy.url().should('eq', 'https://app.local.meldrx.com/Account/Login');
});

Then(`I am on the login screen`, () => {
    cy.visit('https://app.local.meldrx.com/Account/Login');
});

When(`I provide the email address: {string}`, (email: string) => {
    const emailInput = cy.get('#Email');
    const username = Cypress.env('username');

    if (email === 'random') {
        emailInput.type(username);
    }
    else {
        if (email) {
            emailInput.type(email);
        } else {
            emailInput.clear();
        }
    }
});

When(`I click on the Submit button`, () => {
    cy.get('#forgot-pwd-submit').click();
});

Then(`I see a success alert`, () => {
    cy.get('#forgot-pwd-success').should('be.visible');
});

Given(`I clear my inbox`, () => {
    cy.clearInbox();
});

Then(`I have exactly one email in my inbox`, () => {
    cy.countUnreadEmails().then((count) => {
        expect(count).to.equal(1);
    });
});

When(`I click on the return to login button`, () => {
    cy.get('#return-to-sign-in').click()
});

Then(`I am provided with an error message: {string}`, (error: string) => {
    cy.get('#Email-error')
        .should('be.visible')
        .contains(error);
});

Given(`I navigate to the Reset Password screen URL`, () => {
    cy.visit('https://app.local.meldrx.com/Account/ResetPassword');
});

Then(`I am shown an error message: {string}`, (error: string) => {
    cy.get('#shared-error').contains(error);
});

Given(`I receive a Forgot Password email`, () => {
    // Get the content of the first email and log it
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

Then(`I arrive at the Reset Password screen`, () => {
    cy.url().should('contain', 'https://app.local.meldrx.com/Account/ResetPassword?userId=');
});

When(`I provide a password: {string}`, (newPassword: string) => {
    const newPasswordInput = cy.get('#Password');
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

When(`I provide a confirmation password: {string}`, (confirmPassword: string) => {
    const confirmPasswordInput = cy.get('#ConfirmPassword');
    const password = Cypress.env('password');

    if (confirmPassword === 'random') {
        confirmPasswordInput.type(password);
    }
    else {
        if (confirmPassword) {
            confirmPasswordInput.type(confirmPassword);
        } else {
            confirmPasswordInput.clear();
        }
    }
});

When(`I click on the reset button`, () => {
    cy.get('#submit').click();
});

Then(`I am shown a server error which contains: {string}`, (errorMessage: string) => {
    cy.get('#validation-summary-error')
        .children()
        .children()
        .filter((index, element) => {
            return RegExp(errorMessage, 'i').test(element.textContent);
        })
        .should('exist');
});

Then(`I am shown errors on the {string} field`, (field: string) => {
    cy.get(`#${field}-error`).should('be.visible');
});

Then(`I am on the reset password confirmation screen`, () => {
    cy.url().should('eq', 'https://app.local.meldrx.com/Account/ResetPasswordConfirmation');
});

Then(`I can log in with new credentials`, () => {
    cy.login();
    cy.url().visit('https://app.local.meldrx.com/workspaces');
    cy.contains('A MeldRx workspace is a fully-loaded FHIR server with additional capabilities.').should('exist');
});
