import type { Group, Bundle, BundleEntry, FhirResource } from 'fhir/r4';
import type ResourceType from '~/types/fhir/ResourceType'

export default class FHIRUtils {
    // Takes a reference like "Patient/123" and gives you just the "123"...
    public static getIdFromReference(reference?: string): string {
        if (!reference) { return ''; }
        return reference.split('/')[1];
    }

    // Return how many members are in the group...
    public static getNumberOfMembersInGroup(group?: Group): number {
        if (!group) { return 0; }
        return group.member?.length || 0;
    }

    // Get all patient IDs as an array from the group...
    public static getPatientIdsFromGroup(group?: Group): string[] {
        if (!group) { return []; }
        return group.member?.map(member => this.getIdFromReference(member.entity.reference)) || [];
    }

    // Create a Group object in the MeldRx format...
    public static createGroupModel(name: string): Group {
        return {
            resourceType: 'Group',
            type: 'person',
            actual: true,
            name,
            identifier: [
                {
                    system: 'meldrx',
                    value: name
                }
            ]
        };
    }

    // Filter a bundle by a specific resource type...
    public static filterBundleByType<T>(bundle: Bundle | undefined, resourceType: ResourceType): T[] {
        if (!bundle) { return []; }
        if (!bundle.entry) { return []; }

        return bundle.entry
            .map((entry: BundleEntry) => entry.resource)
            .filter((resource?: FhirResource) => !!resource && resource.resourceType === resourceType) as T[];
    }

    // Returns true if the patient is already in the group...
    public static isPatientInGroup(group: Group, patientId: string): boolean {
        return group.member?.some(member => member.entity.reference === `Patient/${patientId}`) || false;
    }

    // Adds the patient to the given group and returns the group...
    public static addPatientToGroup(group: Group, patientId: string): Group {
        if (!group.member) { group.member = []; }

        // If patient is already present in the group, return the group as is...
        if (FHIRUtils.isPatientInGroup(group, patientId)) {
            return group;
        }

        // Add the patient to the group...
        const fullId = `Patient/${patientId}`;
        group.member.push({
            entity: {
                reference: fullId
            }
        });

        return group;
    }

    // Removes the patient from the given group and returns the group...
    public static removePatientFromGroup(group: Group, patientId: string): Group {
        if (!group.member) { group.member = []; }

        // Add the patient to the group...
        const fullId = `Patient/${patientId}`;
        group.member = group.member.filter(member => member.entity.reference !== fullId);

        if (group.member.length === 0) {
            group.member = undefined;
        }
        return group;
    }

    // Converts file contents to a bundle. File must just be a serialized bundle.
    public static createBundleFromFileContents(fileContents: string): Bundle {
        const o = JSON.parse(fileContents);
        return o as Bundle;
    }
}
