export enum BackgroundJobStatus {
    None = 'None',
    Running = 'Running',
    Completed = 'Completed',
    Error = 'Error',
    Expired = 'Expired',
    Cancelled = 'Cancelled'
}
export const backgroundJobStatusConfig = [
    { value:BackgroundJobStatus.None, title: 'None' },
    { value:BackgroundJobStatus.Running, title: 'Running' },
    { value:BackgroundJobStatus.Completed, title: 'Completed' },
    { value:BackgroundJobStatus.Error, title: 'Error' },
    { value:BackgroundJobStatus.Expired, title: 'Expired' },
    { value:BackgroundJobStatus.Cancelled, title:  'Cancelled '}
]
