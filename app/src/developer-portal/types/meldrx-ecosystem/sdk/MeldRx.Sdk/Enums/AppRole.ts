export enum AppRole {
    Administrator = 'Administrator',
    Write = 'Write',
    Read = 'Read'
};

export const appRoleConfig = [
    { value: AppRole.Administrator, title: 'Administrator' },
    { value: AppRole.Write, title: 'Write' },
    { value: AppRole.Read, title: 'Read' }
];
