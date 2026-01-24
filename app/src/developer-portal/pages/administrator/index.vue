<script setup lang="ts">

definePageMeta({
    layout: 'blank',
    middleware: [
        function(){
            const { isAdmin } = useAuth()

            if (isAdmin()) {
                return navigateTo('/administrator/workspaces')
            }
            else
            {
                return navigateTo('/')
            }
        }
    ]
})

useHead({ title: 'Admin | MeldRx' })

</script>
