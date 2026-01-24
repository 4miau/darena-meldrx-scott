Feature: Manage a Group

  Scenario: I update a group name
    Given I am on the workspaces page
    When I click on the name of the standalone workspace
    And I am on the workspace manage page
    And I click on Groups
    And I am on the workspace groups page
    And I click on the Edit button: 'Group1'
    And I change the group name to: 'TestGroup'
    And I click the Edit Group modal button
    Then the group name is updated

  Scenario Outline: I update a group name with invalid data
    Given I go the workspace groups page
    And I click on the Edit button: 'Group2'
    And I provide a group name: '<GroupName>'
    And I click the Edit Group modal button
    Then I am shown a field validation error: '<ValidationError>'

    Examples:
    | GroupName | ValidationError            |
    | none      | Name is required           |
    | My Group  | Name cannot contain spaces |

  Scenario: I attempt to rename a group where group name already exists
    Given I go the workspace groups page
    And I click on the Edit button: 'Group2'
    And I change the group name to: 'TestGroup'
    And I click the Edit Group modal button
    Then I am shown a pop-up error: 'Unable to update group'

  Scenario: I add a patient to a group
    Given I am on the workspaces page
    When I click on the name of the standalone workspace
    And I am on the workspace manage page
    And I click on Patients
    And I am on the workspace patients page
    And I select actions Add to Group
    And I select group 'TestGroup'
    And I click the Add to Group modal button
    Then the user has been added to the group

  Scenario: I remove a patient from a group
    Given I go the workspace groups page
    And I click the name of the group
    And I am on the Manage Group page
    And I click the Remove button
    And I click the Remove Patient from Group modal button
    Then the patient is removed from the group

  Scenario: I delete a group
    Given I go the workspace groups page
    And I click the Delete button: 'TestGroup'
    And I click the Delete Group modal button
    Then the group is deleted