export default function (text:string) : Promise<void> {
    if (navigator.clipboard) {
        return navigator.clipboard.writeText(text);
    }
    else {
        const textArea = document.createElement('textarea');
        textArea.value = text;
        textArea.style.position = 'fixed';
        textArea.style.top = '-999999px';
        textArea.style.left = '-999999px';
        document.body.appendChild(textArea);
        textArea.focus();
        textArea.select();

        return new Promise<void>((resolve, reject) => {
            try {
                const successful = document.execCommand('copy');
                if (successful) {
                    resolve()
                } else {
                    reject(new Error('Failed to copy'))
                }
            } catch (error) {
                reject(error)
            }
        }).finally(() => {
            document.body.removeChild(textArea);
        })
    }
}
