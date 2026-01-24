Feature: Create a Group

  Scenario: I create a new group for a Standalone workspace
    Given I am on the workspaces page
    When I click on the name of the standalone workspace
    And I am on the workspace manage page
    And I click on Groups
    And I am on the workspace groups page
    And I click the Create Group button
    And I provide a group name: 'random'
    And I click the Create Group modal button
    Then The group is created

  Scenario Outline: I attempt to create a new group with invalid data
    Given I go the workspace groups page
    And I click the Create Group button
    And I provide a group name: '<GroupName>'
    And I click the Create Group modal button
    Then I am shown a field validation error: '<ValidationError>'

    Examples:
    | GroupName | ValidationError            |
    | none      | Name is required           |
    | My Group  | Name cannot contain spaces |

  Scenario: I attempt to create a new group where group name already exists
    Given I go the workspace groups page
    And I click the Create Group button
    And I provide a group name: 'Test1'
    And I click the Create Group modal button
    And The group is created
    And I click the Create Group button
    And I provide a group name: 'Test1'
    And I click the Create Group modal button
    Then I am shown a pop-up error: 'Unable to create group'
