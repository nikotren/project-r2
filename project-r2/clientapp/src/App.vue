<template>
  <v-app>
    <Header></Header>
    <Navigation></Navigation>
    <v-main class="scrollable">
        <transition name="fade" mode="out-in">
            <div class="pa-8">
              <router-view />
            </div>
        </transition>
    </v-main>
    <Alert></Alert>
  </v-app>
</template>

<script>
import Header from './components/Header.vue';
import Navigation from './components/Navigation.vue';
import Alert from './components/Alert.vue';

export default {
  name: 'App',

  components: {
    Header,
    Navigation,
    Alert,
  },

  data: () => ({
    drawer: false,
  }),
};
</script>
