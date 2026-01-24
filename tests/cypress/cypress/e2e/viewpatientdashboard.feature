Feature: Viewing patient dashboard as Patient

    Scenario: I view the patient dashboard of Alice Newman as the patient
        Given I accept the patient invite as Alice Newman
        Then I log in as the patient and have access to the patient dashboard
        Then I select the Documents tab and can see Alice Newman's documents
        Then I select All Data tab and should see Alice Newman's medical records