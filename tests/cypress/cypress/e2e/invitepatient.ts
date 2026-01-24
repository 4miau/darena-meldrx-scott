import {Given, Then, When} from "@badeball/cypress-cucumber-preprocessor";
import {faker} from "@faker-js/faker";

before(() => {
    Cypress.session.clearAllSavedSessions()
    cy.createDeveloperUser();
    cy.clearInbox();
    cy.login();
    cy.createDeveloperWorkspace("My Standalone Workspace")
        .then((response) => {
        Cypress.env('wsSlug', response.body.fhirDatabaseDisplayName)
        cy.createPatientInWorkspace(response.body.fhirDatabaseDisplayName, "John", "Smith", "1980-01-01", "male", "test@email.com")
        cy.createPatientInWorkspace(response.body.fhirDatabaseDisplayName, "Jane", "Doe", "1970-12-01", "female", "janedoe@email.com")
        const slug = Cypress.env('wsSlug');
        const targetUrl = `https://app.local.meldrx.com/workspaces/${slug}/patients`;
        cy.url().visit(targetUrl);
        cy.contains('Jane Doe').should('exist');
        cy.get(`#jane-doe-actions`).click();
        cy.contains('Send Invitation').click();
        cy.get('#send-invitation-button').click();
        cy.contains('The patient invite has been sent.').should('exist');
        cy.getEmailContent('first').then((emailBody) => {
            Cypress.env('inviteUrl',emailBody.inviteUrl)
        });
    })
    cy.createDeveloperWorkspace("Good Health Clinic")
        .then((response) => {
            Cypress.env('wsSlug2', response.body.fhirDatabaseDisplayName)
            cy.createPatientInWorkspace(response.body.fhirDatabaseDisplayName, "Johny", "Smith", "1980-01-01", "male", "test@goodhealth.com")
            const slug = Cypress.env('wsSlug2');
            const targetUrl = `https://app.local.meldrx.com/workspaces/${slug}/patients`;
            cy.url().visit(targetUrl);
            cy.contains('Johny Smith').should('exist');
            cy.get(`#johny-smith-actions`).click();
            cy.contains('Send Invitation').click();
            cy.get('#send-invitation-button').click();
            cy.contains('The patient invite has been sent.').should('exist');
            cy.getEmailContent('first').then((emailBody) => {
                Cypress.env('inviteUrl2',emailBody.inviteUrl)
            });
        })
    cy.logout();
});


Given(`I am on the workspace patients page`, () => {
    cy.login();
    const slug = Cypress.env('wsSlug');
    const targetUrl = `https://app.local.meldrx.com/workspaces/${slug}/patients`;

    cy.url().visit(targetUrl);
    cy.contains('View details about the patients in your workspace.').should('exist');

    // This is a hack because when container loads for first time, the patients page double loads once
    if(Cypress.env('firstLoad'))
    {
        cy.wait(10000);
        Cypress.env('firstLoad', false);
    }
});

When('I click on the patient action menu: {string}', (patientName: string) => {
    cy.get(`#${patientName.toLowerCase().replaceAll(' ','-')}-actions`).click();
})

When('I click send an invite to the patient action', () => {
    cy.contains('Send Invitation').click();
})

When('I click the send invitation button', () => {
    cy.get('#send-invitation-button').click();
    cy.contains('The patient invite has been sent.').should('exist');
})

When('I log out as a provider', () => {
    cy.logout();
})

When('I go to the invitation link', () => {
    cy.getEmailContent('first').then((emailBody) => {
        cy.visit(emailBody.inviteUrl);
    });
    cy.contains('You have received an invite from your Healthcare Provider').should('exist');
})

When('I click create account button', () => {
    cy.get('#create-account-button').click();
})

When(`I provide an email: {string}`, (email: string) => {
    if(email == 'random'){
        const randomEmail = faker.internet.email();
        Cypress.env('randomEmail', randomEmail);
        cy.get('#Email').type(randomEmail);
        cy
    }else if(email != 'none'){
        cy.get('#Email').type(email);
    }
});


When(`I provide a password: {string}`, (password: string) => {
    cy.get('#Password').type(password);
});

When(`I confirm my password: {string}`, (confirmPassword: string) => {
    cy.get('#ConfirmPassword').type(confirmPassword);
});

When('I click sign up', () => {
    cy.get('#register-submit').click();
    cy.contains('Confirm your email').should('exist');
})

When('I visit the email verification link', () => {
    cy.getEmailContent('first').then((emailBody) => {
        cy.visit(emailBody.actionLink);
    });
})

When('My email is confirmed', () => {
    cy.contains('Email confirmed').should('exist');
})

When('I click the go to Sign in button', () => {
    cy.get('#continue-to-invite').click();
})

When('I provide my email', () => {
    cy.get('#login-email').type(Cypress.env('randomEmail'));
})

When(`I provide my password: {string}`, (password: string) => {
    cy.get('#login-password').type(password);
});

When('I click on the Sign in button', () => {
    cy.get('#login-button').click();
})

When('I select patient relationship: {string}', (relationship: string) => {
    cy.get('#person-relationship-type').select(relationship);
})

When('I provide patient last name: {string}', (lastName: string) => {
    cy.get('#InviteAcceptBySecurityDetailsDto_LastName').type(lastName);
})

When('I provide the patient DOB: {string}', (dob: string) => {
    cy.get('#date-of-birth').type(dob);
})

When('I click accept invitation', () => {
    cy.get('#submit-button').click();
})

Then('The patient is displayed: {string}', (patientName: string) => {
    cy.contains('Welcome to MeldRx').should('exist');
    cy.contains(patientName).should('exist');
})

When('I visit a different invite link', () => {
    cy.login();
    cy.visit(Cypress.env('inviteUrl'));
})

Then('I am shown an error message: {string}', (errorMessage: string) => {
    cy.contains(errorMessage).should('exist');
})

Given('I visit an invite link for a second patient', () => {
    cy.logout();
    cy.visit(Cypress.env('inviteUrl'));
    cy.get('#sign-in-button').click();
    cy.get('#login-email').type(Cypress.env('randomEmail'));
    cy.get('#login-password').type('Hunter1!');
    cy.get('#login-button').click();
    cy.contains('It looks like this account already has access to health records.').should('exist');
})

When('I select who to connect the patient to: {string}', (connection:string) => {
    cy.get('#person-type-dropdown').select(connection);
})

When('I am redirected to a patient context selection screen with both patients', () => {
    cy.contains('My People').should('exist');
    cy.contains('John Smith').should('exist');
    cy.contains('Jane Doe').should('exist');
})

When('I select a patient from the context selection: {string}', (patientName:string) => {
    cy.contains(patientName).click();
})

When('Context switcher for patients is visible', () => {
    cy.contains('2 People Available').should('exist');
})

When('Context switcher for organization is visible', () => {
    cy.contains('VIEWING DATA FOR ORGANIZATION').should('exist');
})

When('I switch the organization: {string}', (workspaceName: string) => {
    cy.get('#undefined-select').click();
    cy.get(`#${workspaceName.toLowerCase().replaceAll(' ', '-')}-option`).click();
})

When('I switch the patient: {string}', (patientName:string) => {
    cy.get('#switch-person-button').click();
})
Given('I visit an invite link for a second patient from a different workspace', () => {
    cy.logout();
    cy.visit(Cypress.env('inviteUrl2'));
    cy.get('#sign-in-button').click();
    cy.get('#login-email').type(Cypress.env('randomEmail'));
    cy.get('#login-password').type('Hunter1!');
    cy.get('#login-button').click();
    cy.contains('It looks like this account already has access to health records.').should('exist');
})
