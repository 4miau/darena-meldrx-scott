Feature: Allow the user to logout

    Scenario: Logout on command center
        Given I am on the login screen
        When I login as a meldrx user
        And I am on the command center
        And I click the action menu button
        And I click the sign out button
        And I am logged out successfully
