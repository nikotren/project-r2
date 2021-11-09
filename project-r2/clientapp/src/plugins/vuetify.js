import Vue from 'vue';
import Vuetify from 'vuetify/lib/framework';

Vue.use(Vuetify);

export default new Vuetify({
    theme: {
        themes: {
            light: {
                primary: '#78909C',
                secondary: '#9E9E9E',
                accent: '#424242',
                error: '#b71c1c',
            },
        },
    },
});
