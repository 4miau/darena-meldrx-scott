import { Given, Then } from '@badeball/cypress-cucumber-preprocessor'
import {faker} from "@faker-js/faker"
import 'cypress-file-upload'

before(() => {
	Cypress.session.clearAllSavedSessions()
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
	})

	cy.logout()
})

beforeEach(() => {
	cy.session('login', () => { cy.login() })
})

Given('I am on the workspace patients page and {string} is there', (patientName: string) => {
    cy.visit(`https://app.local.meldrx.com/workspaces/${Cypress.env('slug')}/patients`)

    cy.contains(patientName)
        .invoke('removeAttr', 'target')
        .click()
})

Then('Patient dashboard page is displayed', () => {
	cy.contains('Documents').should('exist')
	cy.contains('All Data').should('exist')
	cy.contains('Manage').should('exist')
})

Then('I select the Documents tab and can see patient documents', () => {
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

Then('I select All Data tab and should see patient medical records', () => {
	cy.contains('Allergy').should('exist')
	cy.contains('Penicillin G benzathine').should('exist')

	cy.contains('Condition').should('exist')
	cy.contains('Fever').should('exist')

	cy.contains('table', 'Observation').should('exist')
})

Then('I select the Manage tab and see options relating to the patient', () => {
	cy.contains('Manage').click()
	cy.contains('Edit Patient').should('exist')
	cy.contains('Delete Patient').should('exist')
})

Then('I create and copy the invite url', () => {
	cy.get('#share-data-with-patient-button').click()
	cy.get('#email-address').type(faker.internet.email())
	cy.get('#send-invitation-button').realClick()

	cy.get('#copy-invite-url-button').realClick()
	cy.contains('Copied URL to clipboard.').should('exist')

	const inviteUrl = cy.window().then((win) => win.navigator.clipboard.readText())
	inviteUrl.and('contain', 'https://app.local.meldrx.com/invite/code/')
})

Then('I revoke the invite', () => {
	cy.get('#revoke-invite-button').click()
	cy.contains('The patient invitation has been revoked.').should('exist')
	cy.contains('Share Data With Patient').should('exist')
	cy.contains('Request Data From Patient').should('exist')
})

Then('I create a new group', () => {
	cy.get('#add-to-group-button').click()
	cy.get('#name').type('Patient-Group-1')
	cy.get('#create-group-modal-button').click()
})

Then('I add the patient to the group', () => {
	cy.get('#add-to-group-button').click()
	cy.get('#group-name-select').click()
	cy.get('#patient-group-1-option').click()
	cy.get('.w-full > #add-to-group-button').click({ force: true })
	cy.get('.gap-2 > .relative').contains('Patient-Group-1').should('exist')
})

Then('I remove the patient from the group', () => {
	cy.get('#remove-button').click()
	cy.get('#remove-patient-confirm-button').click()
	cy.contains('Patient has been removed from group Patient-Group-1.').should('exist')
	cy.contains('None').should('exist')
})

Then('I edit the patient', () => {
	cy.contains('Edit Patient').click()

	cy.get('#first-name').clear().type('Jane')
	cy.get('#last-name').clear().type('Doe')

	cy.get('#sex-select').click()
	cy.get('#unknown-option').click()

	cy.get('#date-picker').clear().type('1970-12-12')
	cy.get('#email-address').type(faker.internet.email())

	cy.get('#save-changes-button').click()
	cy.reload()

	cy.contains('Jane Doe', { matchCase: false }).should('exist')
	cy.contains('Unknown', { matchCase: false }).should('exist')
	cy.contains('12/12/1970').should('exist')
})

Then('I delete the patient and check the workspace page', () => {
	cy.get('#delete-patient-button').click()
	cy.get('#delete-patient-confirm-button').click()

	cy.contains('The patient has been deleted.').should('exist')
	cy.visit(`https://app.local.meldrx.com/workspaces/${Cypress.env('slug')}/patients`)
	cy.contains('No Patients').should('exist')
})