<template>
    <v-card>
        <v-card-title>
            Dataset Explorer
            <v-spacer></v-spacer>
            <v-text-field
                v-model="search"
                append-icon="mdi-magnify"
                label="Search"
                single-line
                hide-details
            ></v-text-field>
        </v-card-title>
        <v-data-table
            v-if="store.dataset !== null && store.headers !== null"
            :headers="headers_formatted"
            :items="store.dataset"
            :items-per-page="50"
            :search="search"
            :footer-props="footer_props"
            loading-text="Loading your dataset, please wait"
            :loading="loading"
            dense
            multi-sort
        >
            <template v-slot:footer.page-text>
                <v-btn color="brown" class="white--text mr-2" dark elevation="2" @click="importRedirect">
                    <v-icon left>mdi-file-import-outline</v-icon>
                    Import
                </v-btn>
                <v-btn v-if="store.dataset.length > 0" color="red" class="white--text" dark elevation="2" @click="deleteAll">
                    <v-icon left>mdi-delete</v-icon>
                    Reset Dataset
                </v-btn>
            </template>
            <template v-slot:no-data>
                <v-alert :value="true" color="red lighten-3">
                    No dataset imported..
                </v-alert>
            </template>
        </v-data-table>
    </v-card>
</template>

<script>
import { datasetStore } from '@/store/dataset'

export default {
    name: 'Explorer',
    data: () => ({
        search: null,
        loading: false,
        footer_props: {
            'items-per-page-options': [15, 30, 50, 100, 200, -1], 
            'showFirstLastPage': true
        },
    }),
    setup() {
        const store = datasetStore()
        return { store }
    },
    mounted: function () {
    },
    methods: {
        deleteAll: function() {
            this.store.dataset = [];
            this.store.headers = [];
            this.$root.$refs.Alert.show('Dataset removed successfully.');
        },
        importRedirect: function() {
            this.$router.push('import');
        },
    },
    computed: {
        headers_formatted() {
            let r = [];
            this.store.headers.forEach((e) => {
                r.push({
                    text:   e,
                    value:  e,
                });
            });
            return r;
        }
    },
};
</script>

<style>
    .theme--light.v-data-table tbody tr:nth-of-type(even) {
        background-color: rgba(0, 0, 0, .03);
    }
    .theme--dark.v-data-table tbody tr:nth-of-type(even) {
        background-color: rgba(0, 0, 0, .5);
    }
    .v-data-table thead th {
      font-size: 14px !important;
    }
</style>