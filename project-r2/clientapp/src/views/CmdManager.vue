<template>
    <v-card>
        <v-card-title>
            Commands Manager
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
            v-if="store.commands !== null && headers !== null"
            :headers="headers"
            :items="store.commands"
            :items-per-page="50"
            :search="search"
            :footer-props="footer_props"
            loading-text="Loading commands, please wait"
            :loading="loading"
            dense
            multi-sort
        >
            <template v-slot:item.actions="{ item }">
                <v-icon
                    @click="deleteItem(item)">
                    mdi-delete
                </v-icon>
            </template>
            <template v-slot:footer.page-text>
                <v-btn color="brown" class="white--text mr-2" dark elevation="2" @click="importCommand">
                    <v-icon left>mdi-file-import-outline</v-icon>
                    Import Command(s) From JSON
                </v-btn>
                <v-btn color="teal" class="white--text mr-2" dark elevation="2" @click="exportAll">
                    <v-icon left>mdi-file-export-outline</v-icon>
                    Export All
                </v-btn>
                <v-btn color="red" class="white--text" dark elevation="2" @click="deleteAll">
                    <v-icon left>mdi-delete</v-icon>
                    Remove All Commands
                </v-btn>
            </template>
        </v-data-table>
        <input ref="cmdUpload" type="file" accept="application/JSON" hidden @change="actualImport" onclick="this.value=null;">
    </v-card>
</template>

<script>
import { saveAs } from 'file-saver';
import { commandsStore } from '@/store/commands';

export default {
    name: 'CmdManager',
    data: () => ({
        headers: [
            {
                text:   "Command Name",
                value:  "name",
            },
            {
                text:   "Description",
                value:  "description",
            },
            {
                text:   "Actions",
                value:  "actions",
                sortable:   false,
            },
        ],
        search: null,
        loading: false,
        footer_props: {
            'items-per-page-options': [15, 30, 50, 100, 200, -1], 
            'showFirstLastPage': true
        },
    }),
    setup() {
        const store = commandsStore();
        return { store }
    },
    mounted: function () {
    },
    methods: {
        deleteAll: function() {
            this.store.commands = [];
            this.$root.$refs.Alert.show('All commands removed successfully.');
        },
        deleteItem: function(item) {
            let editedIndex = this.store.commands.indexOf(item);
            this.store.commands.splice(editedIndex, 1);
            this.$root.$refs.Alert.show('Command successfully deleted.', 'info');
        },
        importCommand: function() {
            this.$refs.cmdUpload.click();
        },
        actualImport: function() {
            const reader = new FileReader();
            reader.onerror = (err) => {
                this.$root.$refs.Alert.show('Error while importing: ' + err, 'error');
            }

            reader.onload = (res) => {
                let result = String(res.target.result);

                let new_items = JSON.parse(result);
                this.store.commands = this.store.commands.concat(new_items);
            };
            
            reader.readAsText(this.$refs.cmdUpload.files[0], 'UTF-8');
            this.$root.$refs.Alert.show('Commands imported successfully.');
        },
        exportAll: function() {
            if(this.store.commands.length <= 0) {
                return this.$root.$refs.Alert.show('There are no commands to export.', 'warning');
            }
            let file_name = "project_r_" + String(+ new Date()) + ".json";
            let blob = new Blob([JSON.stringify(this.store.commands, null, 2)], {
                type: 'application/json',
                name: file_name
            });
            saveAs(blob, file_name);
        },
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