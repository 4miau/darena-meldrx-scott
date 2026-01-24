import { Given, When, Then } from "@badeball/cypress-cucumber-preprocessor";
import { faker } from '@faker-js/faker/locale/en';
import 'cypress-file-upload';

before(() => {
    Cypress.session.clearAllSavedSessions()
    cy.createEnterpriseOrganization();
    cy.login();
    cy.createEnterpriseWorkspace(faker.company.name(), faker.string.uuid(), faker.string.uuid())
    .then((response) => Cypress.env('workspaceSlug', response.body.workspaceDto.fhirDatabaseDisplayName));
});

beforeEach(() => {
    cy.session('login', () => {
        cy.login();
    });
});

Given("I am viewing the patient list", () => {
    cy.visit(`https://app.local.meldrx.com/workspaces/${Cypress.env('workspaceSlug')}/patients`);
    cy.title().should('eq', 'Patients | MeldRx');
});

When("I click on the import data button", () => {
    cy.get('#import-data-button').click();
    cy.get('#patients-import-data-modal').should('not.be.empty');
});

When("I select a file for upload", () => {
    const fileName = 'AliceNewman_CCDA.xml';
    cy.get('input[type="file"]').attachFile(fileName);
    cy.get('#upload-button').click();
});

When("I should see new patient", () => {
    cy.reload();
    cy.contains('Alice Newman').should('exist');
});

When("I click on the new patient", () => {
    cy.contains('a', 'Alice Newman').then(($a) => {
        const url = $a.prop('href');
        cy.visit(url);
      });
    cy.title().should('eq', 'Patient Chart | MeldRx');
});

When("I click on the clinical document", () => {
    cy.contains('Documents').click()
    cy.contains('span', 'Clinical Document').click();
});

Then(`The CCDA viewer is visible`, () => {
    cy.get('#viewcda').should('not.be.empty');
});