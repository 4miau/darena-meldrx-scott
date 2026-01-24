import { Given, Then } from '@badeball/cypress-cucumber-preprocessor'
import { faker } from '@faker-js/faker'
import 'cypress-file-upload'

before(() => {
    Cypress.session.clearAllSavedSessions()
    cy.createDeveloperUser()
    cy.login()
    cy.createDeveloperWorkspace('My Patient App Workspace').then((response) => {
        expect(response.status).to.eq(200)
		Cypress.env('wsSlug', response.body.fhirDatabaseDisplayName)
    })
    cy.logout()
})

beforeEach(() => {
    cy.session('login', () => {
        cy.login()
    })
})

Given('I have created an App: {string}', (appType: string) => {
    Cypress.env('appName', `A ${appType} App - ${faker.string.uuid()}`)

    if (appType === 'Patient' || appType === 'Provider') {
        cy.createDeveloperApp(Cypress.env('appName'), appType, 'meldrx-api', `https://app.local.meldrx.com/workspaces/My-${appType}-App/launch`)
            .then(response => expect(response.status).to.eq(200))
    } else {
        cy.createCdsHookApp(Cypress.env('appName'), 'https://sandbox-services.cds-hooks.org/cds-services/patient-greeting')
            .then(response => expect(response.status).to.eq(200))
    }

    cy.url().visit('https://app.local.meldrx.com/apps')
    cy.contains(Cypress.env('appName')).should('exist')
})

Then('I select the app', () => {
    cy.get(`#manage-${Cypress.env('appName').replaceAll(' ', '-').toLowerCase()}-button`).should('exist')
    cy.get(`#manage-${Cypress.env('appName').replaceAll(' ', '-').toLowerCase()}-button`).click()
})

Then('I fill in the app publish details: {string}', (appType: string) => {
    cy.contains('Publish').should('exist')
    cy.contains('Publish').click()
    cy.get('#published-button').click()

    cy.get('#app-description').type(`This is a ${appType} app`)
    cy.get('#publisher-url').type(`https://app.meldrx.com/My-${appType}-App/publisher`)
    cy.get('#terms-of-service').type(`https://app.meldrx.com/My-${appType}-App/terms`)
    cy.get('#privacy-policy').type(`https://app.meldrx.com/My-${appType}-App/privacy`)
})

Then('I check the DSI options for the app', () => {
    cy.get('#evidence-based-dsi-button').click()
    cy.contains('Bibliographic citation of the intervention').should('exist')
    cy.contains('Use of Race').should('exist')
    cy.contains('Use of gender identity').should('exist')
    cy.get('#save-attributes-button').click()
    cy.contains('Changes Captured! Click below to save.').should('exist')

    cy.get('#predictive-dsi-button').click()
    cy.contains('Source of development funding').should('exist')
    cy.contains('Intervention Type').should('exist')
    cy.contains('Intended use of the intervention').should('exist')
    cy.contains('Criteria for training data').should('exist')
    cy.contains('Frequency of performance corrections for validity and fairness risks').should('exist')
    cy.get('#save-attributes-button').click()
    cy.contains('Changes Captured! Click below to save.').should('exist')
})

Then('I save the app as published', () => {
    cy.get('#save-button').click()
})

Then('Check app has been published', () => {
    cy.url().visit(`https://app.local.meldrx.com/workspaces/${Cypress.env('wsSlug')}/extensions/activation`)
    cy.contains('Activate an Extension').should('exist')
    cy.contains('Public').click()
    cy.get('#next-step-button').click()

    cy.get('#extension-searchbar').type(Cypress.env('appName'))
    cy.get('#search-button').click()
})

Then('Activate the app as an extension', () => {
    cy.contains(Cypress.env('appName')).should('exist')
    cy.get(`#select-${Cypress.env('appName').replaceAll(' ', '-').toLowerCase()}-button`).click()
    cy.contains('Selected').should('exist')

    cy.get('#activate-extension-button').click()

    cy.contains('Active Extensions').should('exist')
    cy.contains(Cypress.env('appName')).should('exist')
    cy.contains('Predictive').should('exist')
    cy.contains('Modify Source Attributes').should('exist')
    cy.contains('Remove').should('exist')
})