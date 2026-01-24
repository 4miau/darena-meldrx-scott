Feature: Manage a team member

  Scenario Outline: I invite a team member with wrong inputs when inviting team member
    Given I am logged in on the team members page
    And I click the Invite Team Member button
    And I provide a first name: '<FirstName>'
    And I provide a last name: '<LastName>'
    And I provide an email: '<Email>'
    And I select a role '<Role>'
    And I click the Invite Team Member modal button
    Then I am shown a field validation error: '<ValidationError>'

    Examples:
      | FirstName | LastName | Email               | Role | ValidationError                      |
      | none      | Smith    | johnsmith@gmail.com | User | Please provide a first name          |
      | John      | none     | johnsmith@gmail.com | User | Please provide a last name           |
      | John      | Smith    | none                | User | Please provide an email address      |
      | John      | Smith    | johnsmith           | User | Please provide a valid email address |
      | John      | Smith    | johnsmith@gmail.com | none | Please select a role                 |
      | admin     | user     | admin@meldrx.com    | User | Unable to add user                   |

  Scenario: I invite a team member
    Given I am logged in
    When I click on Settings
    And I click on Team Members
    And I am on the Team Members page
    And I click the Invite Team Member button
    And I provide a first name: 'random'
    And I provide a last name: 'random'
    And I provide an email: 'random'
    And I select a role 'User'
    And I click the Invite Team Member modal button
    And The new team member is created
    And The invitation is sent
    And I go to the registration link
    And I provide my email: 'random'
    And I provide a password: 'Hunter1!'
    And I provide a confirmation password: 'Hunter1!'
    And I click Complete registration
    Then The team member is confirmed

  Scenario: I delete a user as a team member
    Given I am logged in on the team members page
    And I click the Remove button for an existing team member
    And I click confirm deletion
    Then The user is deleted

  Scenario: I delete the user I am logged in as a team member
    Given I am logged in on the team members page
    And I click the Remove button for the team member I am logged in as
    And I click confirm deletion
    Then I am shown an error: 'You can not remove yourself from an organization'
    
  Scenario Outline: I invite a team member with wrong inputs when completing registration
    Given I go to the registration link logged out
    And I provide my email: '<Email>'
    And I provide a password: '<Password>'
    And I provide a confirmation password: '<ConfirmPassword>'
    And I click Complete registration
    Then I am shown a complete registration error which contains: '<ValidationError>'

    Examples:
      | Email            | Password   | ConfirmPassword | ValidationError                      |
      | none             | Hunter2!   | Hunter2!        | field is required                    |
      | test@meldrx.com  | none       | Hunter2!        | do not match                         |
      | test@meldrx.com  | Hunter2!   | none            | field is required                    |
      | admin@meldrx.com | Hunter2!   | Hunter2!        | Email does not match the invitation. |
      | test@meldrx.com  | Password@  | Password@       | at least one digit                   |
      | test@meldrx.com  | Password1  | Password1       | at least one special character       |
      | test@meldrx.com  | password1@ | password1@      | at least one uppercase               |
      | test@meldrx.com  | PASSWORD1@ | PASSWORD1@      | at least one lowercase               |
      | test@meldrx.com  | Pw1@       | Pw1@            | at least 8 characters                |