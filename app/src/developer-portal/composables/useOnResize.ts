type OnResizeHooks = {
  onResize?: () => void;
  onResizeEnd?: () => void;
}

export default function(payload: OnResizeHooks) {
    const state: {
      timeout: any
    } = {
        timeout: undefined
    };

    function handleClick () {
        if(payload.onResize){
            payload.onResize()
        }

        if (payload.onResizeEnd) {
            if (state.timeout) {
                clearTimeout(state.timeout);
            }

            state.timeout = setTimeout(payload.onResizeEnd, 150);
        }
    }

    onMounted(() => {
        window.addEventListener('resize', handleClick);
    });
    onBeforeUnmount(() => {
        window.removeEventListener('resize', handleClick);
    });
}
