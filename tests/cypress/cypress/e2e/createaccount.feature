Feature: Create a new account

  Scenario: Return to previous page
    Given I am on the create account screen
    When I click on the back button
    Then I am on the sign in screen

  Scenario: New account with valid credentials able to sign in
    Given I am on the create account screen
    When I provide an organization name: 'random'
    And I provide an email: 'random'
    And I provide a first name: 'random'
    And I provide a last name: 'random'
    And I provide a password: 'Hunter2!'
    And I provide a confirmation password: 'Hunter2!'
    And I click on the create account button
    And I am on the organization request sent screen
    And I receive a confirmation email
    And I go to the link in the email
    And I am on the email confirmation screen
    And I click on the return to sign in
    And I attempt to sign in with the new account
    And I am logged in
    And I click on the user menu
    Then My email and organization name appear in the menu

  Scenario Outline: New account with partial credentials (server validation)
    Given I am on the create account screen
    When I provide an organization name: 'random'
    And I provide an email: '<Email>'
    And I provide a first name: 'random'
    And I provide a last name: 'random'
    And I provide a password: '<Password>'
    And I provide a confirmation password: '<ConfirmPassword>'
    And I click on the create account button
    Then I am shown a server error which contains: '<ExpectedErrorContains>'

    Examples:
      | Email            | Password   | ConfirmPassword | ExpectedErrorContains         |
      | random           | Password@  | Password@       | at least one digit            |
      | random           | Password1  | Password1       | at least one non alphanumeric |
      | random           | password1@ | password1@      | at least one uppercase        |
      | random           | PASSWORD1@ | PASSWORD1@      | at least one lowercase        |
      | random           | password1@ | password1@      | at least one uppercase        |
      | random           | Pw1@       | Pw1@            | at least 8 characters         |
      | admin@meldrx.com | Hunter2!   | Hunter2!        | problem with this email       |

  Scenario Outline: New account with partial credentials (client validation)
    Given I am on the create account screen
    When I provide an organization name: 'random'
    And I provide an email: '<Email>'
    And I provide a first name: 'random'
    And I provide a last name: 'random'
    And I provide a password: '<Password>'
    And I provide a confirmation password: '<ConfirmPassword>'
    And I click on the create account button
    Then I am shown errors on the '<FieldsWithError>' field

    Examples:
      | Email            | Password | ConfirmPassword | FieldsWithError |
      | thisisnotanemail | Hunter2! | Hunter2!        | Email           |
      | random           |          |                 | Password        |
      | random           | Hunter2! |                 | ConfirmPassword |
