import type { AiStreamFunctionCallResponseChunk } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/Ai/AiStreamFunctionCallResponseChunk';
import type { AiStreamTextResponseChunk } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/Ai/AiStreamTextResponseChunk';
import type { AiStreamUsageResponseChunk } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/Ai/AiStreamUsageResponseChunk';

// We need this reader because of how responses are streamed back by the backend. We essentially want the caller
// of the LLM prompt endpoint to be able to process a single object at a time. The issue is that it is not guaranteed
// that the TextDecoderStream will read a single object at a time, it is possible it can have read multiple. It is
// determined by how fast the backend is writing to the stream.
//
// This reader is used as an intermediary where it will parse the response from the TextDecoderStream, and then
// parse a single object and yield it. This will allow the caller of this reader to process a single object at
// a time, even though the TextDecoderStream may have read more than one.
export class AiPromptReader {
    private readonly _responseStream: ReadableStream;
    constructor(responseStream: ReadableStream) {
        this._responseStream = responseStream;
    }

    async *read(): AsyncGenerator<
        | AiStreamTextResponseChunk
        | AiStreamUsageResponseChunk
        | AiStreamFunctionCallResponseChunk,
        void,
        unknown
        > {
        const reader = this._responseStream
            .pipeThrough(new TextDecoderStream())
            .getReader();

        let chunk: string = '';
        while (true) {
            const { value, done } = await reader.read();
            if (done) {
                break;
            }

            for (const c of value) {
                if (c !== '\n') {
                    chunk += c;
                    continue;
                }

                const obj = JSON.parse(chunk);
                chunk = '';

                yield obj;
            }
        }
    }
}
