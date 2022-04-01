<template>
    <div>
        <v-card class="mx-auto my-2"
                max-width="640"
                outlined>
            <v-card-title>Run Command</v-card-title>
            <v-card-text>
                <div class="row mt-6 mb-2">
                    <div class="col-12">
                        <v-select
                            v-model="actual_command"
                            item-text="name"
                            :items="cmdStore.commands"
                            label="Command"
                            @change="commandChanged"
                            return-object
                            ></v-select>
                    </div>
                    <div v-if="actual_command.description.length > 0" class="col-12">
                        <p>{{ actual_command.description }}</p>
                    </div>
                    <div v-if="actual_command.params.length > 0" class="col-12">
                        <h3>Parameters</h3>
                        <div v-for="param in actual_command.params" :key="param.var_name">
                            <v-text-field
                                v-if="param.type == 'number'" 
                                v-model="actual_values[param.var_name]" 
                                type="number"
                                :label="paramLabel(param)"
                                clearable></v-text-field>
                            <v-text-field
                                v-else-if="param.type == 'text'"
                                v-model="actual_values[param.var_name]"
                                :label="paramLabel(param)"
                                clearable></v-text-field>
                            <v-select 
                                v-else-if="param.type == 'select_headers'"
                                v-model="actual_values[param.var_name]"
                                :label="paramLabel(param)"
                                :items="dataStore.headers"></v-select>
                        </div>
                    </div>
                </div>
            </v-card-text>
            <v-card-actions>
                <v-btn block elevation="2" @click="doCompute" :disabled="actual_command.value == 'empty'">
                    <v-icon left>mdi-calculator</v-icon>
                    Compute
                </v-btn>
            </v-card-actions>
        </v-card>
        <v-card v-if="result !== null" 
            class="mx-auto my-2"
            max-width="1024"
            outlined>
            <v-card-text>
                <div v-if="actual_command.img_output">
                    <img id="svg-result" v-bind:src="'data:image/svg+xml;base64,' + result" class="result-image">
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
                <div v-else>
                    <prism-editor class="my-editor console-editor elevation-2" v-model="result" lineNumbers :highlight="highlighter2" readonly></prism-editor>
                </div>
            </v-card-text>
        </v-card>
    </div>
</template>

<script>
import { PrismEditor } from 'vue-prism-editor';
import 'vue-prism-editor/dist/prismeditor.min.css';

import { highlight, languages } from 'prismjs/components/prism-core';
import 'prismjs/components/prism-r';
import 'prismjs/themes/prism-coy.css';

import { commandsStore } from '@/store/commands';
import { datasetStore } from '@/store/dataset';

import ImageHelper from '@/mixins/ImageHelper';

export default {
    name: 'Compute',
    components: {
        PrismEditor,
    },
    mixins: [
        ImageHelper,
    ],
    data: () => ({
        result: null,
        status: null,
        actual_command: {
            text:   'Please select command',
            value:  'empty',
            description: '',
            img_output: false,
            params: []
        },
        actual_values: {},
    }),
    setup() {
        const cmdStore = commandsStore();
        const dataStore = datasetStore();
        return { cmdStore, dataStore }
    },
    mounted: function() {
    },
    methods: {
        highlighter(code) {
            return highlight(code, languages.r);
        },
        highlighter2(code) {
            return code;
        },
        commandChanged: function() {
            this.actual_values = {}; 
            this.result = null;
            //Load default values for parameters (if any)
            this.actual_command.params.forEach((e) => {
                if(e.default_value) {
                    this.actual_values[e.var_name] = e.default_value;
                }
            });
        },
        doCompute: function() {
            let url = '/r/compute';
            let parameters = [];

            for (const [k, v] of Object.entries(this.actual_values)) {
                parameters.push({
                    name:   k,
                    value:  v,
                    type:   this.actual_command.params.find(o => o.var_name === k).type,
                });
            }

            let data = {
                command:        this.actual_command.code,
                params:         parameters,
                image_output:   this.actual_command.img_output,
                dataset:        btoa(this.dataStore.raw_csv),
            };

            this.$http.post(url, data).then(response => {
                if(response.data.statusCode !== 200) {
                    this.$root.$refs.Alert.show('Error from R engine: ' + response.data.message + '.', 'error');
                }else{
                    this.result = response.data.result;
                    this.status = response.data.statusCode;
                }
            }).catch(error => {
                this.$root.$refs.Alert.show('[API] Error: ' +  error.message, 'error');
            });
        },
        paramLabel: function(p) {
            if(p.description.length > 0) {
                return `${p.name} (${p.description})`;
            }
            return p.name;
        }
    },
    computed: {
    },
}
</script>

<style>
.result-image {
    display: block;
    margin: 0 auto;
    max-width: 100%;
    height: auto;
}
</style>