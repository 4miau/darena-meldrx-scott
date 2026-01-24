import hljs from 'highlight.js/lib/core';
import javascript from 'highlight.js/lib/languages/javascript';
import bash from 'highlight.js/lib/languages/bash';
import 'highlight.js/styles/default.css';

hljs.registerLanguage('javascript', javascript);
hljs.registerLanguage('bash', bash);

export default defineNuxtPlugin((nuxtApp) => {
    nuxtApp.vueApp.directive('code-highlight', {
        mounted (el) {
            hljs.highlightElement(el)
        }
    })
})
