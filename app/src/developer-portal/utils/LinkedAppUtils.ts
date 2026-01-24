import type LinkedAppDto from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/DynamicRegistration/LinkedAppDto';
import type SoFAppBaseDto from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/DynamicRegistration/SoFAppBaseDto';
import { SoFAppTokenAuthMethod } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/DynamicRegistration/SoFAppTokenAuthMethod';
import { SecretType } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Enums/SecretType';
import type { INewLinkedApp, LinkedApp } from '~/types/ui/apps/NewLinkedApp';

export default class LinkedAppUtils {
    public static forLinkedApp(appClientId: string, linkedApp: LinkedApp): LinkedAppDto {
        const modifyCommand: LinkedAppDto = {
            id: linkedApp.id,
            meldRxClientId: appClientId,
            soFAppUserType: linkedApp.soFAppUserType,
            soFAppTokenAuthMethod: linkedApp.soFAppTokenAuthMethod ?? SoFAppTokenAuthMethod.Public,
            fhirApiProviderMeldRxIdentifier: linkedApp.fhirApiProviderMeldRxIdentifier!,
            clientName: linkedApp.clientName ?? '',
            clientId: linkedApp.clientId ?? '',
            clientSecret: linkedApp.clientSecret ?? '',
            scopes: linkedApp.scopes ?? '',
            jwksAlg: linkedApp.jwksAlg ?? '',
            jwksKid: linkedApp.jwksKid ?? '',
            secretType: linkedApp.secretType ?? SecretType.ClientSecret,
            isSharedCredentialType: linkedApp.isSharedCredentialType
        };

        return modifyCommand;
    }

    // Convert a SoFAppBaseDto to an INewLinkedApp...
    public static soFAppBaseDtoToNewLinkedApp(app: SoFAppBaseDto): INewLinkedApp {
        const ehr = EHRUtils.getEhrFromFhirApiProviderMeldRxId(app.fhirApiProviderMeldRxIdentifier);
        return { ...app, ehr };
    }

    // Convert a LinkedAppDto to an LinkedApp...
    public static linkedAppDtoToLinkedApp(app: LinkedAppDto): LinkedApp {
        const ehr = EHRUtils.getEhrFromFhirApiProviderMeldRxId(app.fhirApiProviderMeldRxIdentifier);
        return { ...app, ehr };
    }
}
