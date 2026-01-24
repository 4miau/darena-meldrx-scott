Feature: Create a new workspace

  Scenario: I make a standalone workspace
    Given I am on the command center workspaces page
    When I click the create workspace button
    And I select 'standalone' workspace type
    And I click the next button
    And I provide a workspace name: 'Standalone Workspace'
    And I click create workspace
    And I am shown a workspace created modal
    And I click Go to Workspace button
    Then My workspace is accessible

  Scenario: I make a linked workspace
    Given I am on the command center workspaces page
    When I click the create workspace button
    And I select 'linked' workspace type
    And I click the next button
    And I provide a workspace name: 'Linked Workspace'
    And I select a fhir provider: 'epic'
    And I provide a fhir api url: 'https://fhir.epic.com/interconnect-fhir-oauth/api/FHIR/R4'
    And The fhir api url is validated
    And I click create workspace
    And I am shown a workspace created modal
    And I click Go to Workspace button
    Then My workspace is accessible

  Scenario Outline: I make a standalone workspace with wrong inputs
    Given I am on the command center workspaces page
    When I click the create workspace button
    And I select 'standalone' workspace type
    And I click the next button
    And I provide a workspace name: '<WorkspaceName>'
    And I click create workspace
    Then I am shown a validation error: '<ErrorMessage>'

    Examples:
      | WorkspaceName | ErrorMessage                                      |
      | blank         | Workspace name is required                        |
      | four          | Workspace name must be at least 5 characters long |

  Scenario Outline: I make a linked workspace with wrong inputs
    Given I am on the command center workspaces page
    When I click the create workspace button
    And I select 'linked' workspace type
    And I click the next button
    And I provide a workspace name: '<WorkspaceName>'
    And I select a fhir provider: '<FhirProvider>'
    And I provide a fhir api url: '<FhirApiUrl>'
    And I click create workspace
    Then I am shown a validation error: '<ErrorMessage>'

  Examples:
    | WorkspaceName | FhirProvider | FhirApiUrl                                                | ErrorMessage                                      |
    | blank         | epic         | https://fhir.epic.com/interconnect-fhir-oauth/api/FHIR/R4 | Workspace name is required                        |
    | four          | epic         | https://fhir.epic.com/interconnect-fhir-oauth/api/FHIR/R4 | Workspace name must be at least 5 characters long |
    | My Workspace  | none         | https://fhir.epic.com/interconnect-fhir-oauth/api/FHIR/R4 | FHIR API Provider is required                     |
    | My Workspace  | epic         | https://wrong.fhir.url                                    | Please enter a valid FHIR API URL.                |

  Scenario: I make a 5th workspace to test the limit
    Given I create one more workspace
    When I create one more workspace
    And I am on the command center workspaces page
    And I click the create workspace button
    And I select 'standalone' workspace type
    And I click the next button
    And I provide a workspace name: '5th Workspace'
    And I click create workspace
    And I am shown a workspace created modal
    And I click Go to Workspace button
    And My workspace is accessible
    And I return to workspaces screen
    Then I am not able to create more workspaces
