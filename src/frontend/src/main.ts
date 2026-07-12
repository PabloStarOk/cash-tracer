import { createApp } from 'vue'
import { createPinia } from 'pinia'
import App from './App.vue'
import { PrimeVue } from '@primevue/core'
import Aura from '@primeuix/themes/aura'
import '@/assets/main.css'
import { ConfirmationService, ToastService } from 'primevue'

const app = createApp(App)

app.use(createPinia())
app.use(PrimeVue, {
  theme: {
    preset: Aura,
  },
})
app.use(ConfirmationService)
app.use(ToastService)

app.mount('#app')
