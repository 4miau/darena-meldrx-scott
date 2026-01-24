import { DynamicAuthMethods } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/DynamicRegistration/DynamicAuthMethods';
import { SoFAppTokenAuthMethod } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/DynamicRegistration/SoFAppTokenAuthMethod';
import { SoFAppUserType } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/DynamicRegistration/SoFAppUserType';

export default class AppUtils {
    // Returns a displayable string for the auth method...
    public static authMethodDisplayString (authMethod?: SoFAppTokenAuthMethod): string {
        switch (authMethod) {
            case SoFAppTokenAuthMethod.Public: return 'Public';
            case SoFAppTokenAuthMethod.ClientSecretPost: return 'Confidential';
            default: return 'Unknown';
        }
    }

    // Returns a displayable string for the user type...
    public static userTypeDisplayString (userType?: SoFAppUserType): string {
        switch (userType) {
            case SoFAppUserType.Provider: return 'Provider';
            case SoFAppUserType.Patient: return 'Patient';
            case SoFAppUserType.System: return 'System';
            default: return 'Unknown';
        }
    }

    // Returns a displayable string for the auth method...
    public static dynamicAuthMethodDisplayString(dynamicAuthMethods: DynamicAuthMethods): string {
        switch (dynamicAuthMethods) {
            case DynamicAuthMethods.None: return 'Public';
            case DynamicAuthMethods.ClientSecretPost: return 'Confidential';
            default: return 'Unknown';
        }
    }

    // Creating an app requires the auth method to be a string like this, which is different from the Enum string value...
    public static soFAppTokenAuthMethodToDynamicAuthMethods(authMethod: SoFAppTokenAuthMethod): DynamicAuthMethods {
        switch (authMethod) {
            case SoFAppTokenAuthMethod.ClientSecretPost: return DynamicAuthMethods.ClientSecretPost;
            case SoFAppTokenAuthMethod.Public: return DynamicAuthMethods.None;
            default: return DynamicAuthMethods.None;
        }
    }

    // Convert a "DynamicAuthMethods" to a "SoFAppTokenAuthMethod". In the C#, they use different strings
    public static dyanmicAuthMethodsToSoFAppTokenAuthMethod(authMethod: DynamicAuthMethods): SoFAppTokenAuthMethod {
        switch (authMethod) {
            case DynamicAuthMethods.ClientSecretPost: return SoFAppTokenAuthMethod.ClientSecretPost;
            case DynamicAuthMethods.None: return SoFAppTokenAuthMethod.Public;
            default: return SoFAppTokenAuthMethod.Public;
        }
    }

    // Returns the "Identity Resource" scopes. These are not allowed for "client_credentials" apps...
    public static getIdentityResourceScopes(): string[] {
        return ['roles', 'offline_access', 'openid', 'fhirUser', 'profile', 'email', 'address'];
    }
}
