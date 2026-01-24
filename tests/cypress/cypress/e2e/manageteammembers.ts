import { Given, When, Then, DataTable } from '@badeball/cypress-cucumber-preprocessor';
import { faker } from '@faker-js/faker';

before(() => {
    Cypress.session.clearAllSavedSessions();
    cy.createDeveloperUser();
    cy.clearInbox();
})

Given(`I am logged in`, () => {
    cy.login();

    cy.visit('https://app.local.meldrx.com/workspaces');
    cy.url().should('contain', 'https://app.local.meldrx.com/workspaces');
});

Given(`I am logged in on the team members page`, () => {
    cy.login();

    cy.visit('https://app.local.meldrx.com/settings/team-members');
    cy.url().should('contain', 'https://app.local.meldrx.com/settings/team-members');
});

When(`I click on Settings`, () => {
    cy.contains('Settings').click();
});

When(`I click on Team Members`, () => {
    cy.contains('Team Members').click();
});

When(`I am on the Team Members page`, () => {
    cy.url().should('contain', 'https://app.local.meldrx.com/settings/team-members');
});

When(`I click the Invite Team Member button`, () => {
    cy.get('#invite-team-member-button').click();
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
        cy.get('#last-name').type(faker.person.lastName());
    } else if (lastName != 'none') {
        cy.get('#last-name').type(lastName);
    }
});

When(`I provide an email: {string}`, (email: string) => {
    if (email == 'random') {
        const randomEmail = faker.internet.email();
        Cypress.env('randomEmail', randomEmail);
        cy.get('#email-address').type(randomEmail);
        cy
    } else if (email != 'none') {
        cy.get('#email-address').type(email);
    }
});

When(`I select a role {string}`, (role: string) => {
    if (role != 'none') {
        cy.get('#role-select').click();
        cy.get(`#${role.toLowerCase()}-option`).click();
    }
});

When(`I click the Invite Team Member modal button`, () => {
    cy.get('.gap-4 > #invite-team-member-button').click();
});

When(`The new team member is created`, () => {
    cy.url().should('contain', 'https://app.local.meldrx.com/settings/team-members');
    cy.logout();
});

When(`The invitation is sent`, () => {
    cy.countUnreadEmails().then((count) => {
        expect(count).to.equal(1);
    });
    cy.getEmailContent('first').then((emailBody) => {
        Cypress.env('inviteUrl', emailBody.inviteUrl);
    });
});

When(`I go to the registration link`, () => {
    cy.visit(Cypress.env('inviteUrl'));
});

Given(`I go to the registration link logged out`, () => {
    Cypress.session.clearAllSavedSessions();
    cy.visit(Cypress.env('inviteUrl'));
});

When(`I provide my email: {string}`, (email: string) => {
    if (email == 'random') {
        cy.get('#Email').type(Cypress.env('randomEmail'));
    }
    else if (email != 'none') {
        cy.get('#Email').type(email);
    }
});

When(`I provide a password: {string}`, (password: string) => {
    if (password != 'none') {
        cy.get('#Password').type(password);
    }
});

When(`I provide a confirmation password: {string}`, (confirmPassword: string) => {
    if (confirmPassword != 'none') {
        cy.get('#ConfirmPassword').type(confirmPassword);
    }
});

When(`I click Complete registration`, () => {
    cy.get('#register-submit').click();
});

Then(`The team member is confirmed`, () => {
    cy.contains('Your registration has been successfully completed.');
});

Then(`I am shown a field validation error: {string}`, (errorMessage: string) => {
    cy.contains(errorMessage).should('exist');
});

Then(`I am shown a complete registration error which contains: {string}`, (errorMessage: string) => {
    cy.contains(errorMessage);
});

When(`I click the Remove button for an existing team member`, () => {
    cy.get(':nth-child(2) > :nth-child(5) > #remove-button').click();
});

Then(`The user is deleted`, () => {
    cy.contains(Cypress.env('randomEmail')).should('not.exist');
});

When(`I click the Remove button for the team member I am logged in as`, () => {
    cy.get(':nth-child(1) > :nth-child(5) > #remove-button').click();
});

Then(`I am shown an error: {string}`, (errorMessage: string) => {
    cy.contains(errorMessage).should('exist');
});

When(`I click confirm deletion`, () => {
    cy.get('#remove-member-confirm-button').click();
});