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
            v-if="items !== null && headers !== null"
            :headers="headers"
            :items="items"
            :items-per-page="50"
            :search="search"
            :footer-props="footer_props"
            loading-text="Loading your dataset, please wait"
            :loading="loading"
            dense
            multi-sort
        >
            <template v-slot:item.actions="{ item }">
                <v-icon
                    class="mr-2"
                    color="green lighten-1"
                    @click="editItem(item)"
                >mdi-pencil
                </v-icon>
                <v-icon
                    color="red lighten-1"
                    @click="deleteItem(item)"
                >mdi-delete
                </v-icon>
            </template>
        </v-data-table>
    </v-card>
</template>

<script>

export default {
    name: 'Explorer',

    data: () => ({
        items: null,
        headers: null,
        search: null,
        loading: false,
        footer_props: {
            'items-per-page-options': [15, 30, 50, 100, 200, -1], 
            'showFirstLastPage': true
        },
    }),
    mounted: function () {
        this.loading = true;
        this.items = JSON.parse(localStorage.getItem('loaded_data_json'));
        let keys = JSON.parse(localStorage.getItem('loaded_data_headers'));
        let h = [];
        keys.forEach((e) => {
            h.push({
                text:   e,
                value:  e,
            });
        });
        h.push({
            text:       'Actions',
            value:      'actions',
            align:      'right',
            sortable:   false,
            searchable: false,
        });
        this.headers = h;
        this.loading = false;
    },
    methods: {
    }
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