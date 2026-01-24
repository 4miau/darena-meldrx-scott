import type { ValidationRule } from '~/composables/useValidation'

export default class ValidationRules {
    private static notEmpty<T>(value:T) : boolean {
        return !!value;
    }

    public static describeNotEmpty(description: string) : ValidationRule<string> {
        return [ValidationRules.notEmpty, description]
    }

    public static valid<T>(value: T, rules: ValidationRule<T>[]) {
        return rules.reduce((result, next) => result && next[0](value), true)
    }

    public static appName : ValidationRule<string>[] = [
        [v => !!v, 'App Name is required'],
        [v => v!.trim().length > 0, 'App Name is required'],
        [v => v!.length <= 200, 'App Name must be 200 characters or less']
    ];

    public static workspaceName : ValidationRule<string>[] = [
        [v => !!v, 'Workspace name is required'],
        [v => v!.trim().length > 0, 'Workspace name is required'],
        [v => v!.length >= 5, 'Workspace name must be at least 5 characters long'],
    ];

    public static url : ValidationRule<string>[] = [
        ValidationRules.describeNotEmpty('URL cannot be empty'),
        [v => /^[\w+.]+:\/\//.test(v!), 'Invalid URL schema/protcol'],
        [v => !/\s/.test(v!), 'URL cannot contain whitespace'],
        [v => urls.isValidUrl(v!), 'Invalid URL']
    ];

    public static emailRegex : RegExp = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/

    public static email : ValidationRule<string | null>[] = [
        [v => !!v, 'Please provide an email address'],
        [v => ValidationRules.emailRegex.test(v!), 'Please provide a valid email address']
    ];

    public static npi : ValidationRule<string>[] = [
        [v => !!v, 'NPI is required'],
        [v => v!.length === 10, 'NPI should be 10 numbers long.']
    ]

    public static providerName : ValidationRule<string>[] = [
        [v => !!v, 'Provider Name is required'],
        [v => /^[a-zA-Z0-9][a-zA-Z0-9& .',-]*$/.test(v!), 'Invalid Provider Name']
    ]

    public static slug : ValidationRule<string>[] = [
        [v => !!v, 'Slug is required'],
        [v => !v!.startsWith('-'), 'Slug needs to start with a letter or number'],
        [v => !v?.endsWith('-'), 'Slug needs to end with a letter or number'],
        [v => /^[a-z0-9-_]+$/.test(v!), 'Slug can only contain lower case characters, numbers, dashes, and underscores'],
        [v => v!.length <= 100, 'Slug can\'t be longer than 100 characters']
    ]
}
