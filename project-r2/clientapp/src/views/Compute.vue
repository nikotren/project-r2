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
                            return-object
                            ></v-select>
                    </div>
                    <div v-if="actual_command.params.length > 0" class="col-12">
                        <h3>Parameters</h3>
                        <div v-for="param in actual_command.params" :key="param.json">
                            <v-text-field
                                v-if="param.type == 'number'" 
                                v-model="actual_values[param.json]" 
                                type="number"
                                :label="param.text"
                                clearable></v-text-field>
                            <v-text-field
                                v-else-if="param.type == 'text'"
                                v-model="actual_values[param.json]"
                                :label="param.text"
                                clearable></v-text-field>
                            <v-select 
                                v-else-if="param.type == 'select_headers'"
                                v-model="actual_values[param.json]"
                                :label="param.text"
                                :items="dataStore.headers"></v-select>
                        </div>
                    </div>
                </div>
            </v-card-text>
            <v-card-actions>
                <v-btn block elevation="2" @click="doCompute" :disabled="!(dataStore.headers || actual_command.value == 'empty')">
                    <v-icon left>mdi-calculator</v-icon>
                    Compute
                </v-btn>
            </v-card-actions>
        </v-card>
        <v-card v-if="result !== null" 
            class="mx-auto my-2"
            max-width="640"
            outlined>
            <v-card-text>
                <img v-bind:src="'data:image/png;base64,'+ result" class="result-image">
            </v-card-text>
        </v-card>
    </div>
</template>

<script>
import { commandsStore } from '@/store/commands';
import { datasetStore } from '@/store/dataset';

export default {
    name: 'Compute',
    data: () => ({
        result: null,
        status: null,
        formula_x: null,
        formula_y: null,
        actual_command: {
            text:   'Please select command',
            value:  'empty',
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
        doCompute: function() {
            let url = '/rlang/test1';
            let parameters = [];

            for (const [k, v] of Object.entries(this.actual_values)) {
                parameters.push({
                    name:   k,
                    value:  v
                });
            }

            let data = {
                delimiter:  this.dataStore.delimiter,
                command:    this.actual_command,
                params:     parameters,
                dataset:    this.dataStore.dataset,
            };
            /* eslint no-console: ["error", { allow: ["warn", "error"] }] */
            console.warn(data);

            this.$http.post(url, data).then(response => {
                /* eslint no-console: ["error", { allow: ["warn", "error"] }] */
                console.error(response);
                this.result = response.data.result;
                this.status = response.data.statusCode;
            }).catch(error => {
                /* eslint no-console: ["error", { allow: ["warn", "error"] }] */
                this.$root.$refs.Alert.show('[API] Error: ' +  error.message, 'error');
            });
        },
    }
}
</script>

<style>
.result-image {
    display: block;
    margin: 0 auto;
}
</style>