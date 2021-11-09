<template>
    <v-app-bar
        app
        color="primary"
        >
        <v-btn icon @click="toggleNavigation()">
            <v-icon>mdi-menu</v-icon>
        </v-btn>
        <router-link to="/" class="no-underline">
            <v-toolbar-title class="text-no-wrap app-logo">Project R</v-toolbar-title>
        </router-link>
        <v-spacer></v-spacer>
    </v-app-bar>
</template>

<script>
export default {
    name: 'Header',
    data: () => ({
    }),
    computed: {
    },
    methods: {
        toggleNavigation: function () {
            this.$root.$refs.Navigation.toggle();
        },
    },

}
</script>

<style scoped>
.app-logo {
    margin-left: 1rem;
    color: #fff !important;
    font-weight: 600 !important;
}

.no-underline {
    text-decoration: none !important;
}
</style>