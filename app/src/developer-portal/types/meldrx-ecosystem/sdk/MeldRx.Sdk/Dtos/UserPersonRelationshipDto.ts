import type { UserPersonRelationshipRole } from '../Enums/UserPersonRelationshipRole';
import type { UserPersonRelationshipType } from '../Enums/UserPersonRelationshipType';
import type { ApplicationUserDto } from './ApplicationUserDto';
import type { PersonDto } from './PersonDto';
import type { Guid } from '~/types/common/Guid';

export interface UserPersonRelationshipDto {
    id: Guid;
    userId: string;
    personId: Guid;
    role: UserPersonRelationshipRole;
    relationship: UserPersonRelationshipType;
    user: ApplicationUserDto;
    person: PersonDto;
}
