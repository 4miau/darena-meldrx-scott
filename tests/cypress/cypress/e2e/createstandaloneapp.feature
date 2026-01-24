Feature: Create a new app

  Scenario Outline: I make a <AppName>
    Given I am on the command center apps page
    When I click create app
    And I select user type: '<UserType>'
    And I click next step
    And I provide app name: '<AppName>'
    And I provide app launch URL: '<AppLaunchURL>'
    And I select auth type: '<AuthType>'
    And I select scopes: '<Scopes>'
    And I provide a redirect url: '<RedirectUrls>'
    And I click next step
    And I click register app
    Then My app is created: '<AuthType>'

    Examples:
      | AppName                   | AppLaunchURL       | UserType | AuthType     | Scopes | RedirectUrls                |
      | Public Patient App        | http://example.com | patient  | public       | all    | http://example.com/callback |
      | Public Provider App       | http://example.com | provider | public       | all    | http://example.com/callback |
      | Confidential Patient App  | http://example.com | patient  | confidential | all    | http://example.com/callback |
      | Confidential Provider App | http://example.com | provider | confidential | all    | http://example.com/callback |
      | System App                | none               | system   | system       | all    | none                        |
