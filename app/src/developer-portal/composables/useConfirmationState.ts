type ConfirmationState = {
    active: boolean;
    message: string
    confirmationMessage: string,
    confirmCallback: () => void,
    cancelCallback: () => void,
}

const noOp = () => {}

export default function() {
    function stateFactory() : ConfirmationState {
        return {
            active: false,
            message: '',
            confirmationMessage: '',
            confirmCallback: noOp,
            cancelCallback: noOp
        }
    }

    // internal state
    const state = useState<ConfirmationState>('confirmation-dialog', stateFactory)

    return {
        state: readonly(state),
        hide: () => {
            state.value.active = false
            state.value.confirmCallback = noOp
            state.value.cancelCallback = noOp
        },
        show: (
            description: string,
            confirmationMessage: string,
            confirmCallback: () => void,
            cancelCallback: () => void
        ) => {
            state.value.active = true;
            state.value.message = description;
            state.value.confirmationMessage = confirmationMessage;
            state.value.confirmCallback = confirmCallback
            state.value.cancelCallback = cancelCallback
        }
    }
}
