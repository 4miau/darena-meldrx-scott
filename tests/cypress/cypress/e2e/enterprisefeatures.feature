Feature: Test enterprise features

    Scenario: I create a workspace with a custom slug
        Given I am on the workspace creation page
        And I select 'standalone' workspace type
        And I click the next button
        And I provide a workspace name: 'Enterprise Workspace'
        And I provide a workspace identifier
        And I provide a workspace slug
        And I click no workspace admin
        And I click create workspace
        And I am shown a workspace created modal
        And I click Go to Workspace button
        And I navigate to the workspace manage page
        Then My workspace slug is set correctly