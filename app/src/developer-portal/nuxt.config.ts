export default defineNuxtConfig({
    runtimeConfig: {
        public: {
            containerTag: '',
            containerEnv: '',
            cqlEditorPath: process.env.NUXT_CQL_EDITOR_PATH || '/_cql-builder.js',
            storageUrl: 'https://storage.local.meldrx.com/devstoreaccountpublic/assets',
        }
    },
    modules: ['nuxt-gtag', "@nuxt/eslint", '@nuxtjs/tailwindcss', '@nuxt/icon'],
    ssr: false,
    components: [
        {
            path: '~/components',
            pathPrefix: false
        }
    ],
    devtools: { enabled: true },
    css: ['~/assets/css/main.css'],
    postcss: {
        plugins: {
            tailwindcss: {},
            autoprefixer: {}
        }
    },
    typescript: {
        strict: true,
        typeCheck: true
    },
    gtag: {
        id: 'G-49H6N6EDSC'
    },
    ignore: process.env.NODE_ENV !== 'development'
        ? ['**/component-gallery/**']
        : [],
    app: {
        baseURL: '/',
        head: {
            link: [
                { rel: 'preconnect', href: 'https://fonts.googleapis.com' },
                { rel: 'stylesheet', href: 'https://fonts.googleapis.com/css2?family=Rubik:ital,wght@0,300;0,400;0,500;0,600;0,700;0,800;0,900;1,300;1,400;1,500;1,600;1,700;1,800;1,900&display=swap' },
                { rel: 'stylesheet', href: 'https://maxcdn.bootstrapcdn.com/font-awesome/4.7.0/css/font-awesome.min.css' },

                { href: 'https://cdn.meldrx.com/assets/ccda-viewer/css/cdaSCSS.css', rel: 'stylesheet' },
                { href: 'https://cdn.meldrx.com/assets/ccda-viewer/css/ccd-viewerSCSS.v2.css', rel: 'stylesheet' },
                { href: 'https://cdn.meldrx.com/assets/ccda-viewer/css/marketingSCSS.css', rel: 'stylesheet' },
                { href: 'https://cdn.meldrx.com/assets/ccda-viewer/css/pureextensionSCSS.css', rel: 'stylesheet' },
            ],
            script: [
                { src: 'https://cdn.meldrx.com/assets/jquery-1.12.0.min.js' },
                { src: 'https://cdn.meldrx.com/assets/ccda-viewer/js/masonry.pkgd.min.js' },
                { src: 'https://cdn.meldrx.com/assets/ccda-viewer/js/draggabilly.pkgd.min.js' },
                { src: 'https://cdn.meldrx.com/assets/ccda-viewer/js/packery.pkgd.min.js' },
                { src: 'https://cdn.meldrx.com/assets/ccda-viewer/js/core.js' },
                { src: 'https://cdn.meldrx.com/assets/ccda-viewer/js/xslt.js' },
            ]
        }
    },
    vite: {
        server: {
            hmr: {
                protocol: 'wss',
            },
        }
    },
    hooks: {
        'pages:extend'(pages) {
            for (const page of pages) {
                if (page.path.startsWith('/admin')) {
                    page.meta ||= {}
                    page.meta.layout = 'admin'
                    page.meta.middleware = ['admin-only']
                }
            }
        },
        'vite:extendConfig': (config) => {
            if (typeof config.server!.hmr === 'object') {
                config.server!.hmr.protocol = 'wss';
            }
        },
    },
    compatibilityDate: '2024-07-29'
})