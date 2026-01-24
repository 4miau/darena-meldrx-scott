const GENERIC_ERROR_TYPE = 'https://meldrx.com/generic-error';

export default function(error: any, defaultError: string = '') {
    function showError (error: string) {
        notification({
            title: 'Error',
            description: error,
            displayTime: 3000,
            variant: 'error'
        });
    }

    if (error) {
        if (error.statusCode === 400 && 'data' in error && typeof error.data === 'object' && 'type' in error.data && error.data.type === GENERIC_ERROR_TYPE) {
            const errorPayload = error.data.error;
            if (errorPayload === null || errorPayload === undefined) {
                showError(defaultError);
                return;
            }

            if (typeof errorPayload === 'string') {
                showError(error.data);
                return;
            }

            if (Array.isArray(errorPayload)) {
                showError(errorPayload.map(x => typeof x === 'string' ? x : `${x}`).join(', '));
                return;
            }
        }

        if ([400, 422].includes(error.statusCode) && typeof error.data === 'string') {
            showError(error.data);
            return;
        }

        if (error.statusCode === 403) {
            showError(`Forbidden: ${defaultError}`);
            return;
        }
    }

    showError(defaultError);
}
