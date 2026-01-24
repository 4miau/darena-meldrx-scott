import { useFormKey } from './useForm'

export type ValidationRule<TValue> = [(value?: TValue) => boolean, error: string]

export interface ValidationResult {
    isValid: boolean,
    error?: string
}

// observe components model value agaisnt it's validation rules
export default function useValidation<TValue>(
    computeValue: () => TValue | undefined,
    computedRules: () => ValidationRule<TValue>[] | undefined,
    computedRequired: () => boolean
) {
    // input is clean to start with
    // - clean: don't show error
    // - dirty: show error after input is touched
    const dirty = ref(false)
    const inputValue = computed(computeValue)
    const validationRules = computed(computedRules)
    const required = computed(computedRequired)

    // compute model value against validation rules,
    // exit upon first validation error.
    const validationResult = computed<ValidationResult>(() => {
        if (!required.value && !inputValue.value) {
            return { isValid: true }
        }

        if (!dirty.value || !validationRules.value) {
            return { isValid: true }
        }

        for (let i = 0; i < validationRules.value.length; i++) {
            const rule = validationRules.value[i];

            const valid = rule[0](inputValue.value)
            if (!valid) {
                return { isValid: false, error: rule[1] }
            }
        }

        return { isValid: true }
    })

    // if wrapped in DsForm, register with form service
    const formService = inject<FormService | null>(useFormKey, null)
    if (formService != null) {
        const input: FormInput = {
            validate: () => {
                dirty.value = true
                return validationResult.value.isValid
            }
        }
        formService.addInput(input)
        onBeforeUnmount(() => formService.removeInput(input))
    }

    return {
        dirty,
        validationResult
    }
}
