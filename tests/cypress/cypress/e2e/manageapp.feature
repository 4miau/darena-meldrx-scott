Feature: Manage an app

  Scenario: I update my app with correct inputs
    Given I am on the command center apps page
    When I click the manage button 'My App'
    And I am on the app manage page
    And I change the app name to: 'My Updated App'
    And I change the app launch url to: 'http://updated.com'
    And I change scopes: 'profile patient/*.read'
    And I change redirect url to: 'http://updated.com/callback'
    And I try to save app
    And My app is saved: 'My Updated App'
    Then My app is updated: 'My Updated App', 'http://updated.com', 'profile patient/*.read', 'http://updated.com/callback'

  Scenario Outline: I update my app with wrong inputs
    Given I am on the command center apps page
    When I click the manage button 'My Updated App'
    And I am on the app manage page
    And I change the app name to: '<AppName>'
    And I change the app launch url to: '<AppLaunchURL>'
    And I change scopes: '<Scopes>'
    And I change redirect url to: '<RedirectUrls>'
    And I try to save app
    Then I am shown a validation error: '<ErrorMessage>'

    Examples:
      | AppName | AppLaunchURL | Scopes | RedirectUrls | ErrorMessage                          |
      | blank   | skip         | skip   | skip         | App Name is required                  |
      | skip    | wrongurl     | skip   | skip         | Invalid URL schema/protcol            |
      | skip    | skip         | none   | skip         | Scopes are required                   |
      | skip    | skip         | skip   | none         | At least one Redirect URL is required |
      | skip    | skip         | skip   | blank        | URL cannot be empty                   |

  Scenario: I update my confidential app client secret
    Given I am on the command center apps page
    When I click the manage button 'My Confidential App'
    And I am on the app manage page
    And I change the app name to: 'My Updated Confidential App'
    And I delete my old client secret
    And I click add new secret
    And I add a JWKS URI: 'https://myjwks.com'
    And I try to save app
    And My app is saved: 'My Updated Confidential App'
    Then My client secret is updated 'My Updated Confidential App', 'https://myjwks.com'

  Scenario: I delete my app
    Given I am on the command center apps page
    When I click the manage button 'My Updated App'
    And I am on the app manage page
    And I click the delete button
    And I confirm deletion
    Then My app is deleted