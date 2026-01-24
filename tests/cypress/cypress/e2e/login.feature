Feature: Log in to the system

  Scenario: Attempt to log in with admin credentials
    Given I am on the login screen
    When I provide the email address: 'admin@meldrx.com'
    And I provide the password: 'MeldRx@Darena2022'
    And I click on the Login button
    Then I am logged in successfully as admin

  Scenario: Go to the user create account screen
    Given I am on the login screen
    When I click on the Create an account button
    Then I am taken to the Create Account screen

  Scenario Outline: Attempt to log in with improper credentials
    Given I am on the login screen
    When I provide the email address: '<email>'
    And I provide the password: '<pass>'
    And I click on the Login button
    Then I am provided an error message: 'Error: Invalid username or password'
    Examples:
      | email              | pass     |
      | invalid@meldrx.com | iNvalid! |
      | thisisnotanemail   | iNvalid! |

  Scenario Outline: Attempt to log in with empty fields
    Given I am on the login screen
    When I provide the email address: '<email>'
    And I provide the password: '<pass>'
    And I click on the Login button
    # we don't actually show a message for these, just the red outline
    Then I am shown a validation error: '<type>'
    Examples:
      | email            | pass | type  |
      |                  |      | both  |
      | admin@meldrx.com |      | pass  |
      |                  | test | email |

  Scenario: Click the forgot password button
    Given I am on the login screen
    When I click on the Forgot Password button
    Then I am taken to the Forgot Password screen
