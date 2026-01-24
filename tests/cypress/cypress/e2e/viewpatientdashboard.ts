import { Given, Then } from '@badeball/cypress-cucumber-preprocessor'
import {faker} from "@faker-js/faker"
import 'cypress-file-upload'

before(() => {
	Cypress.session.clearAllSavedSessions()
	cy.clearInbox()
	cy.createDeveloperUser()
	cy.login()
	cy.createDeveloperWorkspace('My Patient App Workspace').then((response) => {
		expect(response.status).to.eq(200)
		Cypress.env('slug', response.body.fhirDatabaseDisplayName)

		cy.visit(`https://app.local.meldrx.com/workspaces/${Cypress.env('slug')}/patients`)
		cy.get('#import-data-button').click()
		cy.get('#patients-import-data-modal').should('not.be.empty')
		const fileName = 'AliceNewman_CCDA.xml'
		cy.get('input[type="file"]').attachFile(fileName)
		cy.get('#upload-button').click()
		cy.reload()

		cy.get('#alice-newman-actions').click()
		cy.contains('Send Invitation').click()
		cy.get('#email-address').type(Cypress.env('email'))
		cy.get('#send-invitation-button').click()
	})

	Cypress.env('email', faker.internet.email())
	Cypress.env('password', faker.internet.password({ prefix: '!1' }))

	cy.getEmailContent('first').then((emailBody) => { Cypress.env('inviteUrl', emailBody.inviteUrl) })
	cy.logout()
})

Given('I accept the patient invite as Alice Newman', () => {
	cy.visit(Cypress.env('inviteUrl'))
	cy.get('#create-account-button').click()

	cy.get('#Email').type(Cypress.env('email'))
	cy.get('#Password').type(Cypress.env('password'))
	cy.get('#ConfirmPassword').type(Cypress.env('password'))
	cy.get('#register-submit').click()

	cy.getEmailContent('first').then((emailBody) => { cy.visit(emailBody.actionLink) })
	cy.get('#continue-to-invite').click()
})

Then('I log in as the patient and have access to the patient dashboard', () => {
	cy.get('#login-email').type(Cypress.env('email'))
	cy.get('#login-password').type(Cypress.env('password'))
	cy.get('#login-button').click()
	cy.get('#InviteAcceptBySecurityDetailsDto_LastName').type('Newman')
	cy.get('#date-of-birth').type('1970-05-01')
	cy.get('#submit-button').click()

	cy.contains('Documents').should('exist')
	cy.contains('All Data').should('exist')
})

Then('I select the Documents tab and can see Alice Newman\'s documents', () => {
	cy.contains('Documents').click()

	const progressNote = 'Ms Alice Newman seems to be developing high fever and hence we recommend her to get admitted to a nearby hospital using the provided referral.'
	const procedureNote = 'Dr Albert Davis examined Ms Alice Newman and found that the pacemaker is operating properly and the breathlessness is due to high fever and anxiety.'
	const consultationNote = 'TDr Albert Davis diagnosed Ms Alice Newman to be suffering from Fever and suspected Pneumonia and recommended admission to the Community Health Hospitals'

	cy.contains('Progress Note').should('exist').click()
	cy.contains(progressNote).should('exist')

	cy.contains('Procedure Note').should('exist').click()
	cy.contains(procedureNote).should('exist')

	cy.contains('Consultation Note').should('exist').click()
	cy.contains(consultationNote).should('exist')

	cy.contains('All Data').click()
})

Then('I select All Data tab and should see Alice Newman\'s medical records', () => {
	cy.contains('Allergy').should('exist')
	cy.contains('Penicillin G benzathine').should('exist')

	cy.contains('Condition').should('exist')
	cy.contains('Fever').should('exist')

	cy.contains('table', 'Observation').should('exist')
})