export enum InviteType
{
    Organization,
    Registration
}

export interface CreateInviteDto {
     inviteType: InviteType;
     isSynapseRole?: boolean;
     accessiblePatientId?: string;
     accessiblePatientIdentifierSearchStr?: string;
     userEmail?: string;
     userDisplayName?: string;
}

export interface SendInviteDto {
    email: string | null;
}

export interface InviteDto
{
     id:string,
     createdOn:Date,
     isSynapseRole:boolean,
     dateOfBirth:string,
     lastName:string,
     firstName:string,
     userDisplayName:string,
     invitationCode:string,
     userId:string,
     accessiblePatientId:string,
     acceptedOn:string,
     failedInviteAcceptCount:number
}

export interface InviteCreateResponseDto
{
     id:string,
     code:string,
     url:string
}
