import type {PublishedStatus} from "~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/PublishedApps/PublishedAppDto";

export interface AdminAppUpdateCommand
{
  appId: string;
  publishedStatus: PublishedStatus;
  meldRxVerified?: boolean;
  meldRxHosted?:boolean;
}