import type { Config } from 'tailwindcss'
import defaultTheme from 'tailwindcss/defaultTheme'

export default <Partial<Config>>{
    content: [
        './components/**/*.{js,vue,ts}',
        './layouts/**/*.vue',
        './pages/**/*.vue',
        './plugins/**/*.{js,ts}',
        './app.vue',
        './error.vue'
    ],
    theme: {
        extend: {
            fontFamily: {
                sans: ['Rubik', 'Helvetica', ...defaultTheme.fontFamily.sans]
            },
            borderRadius: {
                none: '0',
                sm: '3px',
                DEFAULT: '5px'
            },
            colors: {
                dsprimary: {
                    DEFAULT: '#02B689',
                    50: '#81D3C4',
                    500: '#02B689' // for toast compatibility
                },
                mint: { // same as primary
                    DEFAULT: '#02B689',
                    50: '#81D3C4'
                },
                secondary: {
                    DEFAULT: '#095E86',
                    50: '#82AFD3',
                    500: '#095E86' // for toast compatibility
                },
                'lapis-lazuli': { // same as secondary
                    DEFAULT: '#095E86',
                    50: '#82AFD3'
                },
                ember: {
                    DEFAULT: '#7E0000',
                    50: '#BF8080'
                },
                fire: {
                    DEFAULT: '#E32D1A',
                    50: '#DD9292',
                    500: '#E32D1A' // for toast compatibility
                },
                tangerine: {
                    DEFAULT: '#F46439',
                    50: '#FCB89E'
                },
                marigold: {
                    DEFAULT: '#E8A649',
                    50: '#F4D2A4'
                },
                jasmine: {
                    DEFAULT: '#E9C772',
                    50: '#EFD595'
                },
                seafoam: '#CBF5EA',
                'dark-cyan': {
                    DEFAULT: '#068A88',
                    50: '#82C4C4'
                },
                emerald: {
                    DEFAULT: '#04956A',
                    50: '#82CBB3'
                },
                pine: {
                    DEFAULT: '#02754B',
                    50: '#82BDA5'
                },
                moss: {
                    DEFAULT: '#054632',
                    50: '#82A399'
                },
                air: '#9AC8DD',
                sky: '#6896AB',
                cerulean: {
                    DEFAULT: '#0C79AC',
                    50: '#85BCD5'
                },
                indigo: {
                    DEFAULT: '#004266',
                    50: '#829FAD'
                },
                mulberry: {
                    DEFAULT: '#744963',
                    50: '#BA98B4'
                },
                white: '#FFFFFF',
                bliss: '#F5F5F5',
                alabaster: '#F0EADC',
                silver: '#D9D9D9',
                dsgray: {
                    DEFAULT: '#C2C2C2',
                },
                platinum: '#909192',
                twilight: '#6B6D6E',
                space: '#46494A',
                onyx: '#3B3E3F',
                coal: '#253436',
                black: '#000000',
                shell: '#F6C1A8',
                peach: '#E0A091',
                honey: '#CF965F',
                sandalwood: '#815622',
                cinnamon: '#8E4E23',
                walnut: '#693B24',
                espresso: '#433325'
            }
        }
    },
    safelist: [
        {
            pattern: /(bg|border|text|ring|to|from|border-l|border-r|border-t|border-b)-(primary|secondary|bliss|fire|ember|white|dsgray|black|cerulean|dsprimary|mint|lapis-lazuli|tangerine|marigold|jasmine|seafoam|dark-cyan|emerald|pine|moss|air|sky|indigo|mulberry|alabaster|silver|platinum|twilight|space|onyx|coal|shell|peach|honey|sandalwood|cinnamon|walnut|espresso)(?:-(50|500))?/,
        }
    ]
}
