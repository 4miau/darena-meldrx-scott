import { Given, When, Then } from "@badeball/cypress-cucumber-preprocessor";

Given("I am on the login screen", () => {
  cy.visit("https://app.local.meldrx.com/Account/Login");
  cy.get('#create-account').should('have.text', 'Create an account')
});

When("I click on the Create an account button", () => {
  cy.get('#create-account').click()
});

When("I provide the email address: {string}", (email: string) => {
  const emailInput = cy.get('#login-email');
  if (email) {
    emailInput.type(email);
  } else {
    emailInput.clear();
  }
});

When("I provide the password: {string}", (password: string) => {
  const passwordInput = cy.get('#login-password')
  if (password) {
    passwordInput.type(password);
  } else {
    passwordInput.clear();
  }
});

When("I click on the Login button", () => {
  cy.get('#login-button').click()
});

When("I click on the Forgot Password button", () => {
  cy.get('#forgot-password').click()
});

Then("I am logged in successfully as admin", () => {
  cy.url().should('eq', 'https://app.local.meldrx.com/administrator/workspaces');
});

Then("I am provided an error message: {string}", (error: string) => {
  cy.get('#error-label').should('have.text', 'Error: Invalid username or password')
});

Then("I am shown a validation error: {string}", (type: string) => {
  if (type === 'both' || type === 'email') {
    cy.get('#login-email-error').should('have.text', 'The Username field is required.')
  }
  if (type === 'both' || type === 'pass') {
    cy.get('#login-password-error').should('have.text', 'The Password field is required.')
  }
});

Then("I am taken to the Forgot Password screen", () => {
  cy.url().should('eq', 'https://app.local.meldrx.com/Account/ForgotPassword');
});

Then("I am taken to the Create Account screen", () => {
  cy.url().should('eq', 'https://app.local.meldrx.com/OrganizationRequest');
});
