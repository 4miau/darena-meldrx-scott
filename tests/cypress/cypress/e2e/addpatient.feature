Feature: Add a Patient

  Scenario: I add a new patient on a Standalone workspace
    Given I am on the workspace patients page
    When I click the Add Patient button
    And I provide a first name: 'random'
    And I provide a last name: 'random'
    And I select a sex: 'Unknown'
    And I select a Date of birth before today: '1995-07-13'
    And I provide an email: 'random'
    And I click the Add Patient modal button
    Then The patient is added

  Scenario Outline: I add a new patient on a Standalone workspace with wrong inputs
    Given I am on the workspace patients page
    When I click the Add Patient button
    And I provide a first name: '<FirstName>'
    And I provide a last name: '<LastName>'
    And I select a sex: '<Sex>'
    And I select a Date of birth before today: '<DateOfBirth>'
    And I provide an email: '<Email>'
    And I click the Add Patient modal button
    Then I am shown a field validation error: '<ValidationError>'

    Examples:
      | FirstName | LastName | Email               | Sex  | DateOfBirth | ValidationError                      |
      | none      | Smith    | johnsmith@gmail.com | Male | 1995-07-13  | First Name is required               |
      | John      | none     | johnsmith@gmail.com | Male | 1995-07-13  | Last Name is required                |
      | John      | Smith    | johnsmith@gmail.com | Male | none        | Date of birth is required            |
      | John      | Smith    | johnsmith           | Male | 1995-07-13  | Please provide a valid email address |
