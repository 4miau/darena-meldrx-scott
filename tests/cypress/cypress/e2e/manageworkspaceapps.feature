Feature: Manage workspace apps

    Scenario: I add an app to my workspace
        Given I am on the command center workspaces page
        When I click the manage button 'My Workspace'
        And I click system apps
        And I click add workspace app
        And I select an app to add: 'My App'
        And I select the role for my app: 'Administrator'
        And I add my selected workspace app
        Then My workspace app is added: 'My App'


    Scenario: I add an app to my workspace with wrong inputs
        Given I go the workspace system apps page
        And I click add workspace app
        And I select an app to add: 'none'
        And I select the role for my app: 'Administrator'
        And I add my selected workspace app
        And I am shown a validation error: 'Please select an app'
        And I click the cancel button
        And I click add workspace app
        And I select an app to add: 'My App'
        And I select the role for my app: 'none'
        And I add my selected workspace app
        And I am shown a validation error: 'Please select a role'
        And I click the cancel button
        And I click add workspace app
        And I select an app to add: 'My App'
        And I select the role for my app: 'Administrator'
        And I add my selected workspace app
        Then I am shown a server error: 'Permission already exists.'


    Scenario: I remove an app from my workspace
        Given I go the workspace system apps page
        And I remove my existing workspace app
        Then My workspace app is removed: 'My App'
