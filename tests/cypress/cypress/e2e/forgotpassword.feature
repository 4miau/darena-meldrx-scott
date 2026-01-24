@forgot-password
Feature: Handle a forgotten password

  Scenario: 0. Setup account
    Given I am logged in as a user

  Scenario: Return to previous page
    Given I am on the forgot password screen
    When I click on the back button
    Then I am on the login screen

  Scenario: Submit with valid email
    Given I am on the forgot password screen
    When I provide the email address: 'random'
    And I click on the Submit button
    Then I see a success alert

  Scenario: Submit with valid email (email count)
    Given I clear my inbox
    And I am on the forgot password screen
    When I provide the email address: 'random'
    And I click on the Submit button
    Then I have exactly one email in my inbox

  Scenario: Return to login after submission
    Given I am on the forgot password screen
    When I provide the email address: 'random'
    And I click on the Submit button
    And I click on the return to login button
    Then I am on the login screen

  Scenario: Submit with invalid email
    Given I am on the forgot password screen
    When I provide the email address: 'thisisnotanemail'
    And I click on the Submit button
    Then I am provided with an error message: 'The Email field is not a valid e-mail address.'

  Scenario: Submit with empty email
    Given I am on the forgot password screen
    When I provide the email address: ''
    And I click on the Submit button
    Then I am provided with an error message: 'The Email field is required.'

  Scenario: Navigation with incorrect or missing metadata
    Given I navigate to the Reset Password screen URL
    Then I am shown an error message: 'Sorry, there was an error'

  Scenario: Navigation to reset password screen
    Given I receive a Forgot Password email
    When I go to the link in the email
    Then I arrive at the Reset Password screen

  Scenario Outline: Incorrect form fields with server-side errors
    Given I receive a Forgot Password email
    When I go to the link in the email
    When I provide a password: '<Password>'
    And I provide a confirmation password: '<ConfirmPassword>'
    And I click on the reset button
    Then I am shown a server error which contains: '<ExpectedErrorContains>'

    Examples:
      | Password   | ConfirmPassword | Checkbox | ExpectedErrorContains         |
      | Password@  | Password@       | yes      | at least one digit            |
      | Password1  | Password1       | yes      | at least one non alphanumeric |
      | password1@ | password1@      | yes      | at least one uppercase        |
      | PASSWORD1@ | PASSWORD1@      | yes      | at least one lowercase        |
      | password1@ | password1@      | yes      | at least one uppercase        |
      | Pw1@       | Pw1@            | yes      | at least 8 characters         |

  Scenario Outline: Incorrect form fields with client-side errors
    Given I receive a Forgot Password email
    When I go to the link in the email
    And I provide a password: '<Password>'
    And I provide a confirmation password: '<ConfirmPassword>'
    And I click on the reset button
    Then I am shown errors on the '<FieldsWithError>' field

    Examples:
      | Password | ConfirmPassword | Checkbox | FieldsWithError |
      |          |                 | yes      | Password        |
      | Hunter2! |                 | yes      | ConfirmPassword |

  Scenario: Success Flow - Reset the password
    Given I receive a Forgot Password email
    When I go to the link in the email
    When I provide a password: 'random'
    And I provide a confirmation password: 'random'
    And I click on the reset button
    Then I am on the reset password confirmation screen

  Scenario: Success Flow - I can log in with new password
    Given I am on the login screen
    Then I can log in with new credentials
