Feature: Invite a patient to view their data
  
    Scenario: Accept patient invite and visit patient portal
      Given I am on the workspace patients page
      When I click on the patient action menu: 'John Smith'
      And I click send an invite to the patient action
      And I click the send invitation button
      And I log out as a provider
      And I go to the invitation link
      And I click create account button
      And I provide an email: 'random'
      And I provide a password: 'Hunter1!'
      And I confirm my password: 'Hunter1!'
      And I click sign up
      And I visit the email verification link
      And My email is confirmed
      And I click the go to Sign in button
      And I provide my email
      And I provide my password: 'Hunter1!'
      And I click on the Sign in button
      And I select patient relationship: 'Parent'
      And I provide patient last name: 'Smith'
      And I provide the patient DOB: '1980-01-01'
      And I click accept invitation
      Then The patient is displayed: 'John Smith'

  Scenario Outline: Accept patient invite with wrong inputs
    Given I visit a different invite link
    And I select patient relationship: 'Parent'
    And I provide patient last name: '<LastName>'
    And I provide the patient DOB: '<DoB>'
    And I click accept invitation
    Then I am shown an error message: '<ErrorMessage>'
    Examples:
      | LastName | DoB        | ErrorMessage                                         |
      | Doe      | 1980-01-01 | The invitation code and security answer do not match |
      | Smith    | 1970-12-01 | The invitation code and security answer do not match | 
    
    Scenario: Add second patient to same account
      Given I visit an invite link for a second patient
      When I select who to connect the patient to: 'Add a new person'
      And I select patient relationship: 'Parent'
      And I provide patient last name: 'Doe'
      And I provide the patient DOB: '1970-12-01'
      And I click accept invitation
      And I am redirected to a patient context selection screen with both patients
      And I select a patient from the context selection: 'Jane Doe'
      And The patient is displayed: 'Jane Doe'
      And Context switcher for patients is visible
      And I switch the patient: 'John Smith'
      And I select a patient from the context selection: 'John Smith'
      Then The patient is displayed: 'John Smith'
      
    Scenario: Add a patient to an existing patient
      Given I visit an invite link for a second patient from a different workspace
      When I select who to connect the patient to: 'John Smith'
      And I select patient relationship: 'Parent'
      And I provide patient last name: 'Smith'
      And I provide the patient DOB: '1980-01-01'
      And I click accept invitation
      And I am redirected to a patient context selection screen with both patients
      And I select a patient from the context selection: 'John Smith'
      And The patient is displayed: 'John Smith'
      And Context switcher for organization is visible
      And I switch the organization: 'Good Health Clinic'
      Then The patient is displayed: 'Johny Smith'
      