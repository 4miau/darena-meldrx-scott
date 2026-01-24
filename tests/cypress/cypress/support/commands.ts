import { faker } from '@faker-js/faker/locale/en';

Cypress.Commands.add("loginAsAdmin", () => {
  const adminEmail = 'admin@meldrx.com';
  const adminPassword = 'MeldRx@Darena2022';

  cy.visit('https://app.local.meldrx.com/Account/Login');
  cy.get('#login-email').type(adminEmail);
  cy.get('#login-password').type(adminPassword);
  cy.get('#login-button').click();
});

Cypress.Commands.add("createDeveloperUser", () => {
  const orgName = faker.company.name();
  const firstname = faker.person.firstName();
  const lastname = faker.person.lastName();
  const username = faker.internet.email();
  const password = faker.internet.password({ length: 20, prefix: '!1aA' });

  cy.visit('https://app.local.meldrx.com/OrganizationRequest?returnUrl=%2F');
  cy.get('input[name="__RequestVerificationToken"]').invoke('val').then((token)=> {
    cy.request({
      method: 'POST',
      url: '/OrganizationRequest',
      form: true,
      body:{
        OrganizationName: orgName,
        FirstName: firstname,
        LastName: lastname,
        Email: username,
        Password: password,
        ConfirmPassword: password,
        __RequestVerificationToken: token
      }
    }).then((response) => {
      expect(response.status).to.eq(200);
    });
  })

  Cypress.env('username', username);
  Cypress.env('password', password);
});

Cypress.Commands.add("createEnterpriseOrganization", () => {
  const orgName = faker.company.name();
  const firstname = faker.person.firstName();
  const lastname = faker.person.lastName();
  const username = faker.internet.email();
  const password = faker.internet.password({ length: 20, prefix: '!1aA' });

  cy.visit('https://app.local.meldrx.com/OrganizationRequest?returnUrl=%2F');
  cy.get('input[name="__RequestVerificationToken"]').invoke('val').then((token)=> {
    cy.request({
      method: 'POST',
      url: '/OrganizationRequest',
      form: true,
      body: {
        OrganizationName: orgName,
        FirstName: firstname,
        LastName: lastname,
        Email: username,
        Password: password,
        ConfirmPassword: password,
        __RequestVerificationToken: token
      }
    }).then((response) => {
      expect(response.status).to.eq(200);
    });
  });

  Cypress.env('username', username);
  Cypress.env('password', password);
  Cypress.env('orgName', orgName);

  cy.loginAsAdmin();

  cy.visit('https://app.local.meldrx.com/administrator/subscriptions')
  cy.get('#search-organization').type(Cypress.env('orgName')+ '{enter}');
  cy.get(`#edit-${Cypress.env('orgName').toLowerCase().replaceAll(' ','-').replaceAll(',','\\,')}-button`).click();
  cy.get('#subscription-type-select').click();
  cy.contains('enterprise').click({ force: true });
  cy.contains('Save').click({ force: true });

  cy.logout();
});

Cypress.Commands.add("login", () => {
  const username = Cypress.env('username');
  const password = Cypress.env('password');

  cy.visit('https://app.local.meldrx.com/Account/Login');
  cy.get('input[name="__RequestVerificationToken"]').invoke('val').then((token)=> {
    cy.request({
      method: 'POST',
      url: '/Account/Login',
      form:true,
      body: {
        username: username,
        password: password,
        returnUrl: '/',
        button: 'login',
        __RequestVerificationToken: token
      },
    }).then((response) => {
      expect(response.status).to.eq(200);
    });
  })
});

Cypress.Commands.add('apiRequest', (method, endpoint, requestBody) => {
  return cy.request({
    method: method,
    failOnStatusCode: false,
    url: endpoint,
    body: requestBody
  }).then((response) => {
    return response;
  })
});

Cypress.Commands.add('createDeveloperWorkspace',(workspaceName) => {
  cy.apiRequest(
      'POST',
      '/api/workspaces',
      {
        Name: workspaceName,
      }
  )
})

Cypress.Commands.add('createEnterpriseWorkspace',(workspaceName, workspaceSlug, workspaceIdentifier) => {
  cy.apiRequest(
      'POST',
      '/api/workspaces/organization',
      {
        workspaceName: workspaceName,
        email: null,
        firstName: null,
        lastName: null,
        organizationName: workspaceName,
        description: null,
        organizationIdentifier: workspaceIdentifier,
        password: null,
        confirmPassword: null,
        slug: workspaceSlug,
        validationOption: 'Enabled'
      }
  )
})

Cypress.Commands.add('createPatientInWorkspace',(workspaceSlug, givenName, familyName, dateOfBirth, gender, email?) => {
  cy.apiRequest(
      'POST',
      `/api/fhir/${workspaceSlug}/patient`,
      {
        "resourceType": "Patient",
        "name": [
          {
            "use": "usual",
            "family": familyName,
            "given": [
              givenName
            ]
          }
        ],
        "active": true,
        "birthDate": dateOfBirth,
        "gender": gender,
        "telecom": [
          {
            "system": "email",
            "value": email
          }
        ]
      }
  )
})

Cypress.Commands.add("logout", () => {
  cy.request({
    method: 'GET',
    url: '/Account/Logout',
  })
})

// Custom maildev commands - consider switching to https://github.com/Clebiez/cypress-maildev
Cypress.Commands.add('clearInbox', () => {
  cy.visit('https://sendgrid.local.meldrx.com');
  cy.contains('Now receiving all emails').should('exist');
  cy.get(':nth-child(3) > .header-nav-item-link').dblclick();
});

Cypress.Commands.add('countUnreadEmails', () => {
  cy.visit('https://sendgrid.local.meldrx.com');
  cy.get('.brand-unread').then(($span) => {
    const countText = $span.text();
    const count = parseInt(countText, 10);
    return count;
  });
});

Cypress.Commands.add('getEmailContent', (position = 'first') => {
  cy.visit('https://sendgrid.local.meldrx.com');
  let selector = '.email-item-link';
  if (position === 'first') {
    selector = `${selector}:first`;
  } else if (position === 'last') {
    selector = `${selector}:last`;
  } else if (typeof position === 'number') {
    selector = `.email-item-link:nth-child(${position})`;
  }

  return cy.get(selector).click().then(() => {
    cy.intercept('GET', 'https://sendgrid.local.meldrx.com/email/*').as('getEmailDetails');
    cy.get('.plain-text').should(($el) => {
      expect($el.text().trim()).not.to.be.empty;
    });

    return cy.get('.plain-text').invoke('text').then((emailContent) => {
      try {
        const emailJson = JSON.parse(emailContent);
        return emailJson;
      } catch (error) {
        cy.log('Failed to parse email content into JSON');
        throw error; // Ensure the error is not silently ignored
      }
    });
  });
});

Cypress.Commands.add('createDeveloperApp',(appName, appType, scope, ehrLaunchUrl = "", redirectUris?) => {
    cy.apiRequest(
        'POST',
        '/api/apps/batch',
        {
            "clientName": appName,
            "publisherUrl": "",
            "soFAppUserType": appType,
            "tokenEndpointAuthMethod": "client_secret_post",
            "ehrLaunchUrl": ehrLaunchUrl,
            "secretType": "ClientSecret",
            "jwksUri": "",
            "scope": scope,
            "redirectUris": redirectUris || [ `https://app.local.meldrx.com/My-${appType}-App/redirect` ],
            "postLogoutRedirectUris": [],
            "linkedApps": []
        }
    )
})

Cypress.Commands.add('createCdsHookApp', (name, cdsHookServiceUrl) => {
    const payload = new FormData()
    payload.append('Name', name)
    payload.append('CdsHookServiceUrl', cdsHookServiceUrl)

    cy.apiRequest('POST', '/api/apps/cds-hook', payload)
})