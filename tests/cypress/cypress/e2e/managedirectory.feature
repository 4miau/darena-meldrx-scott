Feature: Create and manage directories

    Scenario: I add a directory to my workspace
        Given I am on the directory page for my workspace
        When I click add directory
        And I provide a display name: 'random'
        And I provide a adressline1: '48th Street'
        And I provide a city: 'New York'
        And I provide a state: 'NY'
        And I provide a postal code: '753523131'
        And I click the Save button
        And My directory is created: 'random'
        And I go to the directories page
        And I search for my directory: 'random'
        Then My directory is displayed: 'random'
        
    Scenario: I edit my directory
        Given I am on the directory page for my workspace
        When I click edit directory
        And I provide a display name: 'random'
        And I provide a adressline1: '2nd Boulevard'
        And I provide a city: 'Los Angeles'
        And I provide a state: 'California'
        And I provide a postal code: 'CA'
        And I click the Save button
        And My directory is updated: 'random'
        And I go to the directories page
        And I search for my directory: 'random'
        Then My directory is displayed: 'random'

    Scenario: I set my directory visibility to hidden
        Given I am on the directory page for my workspace
        When I toggle the visibility of my directory
        And The visibility changes to: 'Hidden'
        And I go to the directories page
        And I search for my directory: 'random'
        Then My directory is not displayed: 'random'

    Scenario Outline: I add a directory to my workspace with wrong inputs
        Given I am on the directory page for my workspace
        When I click add directory
        And I provide a display name: '<DisplayName>'
        And I provide a adressline1: '<AddressLine1>'
        And I provide a city: '<City>'
        And I provide a state: '<State>'
        And I provide a postal code: '<PostalCode>'
        And I click the Save button
        Then I am shown a field validation error: '<ValidationError>'


        Examples:
            | DisplayName | AddressLine1 | City     | State | PostalCode | ValidationError          |
            | blank       | 1st street   | New York | NY    | 123456     | Display name is required |
            | My Hospital | blank        | New York | NY    | 123456     | Address is required      |
            | My Hospital | 1st street   | blank    | NY    | 123456     | City is required         |
            | My Hospital | 1st street   | New York | blank | 123456     | State is required        |
            | My Hospital | 1st street   | New York | NY    | blank      | Postal Code is required  |