// confirmation result
export type Confirmation<TPayload = any> = {
    data?: TPayload,
    isCancelled: boolean
}

export default function<TPayload = any>() {
    const { show } = useConfirmationState()
    const promiseResolution = ref<(v: Confirmation<TPayload>) => void>()

    // show the dialog
    function showAdapter(description: string, confirmationMessage: string) {
        show(description, confirmationMessage, confirmCallback, cancelCallback)

        // store promise resolution
        return new Promise<Confirmation<TPayload>>(
            (resolve) => {
                promiseResolution.value = resolve
            }
        )
    }

    function confirmCallback() {
        if (promiseResolution.value) {
            promiseResolution.value({
                isCancelled: false
            })
            promiseResolution.value = undefined
        }
    }

    function cancelCallback() {
        if (promiseResolution.value) {
            promiseResolution.value({
                isCancelled: true
            })
            promiseResolution.value = undefined
        }
    }

    return showAdapter
}
