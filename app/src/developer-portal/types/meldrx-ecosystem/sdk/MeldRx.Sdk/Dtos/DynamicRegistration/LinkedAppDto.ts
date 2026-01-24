import type { Guid } from '~/types/common/Guid'
import type SoFAppBaseDto from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/DynamicRegistration/SoFAppBaseDto'

export default interface LinkedAppDto extends SoFAppBaseDto {
    id: Guid;
    meldRxClientId: string;
    isSharedCredentialType?: boolean;
}
