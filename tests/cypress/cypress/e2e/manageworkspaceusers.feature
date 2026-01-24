Feature: Manage workspace users

    Scenario Outline: I add a user to a workspace with wrong inputs
      Given I am on the manage workspace users page
      When I click invite workspace users
      And I provide a first name: '<FirstName>'
      And I provide a last name: '<LastName>'
      And I provide an email: '<Email>'
      And I select a role '<Role>'
      And I click invite workspace user
      Then I am shown a validation error: '<ValidationError>'
      Examples:
        | FirstName | LastName | Email               | Role | ValidationError                      |
        | none      | Smith    | johnsmith@gmail.com | User | Please provide a first name          |
        | John      | none     | johnsmith@gmail.com | User | Please provide a last name           |
        | John      | Smith    | none                | User | Please provide an email address      |
        | John      | Smith    | johnsmith           | User | Please provide a valid email address |
        | John      | Smith    | johnsmith@gmail.com | none | Please select a role                 |
        | admin     | user     | admin@meldrx.com    | User | Unable to add user                   |

    Scenario: I add a user to a workspace
        Given I am on the manage workspace users page
        When I click invite workspace users
        And I provide a first name: 'random'
        And I provide a last name: 'random'
        And I provide an email: 'random'
        And I select a role 'User'
        And I click invite workspace user
        And My new workspace user is created
        And The invitation is sent
        And I go to the registration link
        And I provide my email
        And I provide a password: 'Hunter1!'
        And I confirm my password: 'Hunter1!'
        And I click register
        And My new workspace user is confirmed
        And I click on the return to sign in
        And I attempt to sign in with the new account and password: 'Hunter1!'
        And I am logged in
        And I click on the user menu
        Then My email appears in the menu

    Scenario: I delete a user from a workspace
      Given I am on the manage workspace users page
      When I click the remove button for an existing user
      And I click confirm deletion
      Then That user is deleted