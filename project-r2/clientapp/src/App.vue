<template>
  <v-app>
    <Header></Header>
    <Navigation></Navigation>
    <v-main class="scrollable">
      <div class="pa-8">
        <transition name="fade" mode="out-in">
          <router-view class="view"></router-view>
        </transition>
      </div>
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

<style>
  @import url('https://fonts.googleapis.com/css2?family=Fira+Code&display=swap');

  .my-editor {
      background: #fafafa;
      color: #000;
      font-family: Fira code, Fira Mono, Consolas, Menlo, Courier, monospace;
      font-size: 16px;
      line-height: 1.5;
      padding: 5px;
  }
  .prism-editor__textarea:focus {
      outline: none;
      border: 0;
  }

  .fade-enter-active, .fade-leave-active {
    width: inherit;
    transition: opacity .2s;
  }

  .fade-enter, .fade-leave-to {
    opacity: 0;
  }

  .view > div  {
    transition: height .2s;
  }
</style>
