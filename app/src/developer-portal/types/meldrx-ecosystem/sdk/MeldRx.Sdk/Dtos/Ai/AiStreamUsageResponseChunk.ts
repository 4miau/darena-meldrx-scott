import type { AiStreamResponseChunkType } from './AiStreamResponseChunkType';

export type AiStreamUsageResponseChunk = {
    type: AiStreamResponseChunkType.Usage;
    inputCount?: number;
    outputCount?: number;
    totalCount?: number;
};
