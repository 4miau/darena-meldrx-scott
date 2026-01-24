Feature: I login to MyMipsScore

  Scenario: Login to MMS
    Given I am on the MeldRx login screen
    And I provide the email address: 'admin@meldrx.com'
    And I provide the password: 'MeldRx@Darena2022'
    When I log into MeldRx and click MyMipsScore
    Then I am logged in successfully to MMS