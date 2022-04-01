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
                <div class="mt-2">
                    <v-checkbox
                        v-model="img_out"
                        label="Output is image"
                        hide-details
                    ></v-checkbox>
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
                v-if="results.length !== 0 || img_res">
            <v-card-title>Console Output</v-card-title>
            <v-card-text>
                <div v-if="img_res">
                    <img v-bind:src="'data:image/svg+xml;base64,'+ img_res" class="result-image">
                    <v-card-actions class="justify-center">
                        <v-btn color="green lighten-3" class="black--text" elevation="2" @click="saveImagePng">
                            <v-icon class="pr-2">mdi-file-image</v-icon>
                            Save as PNG
                        </v-btn>
                        <v-btn color="green lighten-3" class="black--text" elevation="2" @click="saveImageSvg">
                            <v-icon class="pr-2">mdi-file-image</v-icon>
                            Save as SVG
                        </v-btn>
                        <v-tooltip bottom>
                            <template v-slot:activator="{ on, attrs }">
                                <v-btn 
                                color="green lighten-3" 
                                class="black--text ml-2" 
                                elevation="2"
                                v-bind="attrs"
                                v-on="on"
                                @click="saveImagePdf">
                                    <v-icon class="pr-2">mdi-file-pdf-box</v-icon>
                                    Save as PDF
                                </v-btn>
                            </template>
                            <span>Attention! Image in PDF will be rasterized.</span>
                        </v-tooltip>
                    </v-card-actions>
                </div>
                <div v-if="results.length !== 0" class="my-2">
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

import ImageHelper from '@/mixins/ImageHelper';

export default {
    name: 'Home',
    components: {
        PrismEditor,
    },
    mixins: [
        ImageHelper,
    ],
    data: () => ({
        cmd: "",
        results: [],
        img_out: false,
        img_res: null,
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
                image_output:   this.img_out,
                command:        this.cmd,
            };

            this.$http.post('/r/compute', data).then(response => {
                if(response.data.statusCode == 200) {
                    if(this.img_out) {
                        this.img_res = response.data.result;
                    }else{
                        this.results.push(response.data.result);
                    }
                }else{
                    this.$root.$refs.Alert.show('Error from R engine: ' + response.data.message + '.', 'error');
                }
            }).catch(error => {
                this.$root.$refs.Alert.show('[API] Error: ' +  error.message, 'error');
            });
        },
        clearResults() {
            this.img_res = null,
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