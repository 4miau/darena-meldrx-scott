export interface WorkspaceOrganizationCommand {
    workspaceId: string 
    organizationId: string  | null
    resellerId: string  | null
}

export interface CreateWorkspaceForExistingOrganization {
    name: string
    slug: string
    organizationId: string
}