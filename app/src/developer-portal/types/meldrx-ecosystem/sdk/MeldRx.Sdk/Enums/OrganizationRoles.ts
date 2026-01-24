export enum OrganizationRoles {
    Admin='Admin',
    User='User',
}

export const roleConfig = [
    { value: OrganizationRoles.Admin, title: 'Administrator' },
    { value: OrganizationRoles.User, title: 'User' }
];
