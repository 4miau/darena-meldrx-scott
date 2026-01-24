export default function<T>(func: (v:T) => void, timeout: number) {
    const timer = ref<any>(null)
    
    const debounce = (v: T) => {
        clearTimeout(timer.value);
        timer.value = setTimeout(() => {
            timer.value = null
            func(v)
        }, timeout);
    };
    
    return {
        debounce, 
        debouncing: computed(() => timer.value !== null)
    }
}
