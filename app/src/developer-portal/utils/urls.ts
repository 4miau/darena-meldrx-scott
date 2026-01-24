function ensureLeadingSlash(text: string): string {
    return text.startsWith('/') ? text : '/' + text;
}

function ensureNoTrailingSlash(text: string): string {
    return text.endsWith('/') ? text.slice(0, text.length - 1) : text;
}

// Creates a URL by combining the parts and ensuring that there is only one slash between them...
function createUrlPath(...parts: string[]): string {
    let result = '';
    for (let i = 0; i < parts.length; i++) {
        result += ensureNoTrailingSlash(
            ensureLeadingSlash(parts[i])
        );
    }

    return result;
}

// Gets the URL for a static file by combining the base URL with the path...
function staticFile(text: string): string {
    const config = useRuntimeConfig();
    return createUrlPath(config.app.baseURL, text);
}

// Returns true if the text is a valid URL...
function isValidUrl(text: string): boolean {
    try {
        // WARNING: nodejs URL constructor behaves slightly differently to the one in the browser
        const url = new URL(text);
        return !!url.protocol &&
        (!!url.host || // host for known protocol's (ws/http)
            /\/\/\w/.test(url.pathname)); // pathname for cusom protocol's
    } catch {
        return false;
    }
}

export default {
    staticFile,
    createUrlPath,
    isValidUrl
}
