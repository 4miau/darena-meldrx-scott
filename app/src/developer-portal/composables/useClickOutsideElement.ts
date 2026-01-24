export default function (
    element: Ref<HTMLElement | undefined>,
    onClickedOutsideHook: () => void
) {
    function handleClick(e : MouseEvent) {
        if (element.value) {
            if (!e.composedPath().includes(element.value)) {
                onClickedOutsideHook()
            }
        }
    }
    onMounted(() => {
        window.addEventListener('click', handleClick)
    })
    onBeforeUnmount(() => {
        window.removeEventListener('click', handleClick)
    })
}
