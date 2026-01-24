import type { FhirRecordGrantDto } from './FhirRecordGrantDto'
import type { ApplicationUserDto } from './Dtos/ApplicationUserDto'
import type { PersonDto } from './Dtos/PersonDto'

export default interface FhirRecordGrantWithDetailsDto extends FhirRecordGrantDto {
    user: ApplicationUserDto;
    person: PersonDto;
    organization: any; // TODO: Type
    client: number;
}
