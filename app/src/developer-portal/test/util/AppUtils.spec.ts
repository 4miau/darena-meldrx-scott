import { describe, expect, test } from 'vitest';
import { DynamicAuthMethods } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/DynamicRegistration/DynamicAuthMethods';
import { SoFAppTokenAuthMethod } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/DynamicRegistration/SoFAppTokenAuthMethod';
import { SoFAppUserType } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/DynamicRegistration/SoFAppUserType';
import AppUtils from '~/utils/AppUtils';

describe('AppUtils', () => {
    test('authMethodDisplayString', () => {
        expect(AppUtils.authMethodDisplayString(SoFAppTokenAuthMethod.Public)).toEqual('Public');
        expect(AppUtils.authMethodDisplayString(SoFAppTokenAuthMethod.ClientSecretPost)).toEqual('Confidential');
        expect(AppUtils.authMethodDisplayString(undefined)).toEqual('Unknown');
    });

    test('userTypeDisplayString', () => {
        expect(AppUtils.userTypeDisplayString(SoFAppUserType.Provider)).toEqual('Provider');
        expect(AppUtils.userTypeDisplayString(SoFAppUserType.Patient)).toEqual('Patient');
        expect(AppUtils.userTypeDisplayString(SoFAppUserType.System)).toEqual('System');
        expect(AppUtils.userTypeDisplayString(undefined)).toEqual('Unknown');
    });

    test('soFAppTokenAuthMethodToDynamicRegistrationCreateDto', () => {
        expect(AppUtils.soFAppTokenAuthMethodToDynamicAuthMethods(SoFAppTokenAuthMethod.Public)).toEqual(DynamicAuthMethods.None);
        expect(AppUtils.soFAppTokenAuthMethodToDynamicAuthMethods(SoFAppTokenAuthMethod.ClientSecretPost)).toEqual(DynamicAuthMethods.ClientSecretPost);
    });
});
