import { Given, When } from '@badeball/cypress-cucumber-preprocessor';

before(()=> {
    cy.createDeveloperUser();
})

Given("I am on the login screen", () => {
    cy.visit("https://app.local.meldrx.com/Account/Login");
    cy.get('#create-account').should('have.text', 'Create an account')
});

When("I login as a meldrx user", () => {
    cy.login();
});

When(`I click the action menu button`, () => {
    cy.get("#action-menu-button").click();
});

When(`I am logged out successfully`, () => {
    cy.get("#action-menu-button").should('not.exist');
    cy.get('#my-personal-data-button').should('not.exist');
    cy.get('#change-password-button').should('not.exist');
    cy.get('#two-factor-authentication-button').should('not.exist');
});

When(`I am on the command center`, () => {
    cy.visit("https://app.local.meldrx.com/");
});

When(`I click the sign out button`, () => {
    cy.get('#sign-out-menu-item').click()
});
