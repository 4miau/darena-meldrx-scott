import withNuxt from './.nuxt/eslint.config.mjs'

export default withNuxt(
    {
        rules: {
            indent: [
                'error',
                4,
                {
                    SwitchCase: 1,
                }
            ],

            semi: 'off',
            'brace-style': 'off',
            'vue/no-multiple-template-root': 'off',
            '@typescript-eslint/no-explicit-any': 'off',
            '@typescript-eslint/no-invalid-void-type': 'off',
            '@typescript-eslint/no-extraneous-class': 'off',

            'vue/singleline-html-element-content-newline': ['error', {
                ignoreWhenNoAttributes: true,
                ignoreWhenEmpty: true,

                ignores: [
                    'DsText',
                    'DsLink',
                    'a',
                    'abbr',
                    'audio',
                    'b',
                    'bdi',
                    'bdo',
                    'canvas',
                    'cite',
                    'code',
                    'data',
                    'del',
                    'dfn',
                    'em',
                    'i',
                    'iframe',
                    'ins',
                    'kbd',
                    'label',
                    'map',
                    'mark',
                    'noscript',
                    'object',
                    'output',
                    'picture',
                    'q',
                    'ruby',
                    's',
                    'samp',
                    'small',
                    'span',
                    'strong',
                    'sub',
                    'sup',
                    'svg',
                    'time',
                    'u',
                    'var',
                    'video',
                ],

                externalIgnores: [],
            }],

            'space-before-function-paren': 'off',
        },
    }
)
