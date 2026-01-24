import type { AiStreamResponseChunkType } from './AiStreamResponseChunkType';

export type AiStreamTextResponseChunk = {
    type: AiStreamResponseChunkType.Text;
    text: string;
};
