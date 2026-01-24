import { When, Then } from '@badeball/cypress-cucumber-preprocessor';
import { faker } from '@faker-js/faker';

before(() => {
    cy.createDeveloperUser();
    cy.clearInbox();
    cy.login();
    cy.createDeveloperWorkspace("My Workspace")
        .then((response) => Cypress.env('wsSlug', response.body.fhirDatabaseDisplayName))

    cy.logout();

})

When(`I am on the manage workspace users page`, () => {
    cy.login();
    cy.url().visit(`https://app.local.meldrx.com/workspaces/${Cypress.env('wsSlug')}/users`);
    cy.url().should('contain', 'https://app.local.meldrx.com/workspaces')
});

When(`I click invite workspace users`, () => {
    cy.get('#invite-workspace-users').click();
});

When(`I provide a first name: {string}`, (firstName: string) => {
    if(firstName == 'random'){
        cy.get('#first-name').type(faker.person.firstName());
    }else if(firstName != 'none'){
        cy.get('#first-name').type(firstName);
    }
});

When(`I provide a last name: {string}`, (lastName: string) => {
    if(lastName == 'random'){
        cy.get('#last-name').type(faker.person.lastName());
    }else if(lastName != 'none'){
        cy.get('#last-name').type(lastName);
    }
});

When(`I provide an email: {string}`, (email: string) => {
    if(email == 'random'){
        const randomEmail = faker.internet.email();
        Cypress.env('randomEmail', randomEmail);
        cy.get('#email-address').type(randomEmail);
        cy
    }else if(email != 'none'){
         cy.get('#email-address').type(email);
    }
});

When(`I select a role {string}`, (role: string) => {
    if(role != 'none'){
        cy.get('#role-select').click();
        cy.get(`#${role.toLowerCase()}-option`).click();
    }
});

When(`I click invite workspace user`, () => {
    cy.get('#invite-workspace-user-button').click();
});

When(`The invitation is sent`, () => {
    cy.countUnreadEmails().then((count) => {
        expect(count).to.equal(1);
    });
});

When(`I go to the registration link`, () => {
    cy.logout();
    cy.getEmailContent('first').then((emailBody) => {
        cy.visit(emailBody.inviteUrl);
    });
});

When(`I provide my email`, () => {
    cy.get('#Email').type(Cypress.env('randomEmail'));
});

When(`I provide a password: {string}`, (password: string) => {
    cy.get('#Password').type(password);
});

When(`I confirm my password: {string}`, (confirmPassword: string) => {
    cy.get('#ConfirmPassword').type(confirmPassword);
});

When(`I click register`, () => {
    cy.get('#register-submit').click();
});

When(`My new workspace user is confirmed`, () => {
    cy.url().should('contain','https://app.local.meldrx.com/CompleteRegistration/Status')
});

Then(`I am shown a validation error: {string}`, (errorMessage: string) => {
    cy.contains(errorMessage).should('exist');
});

When(`I click the remove button for an existing user`, () => {
    cy.get(':nth-child(1) > :nth-child(5) > #remove-button').click()
});

Then(`That user is deleted`, () => {
    cy.contains(Cypress.env('randomEmail')).should('not.exist');
});

When(`My new workspace user is created`, () => {
    cy.contains(Cypress.env('randomEmail')).should('exist');
});

When(`I click on the return to sign in`, () => {
    cy.get('#return-to-sign-in').click();
});

When(`I attempt to sign in with the new account and password: {string}`, (password: string) => {
    cy.get('#login-email').type(Cypress.env('randomEmail'));
    cy.get('#login-password').type(password);
    cy.get('#login-button').click();
});

When(`I am logged in`, () => {
    cy.url().should('contain', 'https://app.local.meldrx.com/workspaces/')
});

When(`I click on the user menu`, () => {
    cy.get('#action-menu-button').click();
});

Then(`My email appears in the menu`, () => {
    cy.contains(Cypress.env('randomEmail'))
    cy.contains('Current organization').should('not.exist');
    cy.logout();
});

When(`I click confirm deletion`, () => {
    cy.get('#remove-user-confirm-button').click();
});