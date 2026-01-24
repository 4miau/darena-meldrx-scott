import { When, Then } from "@badeball/cypress-cucumber-preprocessor";

When("I visit the Swagger URL", () => {
  cy.visit("https://app.local.meldrx.com/swagger");
});

Then("The page should load without errors", () => {
  cy.request("https://app.local.meldrx.com/swagger").then((response) => {
    expect(response.status).to.eq(200);
  });

  cy.contains("MeldRx APIs").should("be.visible");
});
