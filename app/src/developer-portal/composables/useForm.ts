// key to identify this service injection
export const useFormKey = 'form-services'

// type to refer tp <DsForm ref="someRef"> as
//      const someRef = ref<FormRef>()
export interface FormRef {
    validate: () => boolean
}

// interface for evaluating input's validity status
export interface FormInput {
    validate: () => boolean
}

// form service injection interface
export interface FormService {
    addInput: (input: FormInput) => void,
    removeInput: (input: FormInput) => void
}

// FormService setup (almost Observer pattern)
// - provide a container for inputs to register with.
// - trigger validation against registered inputs.
export function useDefineForm() : FormRef {
    const inputValidations = ref<FormInput[]>([])

    const service: FormService = {
        addInput: input => inputValidations.value.push(input),
        removeInput: (input) => {
            const index = inputValidations.value.indexOf(input)
            inputValidations.value.splice(index, 1)
        }
    }

    provide<FormService>(useFormKey, service)

    return {
        validate: function() {
            let result = true;

            for (let i = 0; i < inputValidations.value.length; i++) {
                const input = inputValidations.value[i];
                if (!input.validate()) {
                    result = false
                }
            }

            return result
        }
    }
}
