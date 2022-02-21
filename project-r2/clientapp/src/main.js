import Vue from 'vue'
import router from './router/index';
import App from './App.vue'
import vuetify from './plugins/vuetify';
import { Api } from './plugins/api';
import VueCompositionApi from "@vue/composition-api";
import { createPinia, PiniaVuePlugin } from 'pinia';
import piniaPluginPersistedstate from 'pinia-plugin-persistedstate'

Vue.config.productionTip = false;
Vue.prototype.$http = Api;

Vue.use(VueCompositionApi);
Vue.use(PiniaVuePlugin);

const pinia = createPinia();
pinia.use(piniaPluginPersistedstate);

new Vue({
  router,
  vuetify,
  pinia,
  render: h => h(App)
}).$mount('#app')
