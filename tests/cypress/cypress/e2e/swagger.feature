Feature: Verify Swagger Endpoint Accessibility

  Scenario: Swagger page loads successfully
    When I visit the Swagger URL
    Then The page should load without errors