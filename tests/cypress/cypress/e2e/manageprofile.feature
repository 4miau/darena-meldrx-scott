Feature: Manage profile

  Scenario: I update my profile
    Given I am logged in
    When I click the profile image in the header
    And I click Account Settings from the header menu
    And I am on the Account settings page
    And I click Edit my profile
    And I am on the My profile page
    And I provide a First name: 'Jane'
    And I provide a Last name: 'Doe'
    And I click the Save button
    Then my profile is updated

  Scenario Outline: I update my profile with wrong inputs
    Given I am logged in
    When I click the profile image in the header
    And I click Account Settings from the header menu
    And I am on the Account settings page
    And I click Edit my profile
    And I am on the My profile page
    And I provide a First name: '<FirstName>'
    And I provide a Last name: '<LastName>'
    And I click the Save button
    Then I am shown a field validation error: '<ValidationError>'

    Examples:
      | FirstName | LastName | ValidationError                   |
      | none      | Smith    | The First Name field is required. |
      | John      | none     | The Last Name field is required.  |

  Scenario: I return to account settings from my profile
    Given I am logged in
    When I click the profile image in the header
    And I click Account Settings from the header menu
    And I am on the Account settings page
    And I click Edit my profile
    And I am on the My profile page
    And I click Return to account settings
    Then I am taken to the account settings page

  Scenario Outline: I delete my account with wrong inputs
    Given I am logged in
    When I click the profile image in the header
    And I click Account Settings from the header menu
    And I am on the Account settings page
    And I click Edit my profile
    And I am on the My profile page
    And I click Delete my account
    And I am on the Delete my account page
    And I provide the password: '<Password>'
    And I click on the Delete data and close my acccount button
    Then I am shown a validation error: '<ValidationError>'

        Examples:
      | Password   | ValidationError                 |
      | none       | The Password field is required. |
      | 1ncorreCt! | Password not correct.           |

  Scenario: I delete my account
    Given I am logged in
    When I click the profile image in the header
    And I click Account Settings from the header menu
    And I am on the Account settings page
    And I click Edit my profile
    And I am on the My profile page
    And I click Delete my account
    And I am on the Delete my account page
    And I provide the password: 'random'
    And I click on the Delete data and close my acccount button
    And I am taken to the login screen
    And I try to log in with my old details
    Then I am shown a field validation error: 'Error: Invalid username or password'