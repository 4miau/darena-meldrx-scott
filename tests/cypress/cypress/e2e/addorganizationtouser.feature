Feature: Add an organization to a user as administrator

    Scenario: Add organization to existing user
        Given I am on the users page
        When I select an existing user
        And I click manage organizations
        And I select an organization to add 'Organization Number 2'
        And I select a role 'User'
        And I click assign
        Then The organization is added to the user

    Scenario: Add organization to user that they are already a part of
        Given I am on the users page
        When I select an existing user
        And I click manage organizations
        And I select an organization to add 'Organization Number 2'
        And I select a role 'User'
        And I click assign
        Then I am shown an error pop-up that contains 'User is already added for this organization'

    Scenario Outline: Add organization to user with wrong inputs
        Given I am on the users page
        When I select an existing user
        And I click manage organizations
        And I select an organization to add '<OrganizationName>'
        And I select a role '<Role>'
        And I click assign
        Then I am shown an error pop-up that contains '<ErrorMessage>'

        Examples:
            | OrganizationName      | Role  | ErrorMessage               |
            | Empty                 | Admin | Error!                     |
            | Organization Number 2 | Empty | The Role field is required |
