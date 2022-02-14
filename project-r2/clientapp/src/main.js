import Vue from 'vue'
import router from './router/index';
import App from './App.vue'
import vuetify from './plugins/vuetify';
import { Api } from './plugins/api';

Vue.config.productionTip = false;
Vue.prototype.$http = Api;

new Vue({
  router,
  vuetify,
  render: h => h(App)
}).$mount('#app')
