import type { ExternalIdp } from '../Enums/ExternalIdp';

export interface ApplicationUserDto {
    userName: string;
    createdAt: string;
    modifiedAt: string;
    id: string;
    firstName: string;
    middleName: string;
    lastName: string;
    email: string;
    npi: string;
    externalUserId: string;
    active: boolean;
    externalIdp: ExternalIdp | null;
    isSocialIdp: boolean;
    emailConfirmed: boolean;
    phoneNumber: string;
    phoneNumberConfirmed: boolean;
    lockoutEnabled: boolean;
    twoFactorEnabled: boolean;
    accessFailedCount: number;
    lockoutEnd: string | null;
}
