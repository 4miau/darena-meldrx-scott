type ToastHandler = (toast: Toast) => void

const toastHandlers: ToastHandler[] = []

type Links = { title: string, link: string }

export type Toast = {
  id?: string,
  title?: string,
  description?: string,
  displayTime?: number,
  variant?: string,
  links?: Links[],
  type?: 'danger' | 'warning' | 'success' | 'info'
}

function addToast(toast: Omit<Toast, 'id'>) {
    const newToast: Toast = {
        id: crypto.randomUUID(),
        ...toast,
        displayTime: toast.displayTime ?? 4000
    }

    toastHandlers.forEach((toastHandler) => { toastHandler(newToast) })
}

export function useToaster(handler?: (toast: Toast) => void) {
    if (handler) {
        toastHandlers.push(handler)
    }

    return addToast
}
