Feature: Manage a workspace

  Scenario: I update my standalone workspace
    Given I am on the command center workspaces page
    Then I head to the workspace manage page: 'My Standalone Workspace'
    And I change the workspace name to: 'blank'
    And I try to save workspace
    And I am shown a validation error: 'Workspace name is required'
    And I change the workspace name to: 'My Updated Standalone Workspace'
    And I try to save workspace
    Then My workspace is updated: 'My Updated Standalone Workspace'

  Scenario: I delete my standalone workspace
    Given I am on the command center workspaces page
    When I head to the workspace manage page: 'My Updated Standalone Workspace'
    And I delete the workspace
    Then My workspace is deleted

  Scenario: I update my linked workspace with correct inputs
    Given I am on the command center workspaces page
    When I head to the workspace manage page: 'My Linked Workspace'
    And I change the workspace name to: 'Updated Linked Workspace'
    And I change the fhir api url: 'https://fhir-ehr-code.cerner.com/r4/ec2458f2-1e24-41c8-b71b-0e701af7583d'
    And The fhir api url is validated
    And I try to save workspace
    Then My workspace is updated: 'Updated Linked Workspace'

  Scenario Outline: I update my linked workspace with wrong inputs
    Given I am on the command center workspaces page
    When I head to the workspace manage page: 'Updated Linked Workspace'
    And I change the workspace name to: '<WorkspaceName>'
    And I change the fhir api url: '<FhirApiUrl>'
    And I try to save workspace
    Then I am shown a validation error: '<ErrorMessage>'

    Examples:
      | WorkspaceName | FhirApiUrl                                                | ErrorMessage                                      |
      | four          | https://fhir.epic.com/interconnect-fhir-oauth/api/FHIR/R4 | Workspace name must be at least 5 characters long |
      | blank         | https://fhir.epic.com/interconnect-fhir-oauth/api/FHIR/R4 | Workspace name is required                        |
      | My Workspace  | https://wrong.fhir.url                                    | Please enter a valid FHIR API URL.                |

  Scenario: I delete my linked workspace
    Given I am on the command center workspaces page
    When I head to the workspace manage page: 'Updated Linked Workspace'
    And I delete the workspace
    Then My workspace is deleted
