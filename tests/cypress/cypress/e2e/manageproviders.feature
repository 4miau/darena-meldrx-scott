Feature: Manage workspace providers

    Scenario: I add a provider to my workspace
        Given I am on the providers page for my workspace
        When I click add provider
        And I provide an NPI: '1942366430'
        And I select an activation date: '2012-02-12'
        And I click add
        And My providers are added: '1ST PHARMACY CORP'
        And I go to the directories page
        And I search for my directory
        And My provider count is: '1'
        And I click on the details button
        Then My active providers are displayed: '1ST PHARMACY CORP'

    Scenario: I edit a provider
        Given I am on the providers page for my workspace
        When I click edit provider: '1942366430'
        And I edit the provider name: 'Edited Name'
        And I select an activation date: '2020-01-03'
        And I confirm edit
        And My provider is updated: 'Edited Name','01/03/2020'
        And I go to the directories page
        And I search for my directory
        And My provider count is: '1'
        And I click on the details button
        Then My active providers are displayed: 'Edited Name'

    Scenario: I deactivate a provider
        Given I am on the providers page for my workspace
        When I click deactivate provider: '1942366430'
        And I click confirm deactivate
        And My providers are deactivated: '1942366430'
        And I go to the directories page
        And I search for my directory
        And My provider count is: '0'
        And I click on the details button
        Then My deactivated providers are not displayed: 'Edited Name'

    Scenario Outline: I add providers to my workspace with wrong inputs
        Given I am on the providers page for my workspace
        When I click add provider
        And I provide an NPI: '<NPI>'
        And I select an activation date: '<ActivationDate>'
        And I click add
        Then I am shown a field validation error: '<ValidationError>'

        Examples:
            | NPI        | ActivationDate | ValidationError                       |
            |            | 2012-12-05     | NPI is required                       |
            | 123456789  | 2012-12-06     | at least one 10 digit NPI is required |
            | invalid    | 2012-12-06     | at least one 10 digit NPI is required |
            | 1942366430 | 2012-12-07     | NPI\'s 1942366430 already exists.     |

    Scenario: I reach my providers limit
        Given I am on the providers page for my workspace
        When I click add provider
        And I provide an NPI: '1821486630,1669612271,1700028081,1245446061'
        And I select an activation date: '2012-12-04'
        And I click add
        And My providers are added: '1821486630,1669612271,1700028081,1245446061'
        Then I reach my provider limit
