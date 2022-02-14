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
            <v-card-actions>
                <v-btn block elevation="2" @click="requestApi" :disabled="(cmd.length <= 0)">
                    <v-icon left>mdi-calculator</v-icon>
                    Compute
                </v-btn>
            </v-card-actions>
        </v-card>
        <div v-if="result" class="my-2">Result is: {{ result }}</div>
        <div v-if="status" class="my-2">Latest status code: {{ status }}</div>
    </v-container>
</template>

<script>
import { PrismEditor } from 'vue-prism-editor';
import 'vue-prism-editor/dist/prismeditor.min.css';

import { highlight, languages } from 'prismjs/components/prism-core';
import 'prismjs/components/prism-r';
import 'prismjs/themes/prism-coy.css';

const BASE_URL = 'http://localhost:50598/api';

export default {
    name: 'Home',
    components: {
        PrismEditor,
    },
    data: () => ({
        cmd: "",
        result: null,
        status: null,
    }),
    methods: {
        highlighter(code) {
            return highlight(code, languages.r)
        },
        requestApi: function () {
            let url = BASE_URL + '/rlang/' + this.cmd;
            this.$http.get(url).then(response => {
                /* eslint no-console: ["error", { allow: ["warn", "error"] }] */
                console.error(response);
                this.result = response.data.result;
                this.status = response.data.statusCode;
            }).catch(error => {
                /* eslint no-console: ["error", { allow: ["warn", "error"] }] */
                console.error('error', error)
            });
        },
    }
};
</script>