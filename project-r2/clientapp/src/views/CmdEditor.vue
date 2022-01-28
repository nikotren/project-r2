<template>
    <v-card class="mx-auto"
            max-width="1024"
            outlined>
        <v-card-title>Command Description</v-card-title>
        <v-card-text>
            <v-text-field 
                v-model="cmd.name"
                label="Command Name"
                clearable></v-text-field>
            <v-text-field 
                v-model="cmd.description"
                label="Command Description"
                clearable></v-text-field>
        </v-card-text>
        <v-card-title class="mt-0 pt-0">Command Arguments</v-card-title>
        <v-card-text>
            <v-expansion-panels accordion>
                <v-expansion-panel
                    v-for="(item, i) in cmd.params"
                    :key="item.id"
                >
                <v-expansion-panel-header>
                    {{ item.name }} ({{ item.var_name }})
                </v-expansion-panel-header>
                <v-expansion-panel-content>
                    <v-text-field 
                        v-model="item.name"
                        label="Argument Name"
                        dense
                        outlined
                        clearable></v-text-field>
                    <v-text-field 
                        v-model="item.var_name"
                        label="Argument Variable Name"
                        dense
                        outlined
                        clearable></v-text-field>
                    <v-text-field 
                        v-model="item.description"
                        label="Argument Description"
                        dense
                        outlined
                        clearable></v-text-field>
                    <v-select 
                            v-model="item.type"
                            label="Argument Type"
                            :items="param_types"></v-select>
                    <v-text-field 
                        v-model="item.default_value"
                        label="Argument Default Value"
                        dense
                        outlined
                        clearable></v-text-field>
                    <div class="d-flex justify-center">
                        <v-btn color="red" dark elevation="2" @click="removeParam(i)">
                            <v-icon left>mdi-delete</v-icon>
                            Remove Argument
                        </v-btn>
                    </div>
                </v-expansion-panel-content>
                </v-expansion-panel>
            </v-expansion-panels>
            <div class="d-flex justify-center mt-4">
                <v-btn
                    class="mx-2"
                    fab
                    dark
                    color="green"
                    @click="addParam"
                >
                <v-icon>
                    mdi-plus
                </v-icon>
                </v-btn>
            </div>
        </v-card-text>
        <v-card-title class="mt-0 pt-0">R Script</v-card-title>
        <v-card-text>
            <!--<div class="mb-2">
                <p class="py-0 my-0">Use <span style="font-family: Fira code, Fira Mono, Consolas, Menlo, Courier, monospace; font-weight: 700;">data</span> variable as currently parsed dataset.</p>
                <p class="py-0 my-0">Use <span style="font-family: Fira code, Fira Mono, Consolas, Menlo, Courier, monospace; font-weight: 700;">result</span> variable as computed result which should be returned.</p>    
            </div>-->
            <prism-editor class="my-editor elevation-2" v-model="cmd.code" :highlight="highlighter" line-numbers></prism-editor>
        </v-card-text>
        <v-card-actions class="text-center justify-center">
            <v-btn elevation="2" @click="saveToJson">
                <v-icon left>mdi-content-save</v-icon>
                Save to JSON
            </v-btn>
            <v-btn elevation="2" @click="importCommand">
                <v-icon left>mdi-application-import</v-icon>
                Import Command to System
            </v-btn>
            <v-btn elevation="2" color="grey lighten-1" @click="reset">
                <v-icon left>mdi-restart</v-icon>
                Reset Form
            </v-btn>
        </v-card-actions>
    </v-card>
</template>

<script>
import { PrismEditor } from 'vue-prism-editor';
import 'vue-prism-editor/dist/prismeditor.min.css';

import { highlight, languages } from 'prismjs/components/prism-core';
import 'prismjs/components/prism-r';
import 'prismjs/themes/prism-coy.css';

import { saveAs } from 'file-saver';
import slugify from 'slugify';

export default {
    name: 'CmdEditor',
    components: {
        PrismEditor,
    },
    data: () => ({
        param_counter: 0,
        param_types: [
            {
                text: "Number",
                value: "number",
            },
            {
                text: "Text",
                value: "text",
            },
            {
                text: "Dataset Column",
                value: "select_headers",
            },
        ],
        cmd: {
            name: "",
            description: "",
            params: [
                {
                    id: 0,
                    name: "Argument 0",
                    var_name: "argument_0",
                    type: "number",
                    description: "",
                    default_value: "",
                },
            ],
            code: "",
        }
    }),
    methods: {
        highlighter(code) {
            return highlight(code, languages.r)
        },
        addParam: function() {
            let cnt = this.cmd.params.length;
            this.param_counter += 1;

            this.cmd.params.push({
                id: this.param_counter,
                name: "Argument " + (cnt),
                var_name: "argument_" + (cnt),
                type: "number",
                description: "",
                default_value: "",
            });
        },
        removeParam: function(i) {
            this.cmd.params.splice(i, 1);
        },
        saveToJson: function() {
            let file_name = slugify(this.cmd.name, { lower: true }) + ".json";
            let blob = new Blob([JSON.stringify([this.cmd], null, 2)], {
                type: 'application/json',
                name: file_name
            });
            saveAs(blob, file_name);
        },
        importCommand: function() {
            //rework to vuex later..
            let saved = localStorage.getItem('loaded_cmds');
            let obj = (saved ? JSON.parse(saved) : []);
            obj.push(this.cmd);
            let saved_new = JSON.stringify(obj);

            localStorage.setItem('loaded_cmds', saved_new);
            this.$root.$refs.Alert.show('Command imported successfully.');
        },
        reset: function() {
            this.param_counter = 0,
            this.cmd = {
                name: "",
                description: "",
                params: [
                    {
                        id: 0,
                        name: "Argument 0",
                        var_name: "argument_0",
                        type: "number",
                        description: "",
                        default_value: "",
                    },
                ],
                code: "",
            }
        },
    }
}
</script>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Fira+Code&display=swap');

.my-editor {
    background: #fafafa;
    color: #000;
    font-family: Fira code, Fira Mono, Consolas, Menlo, Courier, monospace;
    font-size: 16px;
    line-height: 1.5;
    padding: 5px;
}
</style>

<style>
.prism-editor__textarea:focus {
    outline: none;
    border: 0;
}
</style>