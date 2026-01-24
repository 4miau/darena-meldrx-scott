import type { CdsHookForm } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/DynamicRegistration/CdsHookForm';
import type { CardForm } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/DynamicRegistration/CardForm';

export interface CreateCdsHooksAppCommand {
    name?: string;
    cdsHookServiceUrl?: string;
    cdsHook?: CdsHookForm;
    cards?: CardForm[];
    elmFiles?: FileList;
    cqlEditorArtifact?: any;
}

