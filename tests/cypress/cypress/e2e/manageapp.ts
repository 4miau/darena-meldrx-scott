import { Given, When, Then, DataTable } from '@badeball/cypress-cucumber-preprocessor';

before(() => {
    Cypress.session.clearAllSavedSessions()
    cy.createDeveloperUser();
    cy.login();
    cy.apiRequest(
        'POST',
        '/api/apps/batch',
        {
            "clientName": "My App",
            "publisherUrl": "",
            "soFAppUserType": "Patient",
            "tokenEndpointAuthMethod": "none",
            "ehrLaunchUrl": "http://example.com",
            "secretType": "ClientSecret",
            "jwksUri": "",
            "scope": "openid",
            "redirectUris": [
                "http://example.com/callback"
            ],
            "postLogoutRedirectUris": [],
            "linkedApps": []
        }
    )
    cy.apiRequest(
        'POST',
        '/api/apps/batch',
        {
            "clientName": "My Confidential App",
            "publisherUrl": "",
            "soFAppUserType": "Patient",
            "tokenEndpointAuthMethod": "client_secret_post",
            "ehrLaunchUrl": "http://example.com",
            "secretType": "ClientSecret",
            "jwksUri": "",
            "scope": "openid",
            "redirectUris": [
                "http://example.com/callback"
            ],
            "postLogoutRedirectUris": [],
            "linkedApps": []
        }
    )

    cy.logout();
});

beforeEach(() => {
    cy.session('login', () => {
        cy.login();
    });
});

Given(`I am on the command center apps page`, () => {
    cy.url().visit('https://app.local.meldrx.com/apps');
});

When(`I click the manage button {string}`, (appName: string) => {
    cy.get(`#manage-${appName.toLowerCase().replaceAll(' ','-')}-button`).click();
});

When(`I am on the app manage page`, () => {
    cy.contains('Manage App').should('exist');
    cy.get('#client-id')
    .invoke('text')
    .then((clientId) => {
        Cypress.env('clientId', clientId);
    });
});

When(`I change the app name to: {string}`, (appName: string) => {
    if(appName == 'skip'){
        return
    }
    
    if(appName == 'blank'){
        cy.get('#app-name').clear({ force: true });
    }
    else{
        cy.get('#app-name').clear({ force: true });
        cy.get('#app-name').type(appName);
    }
});

When(`I change the app launch url to: {string}`, (appLaunchUrl: string) => {
    if(appLaunchUrl == 'skip'){
        return
    }
    
    cy.get('.grow > #ehr-launch-url').clear({ force: true });
    cy.get('.grow > #ehr-launch-url').type(appLaunchUrl);
});

When(`I change scopes: {string}`, (scopes: string) => {
    if(scopes == 'skip'){
        return
    }
    
    if(scopes == 'all'){
        cy.get('#select-all-button').click();
    }
    if(scopes == 'none'){
        cy.get('#select-all-button').click();
        cy.get('#deselect-all-button').click();
    }
    else{
        cy.get('#scopes-select').click();
        scopes.split(' ').forEach(scope => {cy.contains(scope).click()});
        cy.get('#scopes-select').click();
    }

    cy.contains('Provide the scopes required').click()
});

When(`I change redirect url to: {string}`, (redirectUrl: string) => {
    if(redirectUrl == 'skip'){
        return
    }
    
    if(redirectUrl == 'blank'){
        cy.get('#redirect-url-0 input').clear();
    }
    else if(redirectUrl == 'none'){
        cy.get('#x-button').click();
    }
    else{
        cy.get('#redirect-url-0 input').clear();
        cy.get('#redirect-url-0 input').type(redirectUrl);
    }
});

When(`I try to save app`, () => {
    cy.get(`#save-button`).click();
});

Then(`I am shown a validation error: {string}`, (validationString: string) => {
    cy.contains(validationString).should('exist');
});


Then(`My app is saved: {string}`, (appName: string) => {
    cy.url().visit('https://app.local.meldrx.com/apps');
    cy.contains(appName).should('exist');
    cy.contains(Cypress.env('clientId')).should('exist');
});

When(`I click the delete button`, () => {
    cy.get('#delete-app-button').click();
});

When(`I confirm deletion`, () => {
    cy.get('#delete-app-confirm-button').click();
});

Then(`My app is deleted`, () => {
    cy.url().visit('https://app.local.meldrx.com/apps');
    cy.contains(Cypress.env('clientId')).should('not.exist')
});

Then(`My app is updated: {string}, {string}, {string}, {string}`, (appName: string, appUrl: string, scopes: string, redirectUrl: string) => {
    cy.get(`#manage-${appName.toLowerCase().replaceAll(' ','-')}-button`).click();
    cy.contains('Manage App').should('exist');
    cy.get('.grow > #ehr-launch-url').should('have.value', appUrl);
    scopes.split(' ').forEach(scope => {
        cy.contains(scope).should('exist');
      });
    cy.get('#redirect-url-0 > :nth-child(1) > .flex-col > .flex > .grow > .border').should('have.value', redirectUrl);
});

When(`I delete my old client secret`, () => {
    cy.get('#delete-secret-0-button').click();
    cy.contains('Are you sure you want to delete this secret?').should('exist');
    cy.get('#delete-secret-confirm-button').click();
});

When(`I click add new secret`, () => {
    cy.get('#new-secret-button').click();
    cy.get('#new-client-secret')
    .should('exist')
    .invoke('val')
    .then((clientSecret:string) => {
        const last4Values = clientSecret.slice(-4);
        Cypress.env('clientSecret', last4Values);
    });
    cy.get('#close-button').click();
});

When(`I add a JWKS URI: {string}`, (jwksUri: string) => {
    cy.get('#jwks-uri').type(jwksUri);
});

When(`My client secret is updated {string}, {string}`, (appName: string, jwksUri: string) => {
    cy.intercept('GET', '/api/fhirapiprovider').as('loadApp')
    cy.get(`#manage-${appName.toLowerCase().replaceAll(' ','-')}-button`).click();
    cy.wait('@loadApp')
    cy.contains('Manage App').should('exist');
    cy.contains(Cypress.env('clientSecret')).should('exist');
    cy.get('#jwks-uri').should('have.value', jwksUri);
});
