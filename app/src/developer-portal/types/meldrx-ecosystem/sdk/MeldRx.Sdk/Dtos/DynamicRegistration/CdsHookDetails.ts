import type { CdsHookForm } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/DynamicRegistration/CdsHookForm';
import type { CardForm } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/DynamicRegistration/CardForm';

export interface CdsHookDetails {
  hook?: CdsHookForm;
  cards?: CardForm[];
}
