export default function(description: string, title: string = 'Success') {
    notification({
        title,
        description,
        displayTime: 3000,
        variant: 'success'
    })
}
