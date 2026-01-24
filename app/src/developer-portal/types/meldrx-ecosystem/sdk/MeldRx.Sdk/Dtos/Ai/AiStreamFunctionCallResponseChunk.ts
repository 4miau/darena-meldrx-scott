import type { AiStreamResponseChunkType } from './AiStreamResponseChunkType';

export type AiStreamFunctionCallResponseChunk = {
    type: AiStreamResponseChunkType.Tools;
    name: string;
    arguments?: Record<string, unknown>;
};
