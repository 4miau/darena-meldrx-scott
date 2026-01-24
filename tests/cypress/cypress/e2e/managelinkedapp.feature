Feature: Manage linked app

  Scenario: I update my linked app on app creation screen
    Given I am on step 2 app creation page with an existing linked app
    When I click on my existing linked app: 'Epic Linked App'
    And I am shown the linked app 
    And I update connection name to: 'Epic Linked App Updated'
    And I update client id to: 'Epic Client Id Updated'
    And I select scopes: 'patient/*.read'
    And I try to save linked app
    And My linked app is saved: 'Epic Linked App Updated'
    And My linked app is updated: 'Epic Linked App Updated', 'Epic Client Id Updated', 'openid patient/*.read'
    Then I create the app

  Scenario: I update my linked app on app creation screen with incorrect inputs
    Given I am on step 2 app creation page with an existing linked app
    When I click on my existing linked app: 'Epic Linked App'
    And I am shown the linked app 
    And I select authentication method: 'Public'
    And I update connection name to: 'blank'
    And I update client id to: 'Epic Id'
    And I select scopes: 'openid patient/*.read'
    And I try to save linked app
    And I am shown an error message: 'Connection Name is required'
    And I click the cancel button
    And I click on my existing linked app: 'Epic Linked App'
    And I am shown the linked app 
    And I select authentication method: 'Public'
    And I update connection name to: 'My Linked App'
    And I update client id to: 'blank'
    And I select scopes: 'openid patient/*.read'
    And I try to save linked app
    And I am shown an error message: 'Client Id is required'
    And I click the cancel button
    And I click on my existing linked app: 'Epic Linked App'
    And I am shown the linked app 
    And I select authentication method: 'Public'
    And I update connection name to: 'My Linked App'
    And I update client id to: 'Epic Id'
    And I select scopes: 'none'
    And I try to save linked app
    Then I am shown an error message: 'Scopes are required'

  Scenario: I delete my linked app on app creation screen
    Given I am on step 2 app creation page with an existing linked app
    When I click to delete my existing linked app: 'epic-linked-app'
    And I click confirm deletion
    Then My linked app is deleted

  Scenario: I update my linked app on an existing app
    Given I am on the command center apps page
    When I click the manage button 'My App'
    And I am on the app manage page
    And I click on my existing linked app: 'Epic Linked App Updated'
    And I am shown the linked app 
    And I update connection name to: 'Epic Linked App Updated again'
    And I update client id to: 'Epic Client Id Updated'
    And I select linked app scopes: 'profile'
    And I try to save linked app
    And My linked app is saved: 'Epic Linked App Updated'
    Then My linked app is updated: 'Epic Linked App Updated again', 'Epic Client Id Updated', 'openid patient/*.read profile'

  Scenario: I update my linked app on an existing app with incorrect inputs
    Given I am on the command center apps page
    When I click the manage button 'My App'
    And I am on the app manage page
    And I click on my existing linked app: 'Epic Linked App Updated again'
    And I am shown the linked app
    And I update connection name to: 'blank'
    And I try to save linked app
    And I am shown an error message: 'Connection Name is required'
    And I click the cancel button
    And I click on my existing linked app: 'Epic Linked App Updated again'
    And I am shown the linked app
    And I update client id to: 'blank'
    And I try to save linked app
    And I am shown an error message: 'Client Id is required'
    And I click the cancel button
    And I click on my existing linked app: 'Epic Linked App Updated again'
    And I am shown the linked app
    And I select linked app scopes: 'none'
    And I try to save linked app
    Then I am shown an error message: 'Scopes are required'

  Scenario: I delete my linked app on an existing app
    Given I am on the command center apps page
    When I click the manage button 'My App'
    And I am on the app manage page
    And I click to delete my existing linked app: 'epic-linked-app-updated-again'
    And I click confirm deletion
    Then My linked app is deleted