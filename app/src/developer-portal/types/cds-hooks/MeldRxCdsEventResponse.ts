import type { CdsServiceResponse } from '~/types/cds-hooks/CDSCards';

export type MeldRxCdsEventResponse = {
  success: { [key: string]: CdsServiceResponse },
  error: { [key: string]: string },
}
