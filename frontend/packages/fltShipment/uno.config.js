import { defineConfig, presetUno } from 'unocss'
import transformerDirectives from '@unocss/transformer-directives'

export default defineConfig({
  rules: [
    [
      /^text-(.*)$/,
      ([, c], { theme }) => {
        if (theme.colors[c]) return { color: theme.colors[c] }
      },
    ],
  ],
  theme: {
    colors: {
      EzyBooking: {
        green: {
          DEFAULT: '#189068',
          1: '#1a8242',
          2: '#10b981',
          3: '#76b146',
          4: '#453154',
          5: '#005431',
          6: '#2ec291',
        },

        dark: {
          green: {
            DEFAULT: '#146c4f',
            1: '#0f523c',
            2: '#F2F6F6',
          },
          gray: {
            DEFAULT: '#6f7c7c',
            1: '#556565',
            2: '#f2f6f6',
            3: '#b2baba',
            4: '#d5d9d9',
          },
        },

        bright: {
          green: '#90be20',
        },

        light: {
          green: {
            DEFAULT: '#9ad2c0',
            1: 'rgba(26, 130, 66, 0.05)',
            2: '#e8f4f0',
            3: '#f6f7f7',
            4: '#f3f9f7',
          },
          blue: { DEFAULT: '#e9f1f3', 1: '#E0F6FC' },
          gray: {
            DEFAULT: '#dde0e0',
            1: '#eef0f0',
            2: '#999999',
            3: '#94a3b8',
            4: '#a09e9e',
            5: '#ededed',
            6: '#f5f5f5',
          },
        },

        blue: {
          gray: '#64748b',
          1: '#007C92',
        },

        sky: {
          blue: {
            DEFAULT: '#00a3c5',
            hover: '#00cdf7',
          },
        },

        gray: {
          DEFAULT: '#808b8b',
          1: '#d9d9d9',
          2: '#333737',
        },

        red: {
          DEFAULT: '#f40e0e',
          1: 'rgba(214, 72, 48, 0.05)',
          2: '#d64830',
          3: '#e03426',
          4: '#fee7e7',
          5: '#f40e0e10',
          6: '#FF6F6F',
        },

        alert: {
          yellow: '#eebd0e',
        },

        disable: '#d5d5d9',

        black: {
          DEFAULT: '#414042',
          2: '#0f172a',
        },
        white: {
          1: '#fcf8f9',
        },
      },
    },
    fontFamily: {},
    breakpoints: {
      sm: '320px',
      md: '640px',
      lg: '991px',
      xl: '1280px',
      '2xl': '1440px',
      '3xl': '1920px',
    },
    verticalBreakpoints: {},
    boxShadow: {
      'gls-base': '0px 10px 10px 0px rgba(0, 0, 0, 0.05)',
    },
  },
  shortcuts: [
    // you could still have object style
    {
      btn: 'py-2 px-4 font-semibold rounded-lg shadow-md',
    },
  ],
  variants: [
    // hover:
    (matcher) => {
      if (!matcher.startsWith('hover:')) return matcher
      return {
        // slice `hover:` prefix and passed to the next variants and rules
        matcher: matcher.slice(6),
        selector: (s) => `${s}:hover`,
      }
    },
  ],
  preflights: [],
  layers: {
    components: -1,
    default: 1,
    utilities: 2,
    'my-layer': 3,
  },
  presets: [presetUno({ prefix: 'u-' })],
  transformers: [transformerDirectives()],
  safelist: [...Array.from({ length: 12 }, (_, i) => `u-grid-cols-${i + 1}`)],
})
