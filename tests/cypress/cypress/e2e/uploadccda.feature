Feature: Upload a CCDA file

  Scenario: I attempt to upload a CCDA
    Given I am viewing the patient list
    When I click on the import data button
    And I select a file for upload
    And I should see new patient
    And I click on the new patient
    And I click on the clinical document
    Then The CCDA viewer is visible
