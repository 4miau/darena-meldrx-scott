Feature: Allow users to change their password

  Scenario Outline: 1. Missing and Mismatching fields
    Given I am on the change password screen
    When I provide the old password '<OldPassword>'
    And I provide the new password '<NewPassword>'
    And I provide the confirmation password '<ConfirmPassword>'
    And I click the update password button
    Then I am shown an error on the '<FieldWithError>' field

    Examples:
      | OldPassword | NewPassword | ConfirmPassword | FieldWithError  |
      |             | Hunter2!    | Hunter2!        | OldPassword     |
      | random      |             | Hunter2!        | ConfirmPassword |
      | random      |             |                 | NewPassword     |

  Scenario Outline: 2. Invalid passwords
    Given I am on the change password screen
    When I provide the old password '<OldPassword>'
    And I provide the new password '<NewPassword>'
    And I provide the confirmation password '<ConfirmPassword>'
    And I click the update password button
    Then I am shown a validation error that contains '<ExpectedErrorContains>'

    Examples:
      | OldPassword | NewPassword | ConfirmPassword | ExpectedErrorContains         |
      | bad         | Hunter2!    | Hunter2!        | incorrect password            |
      | random      | Password@   | Password@       | at least one digit            |
      | random      | Password1   | Password1       | at least one non alphanumeric |
      | random      | password1@  | password1@      | at least one uppercase        |
      | random      | PASSWORD1@  | PASSWORD1@      | at least one lowercase        |
      | random      | Pw1@        | Pw1@            | at least 8 characters         |

  Scenario: 3. Update password correctly
    Given I am on the change password screen
    When I provide the old password 'random'
    And I provide the new password 'Hunter2!'
    And I provide the confirmation password 'Hunter2!'
    And I click the update password button
        # !Warning! this "Then" has hidden logic behind the scenes because it has to tell
        # SharedAuthentication that the password it defaults to is no longer correct
    Then I am shown a success alert after updating my password

  Scenario: 4. Attempt to log in with new password
        # the background means that we are already "logged in" - just assert home screen
    Given I am on the login screen
    When I provide my email address
    And I provide the new password
    And I click on the Login button
    Then I am logged in successfully
