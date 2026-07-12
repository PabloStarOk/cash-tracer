import { createApp } from 'vue'
import { createPinia } from 'pinia'
import App from './App.vue'
import { PrimeVue } from '@primevue/core'
import '@/assets/main.css'
import { ConfirmationService, ToastService } from 'primevue'
import { primeVueConfig } from '@/plugins/primevue'
import { useThemeManager } from '@/composables/useThemeManager'

const { initialize } = useThemeManager()
initialize()

const app = createApp(App)

app.use(createPinia())
app.use(PrimeVue, primeVueConfig)
app.use(ConfirmationService)
app.use(ToastService)

app.mount('#app')
