<template>
    <v-container>
        <v-card class="mx-auto"
                max-width="1024"
                outlined>
            <v-card-title>R Live</v-card-title>
            <v-card-text>
                <div>Type a command to run by Rlang engine:</div>
                <div class="my-2">
                    <prism-editor class="my-editor elevation-2" v-model="cmd" :highlight="highlighter" line-numbers></prism-editor>
                </div>
            </v-card-text>
            <v-card-actions class="justify-center">
                <v-btn elevation="2" @click="requestApi" :disabled="(cmd.length <= 0)">
                    <v-icon left>mdi-calculator</v-icon>
                    Compute
                </v-btn>
                <v-btn color="red" class="white--text" elevation="2" @click="clearCmd" :disabled="(cmd.length <= 0)">
                    <v-icon left>mdi-delete</v-icon>
                    Reset
                </v-btn>
            </v-card-actions>
        </v-card>
        <!--<div v-if="status" class="my-2">Latest status code: {{ status }}</div>-->
        <v-card class="mx-auto"
                max-width="1024"
                outlined
                v-if="results.length !== 0">
            <v-card-title>Console Output</v-card-title>
            <v-card-text>
                <div class="my-2">
                    <prism-editor class="my-editor console-editor elevation-2" v-model="results_formatted" :highlight="highlighter2" readonly></prism-editor>
                </div>
            </v-card-text>
            <v-card-actions class="justify-center">
                <v-btn color="red" class="white--text" elevation="2" @click="clearResults">
                    <v-icon left>mdi-delete</v-icon>
                    Clear
                </v-btn>
            </v-card-actions>
        </v-card>
    </v-container>
</template>

<script>
import { PrismEditor } from 'vue-prism-editor';
import 'vue-prism-editor/dist/prismeditor.min.css';

import { highlight, languages } from 'prismjs/components/prism-core';
import 'prismjs/components/prism-r';
import 'prismjs/themes/prism-coy.css';

export default {
    name: 'Home',
    components: {
        PrismEditor,
    },
    data: () => ({
        cmd: "",
        results: [],
        status: null,
    }),
    methods: {
        highlighter(code) {
            return highlight(code, languages.r);
        },
        highlighter2(code) {
            return code;
        },
        requestApi: function () {
            let data = {
                command: this.cmd
            };

            this.$http.post('/r/live', data).then(response => {
                /* eslint no-console: ["error", { allow: ["warn", "error"] }] */
                console.error(response);
                if(response.data.statusCode == 200) {
                    this.results.push(response.data.result);
                }else{
                    this.$root.$refs.Alert.show('Error from R engine: ' + response.data.message + '.', 'error');
                }
            }).catch(error => {
                /* eslint no-console: ["error", { allow: ["warn", "error"] }] */
                console.error('error', error)
            });
        },
        clearResults() {
            this.results = [];
        },
        clearCmd() {
            this.cmd = "";
        },
    },
    computed: {
        results_formatted() {
            return this.results.join('\r\n');
        }
    },
};
</script>