Feature: Managing patient dashboard as Provider

    Scenario: I view the patient dashboard of Alice Newman
        Given I am on the workspace patients page and 'Alice Newman' is there
        And Patient dashboard page is displayed
        Then I select the Documents tab and can see patient documents
        And I select All Data tab and should see patient medical records
        And I select the Manage tab and see options relating to the patient

    Scenario Outline: I copy and revoke invite url for Alice Newman
        Given I am on the workspace patients page and 'Alice Newman' is there
        Then I select the Manage tab and see options relating to the patient
        Then I create and copy the invite url
        And I revoke the invite

    Scenario Outline: I add Alice Newman to a new group
        Given I am on the workspace patients page and 'Alice Newman' is there
        And I select the Manage tab and see options relating to the patient
        Then I create a new group
        And I add the patient to the group
        And I remove the patient from the group

    Scenario: I edit Alice Newman's patient data
        Given I am on the workspace patients page and 'Alice Newman' is there
        And Patient dashboard page is displayed
        Then I select the Manage tab and see options relating to the patient
        Then I edit the patient

    Scenario: I delete the updated patient data
        Given I am on the workspace patients page and 'Jane Doe' is there
        And Patient dashboard page is displayed
        And I select the Manage tab and see options relating to the patient
        Then I delete the patient and check the workspace page