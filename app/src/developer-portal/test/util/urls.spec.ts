import type { RuntimeConfig } from 'nuxt/schema';
import { vi, describe, expect, test } from 'vitest';
import urls from '~/utils/urls';

describe('utils/urls.createUrlPath', () => {
    test('createUrlPath handles leading & trailing slashes', () => {
        expect(urls.createUrlPath('first', 'second')).toEqual('/first/second');

        expect(urls.createUrlPath('/first', 'second')).toEqual('/first/second');
        expect(urls.createUrlPath('first', '/second')).toEqual('/first/second');
        expect(urls.createUrlPath('/first', '/second')).toEqual('/first/second');

        expect(urls.createUrlPath('first/', 'second')).toEqual('/first/second');
        expect(urls.createUrlPath('first', 'second/')).toEqual('/first/second');
        expect(urls.createUrlPath('first/', 'second/')).toEqual('/first/second');

        expect(urls.createUrlPath('/first', '/second/')).toEqual('/first/second');
        expect(urls.createUrlPath('/first/', '/second')).toEqual('/first/second');
        expect(urls.createUrlPath('/first', '/second/')).toEqual('/first/second');
        expect(urls.createUrlPath('/first/', '/second/')).toEqual('/first/second');
    })

    test('createUrlPath handles multiple parts', () => {
        expect(urls.createUrlPath('one', 'two', 'three', 'four')).toEqual('/one/two/three/four');
    })
});

describe('utils/urls.staticFile', () => {
    vi.stubGlobal('useRuntimeConfig', () => {
        const stub: RuntimeConfig = {
            app: {
                baseURL: 'thebase',
                buildAssetsDir: '',
                cdnURL: ''
            },
            public: {
                containerTag: '',
                containerEnv: '',
                cqlEditorPath: '',
                storageUrl: '',
                gtag: {
                    enabled: true,
                    id: '',
                    initCommands: [],
                    tags: [],
                    url: '',
                    config: undefined,
                    loadingStrategy: 'async',
                }
            },
            icon: {
                serverKnownCssClasses: []
            }
        }

        return stub
    })

    test('staticFile applies baseUrl', () => {
        expect(urls.staticFile('a/path')).toEqual('/thebase/a/path');
    })
});

// isValidUrl...
describe('utils/urls.isValidUrl', () => {
    test('isValidUrl', () => {
        // Valid URLs...
        expect(urls.isValidUrl('https://www.google.com')).toEqual(true);
        expect(urls.isValidUrl('http://www.google.com')).toEqual(true);
        expect(urls.isValidUrl('https://www.google.com/')).toEqual(true);
        expect(urls.isValidUrl('meldrx://main')).toEqual(true);

        // Invalid URLs...
        expect(urls.isValidUrl('www.google.com')).toEqual(false);
        expect(urls.isValidUrl('google.com')).toEqual(false);
        expect(urls.isValidUrl('google')).toEqual(false);
        expect(urls.isValidUrl('https://')).toEqual(false);
    })
});
