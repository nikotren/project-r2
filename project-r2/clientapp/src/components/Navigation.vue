<template>
    <v-navigation-drawer
        v-model="opened"
        absolute
        bottom
        temporary
        >
        <v-list nav>
            <v-list-item-group>
                <v-list-item v-for="item in items" :key="item.title" :to="item.path">
                    <v-list-item-icon>
                        <v-icon>{{ item.icon }}</v-icon>
                    </v-list-item-icon>

                    <v-list-item-content>
                        <v-list-item-title>{{ item.title }}</v-list-item-title>
                    </v-list-item-content>
                </v-list-item>
            </v-list-item-group>
        </v-list>
    </v-navigation-drawer>
</template>

<script>
export default {
    name: 'Navigation',
    data: () => ({
        opened: false,
        items: [
            {
                icon: 'mdi-home',
                title: 'Home',
                path: '/',
            },
            {
                icon: 'mdi-file-import',
                title: 'Import Dataset',
                path: '/import',
            },
            {
                icon: 'mdi-database-search',
                title: 'Dataset Explorer',
                path: '/explorer',
            },
            {
                icon: 'mdi-math-integral-box',
                title: 'Compute',
                path: '/compute',
            },
            {
                icon: 'mdi-information',
                title: 'About',
                path: '/about',
            },
        ],
    }),
    created() {
        this.$root.$refs.Navigation = this;
    },
    methods: {
        toggle: function () {
            this.opened = !this.opened;
        },
    },
}
</script>