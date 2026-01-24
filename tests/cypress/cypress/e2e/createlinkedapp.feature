  Feature: Create a new app with linked apps

    Scenario Outline: I make a <AppName> linked app
      Given I am on the command center apps page
      When I click create app
      And I select user type: '<UserType>'
      And I click next step
      And I provide app name: '<AppName>'
      And I select auth type: '<AuthType>'
      And I select scopes: '<Scopes>'
      And I provide a redirect url: '<RedirectUrls>'
      And I click next step
      And I click create linked app
      And I select a linked app provider: 'epic'
      And I select to use my own credentials
      And I provide a connection name: 'epic linked app'
      And I provide a client id: 'epic client id'
      And I provide a client secret: 'epic <ClientSecret>'
      And I select scopes: 'profile'
      And I click add linked app
      And I close the linked app modal
      And My linked app is created: 'epic linked app'
      And I click register app
      Then My app is created

    Examples:
      | AppName                     | UserType    | AuthType      | Scopes     | RedirectUrls                  | ClientSecret    |
      | Patient App Public          | patient     | public        | all        | http://example.com/callback   | none            |
      | Provider App Public         | provider    | public        | all        | http://example.com/callback   | none            |
      | Patient App Confidential    | patient     | confidential  | all        | http://example.com/callback   | client secret   |
      | Provider App Confidential   | provider    | confidential  | all        | http://example.com/callback   | client secret   |
      | System App                  | system      | system        | all        | none                          | system          |
