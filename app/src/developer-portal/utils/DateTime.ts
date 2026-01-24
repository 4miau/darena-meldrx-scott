type DateFormat = 'default' | 'fhir' | 'iso';
export default class DateTime {
    public static defaultDateRegex = /^(?<m>\d{2})\/(?<d>\d{2})\/(?<y>\d{4})/
    public static isoDateRegex = /^(?<y>\d{4})-(?<m>\d{2})-(?<d>\d{2})/

    public static nowDate (format?: DateFormat): string {
        const now = new Date()

        if (format === 'fhir' || format === 'iso') {
            return `${now.getFullYear()}-${this.addLeadingZero(now.getMonth() + 1)}-${this.addLeadingZero(now.getDate())}`
        }

        return `${this.addLeadingZero(now.getMonth() + 1)}/${this.addLeadingZero(now.getDate())}/${now.getFullYear()}`
    }

    // convert to YYYY-MM-DD
    public static toISODateString(date: string) {
        if (DateTime.isoDateRegex.test(date)) {
            return date;
        }

        const defaultDate = DateTime.defaultDateRegex.exec(date)
        if (defaultDate && defaultDate.groups) {
            return `${defaultDate.groups.y}-${defaultDate.groups.m}-${defaultDate.groups.d}`
        }

        throw new Error(`couldn't convert '${date}' to iso date`)
    }

    // convert to MM/DD/YYYY
    public static toDefaultDateString(date: string) {
        if (DateTime.defaultDateRegex.test(date)) {
            return date;
        }

        const defaultDate = DateTime.isoDateRegex.exec(date)
        if (defaultDate && defaultDate.groups) {
            return `${defaultDate.groups.m}/${defaultDate.groups.d}/${defaultDate.groups.y}`
        }

        throw new Error(`couldn't convert '${date}' to default date`)
    }

    private static addLeadingZero (v: number) {
        return v >= 10 ? v.toString() : `0${v}`
    }
}
