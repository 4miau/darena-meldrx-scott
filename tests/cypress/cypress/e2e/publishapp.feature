Feature: Publishing an app

    Scenario: I publish a Patient App
        Given I have created an App: 'Patient'
        Then I select the app
        When I fill in the app publish details: 'Patient'
        Then I check the DSI options for the app
        Then I save the app as published
        Then Check app has been published
        Then Activate the app as an extension

    Scenario: I publish a Provider App
        Given I have created an App: 'Provider'
        Then I select the app
        When I fill in the app publish details: 'Provider'
        Then I check the DSI options for the app
        Then I save the app as published
        Then Check app has been published
        Then Activate the app as an extension

    Scenario: I publish a CdsHooks App
        Given I have created an App: 'CdsHooks'
        Then I select the app
        When I fill in the app publish details: 'CdsHooks'
        Then I check the DSI options for the app
        Then I save the app as published
        Then Check app has been published
        Then Activate the app as an extension