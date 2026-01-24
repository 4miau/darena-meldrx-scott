import { Given, When, Then } from '@badeball/cypress-cucumber-preprocessor'

Given(`I am on the MeldRx login screen`, () => {
	cy.visit('https://app.local.meldrx.com/Account/Login')
})

When(`I provide the email address: {string}`, (email: string) => {
	const emailInput = cy.get('#login-email')
	if (email) {
		emailInput.type(email)
	} else {
		emailInput.clear()
	}
})

When(`I provide the password: {string}`, (password: string) => {
	const passwordInput = cy.get('#login-password')
	if (password) {
		passwordInput.type(password)
	} else {
		passwordInput.clear()
	}
})

When(`I log into MeldRx and click MyMipsScore`, () => {
	cy.get('#login-button').click()
	cy.contains('MyMipsScore').invoke('removeAttr', 'target').click()
	if (cy.get('.MuiDialog-container').should('exist')) {
		cy.contains('OK').click()
	}
})

Then(`I am logged in successfully to MMS`, () => {
	cy.url().should('contain', 'https://app.local.meldrx.com/mymipsscore/')
})
