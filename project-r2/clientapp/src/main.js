import Vue from 'vue'
import router from './router/index';
import App from './App.vue'
import vuetify from './plugins/vuetify';
import Axios from 'axios'

Vue.config.productionTip = false;
Vue.prototype.$http = Axios;

new Vue({
  router,
  vuetify,
  render: h => h(App)
}).$mount('#app')
