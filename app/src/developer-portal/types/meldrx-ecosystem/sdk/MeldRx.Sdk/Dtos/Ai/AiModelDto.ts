export interface AiModelDto {
  name: string;
  publisher: string;
  isMultiModal: boolean;
}

export interface WorkspaceModelDto {
  id: string;
  modelUrl: string;
  modelToken: string;
  modelName: string;
  modelHost: string;
  apiKey: string,
  workspaceId: string;
  publisherName: string;
  isMultiModal: boolean;
  disableToolCalling: boolean;
}
export interface AddWorkspaceModel {
  modelUrl: string;
  modelName: string;
  apiKey: string,
  modelHost: string;
  publisherName: string;
  isMultiModal: boolean;
  disableToolCalling: boolean;
}
export interface UpdateWorkspaceModel {
  id: string;
  modelUrl: string;
  modelName: string;
  apiKey: string,
  modelHost: string;
  publisherName: string;
  isMultiModal: boolean;
  disableToolCalling: boolean;
}
