<template>
    <div>
        <v-card class="mx-auto my-2"
                max-width="640"
                outlined>
            <v-card-text>
                <div>Create a simple boxplot from imported Dataset (test).</div>
                <div>Select a formula (2 columns which you want to visualize).</div>
                <div class="row mt-6 mb-2">
                    <div class="col-12">
                        <v-select
                            v-model="actual_command"
                            :items="commands"
                            label="Command"
                            ></v-select>
                    </div>
                    <div class="col-12 col-md-5">
                        <v-select
                            v-model="formula_x"
                            :items="headers"
                            label="Column X"
                            dense
                            solo
                            hide-details
                            ></v-select>
                    </div>
                    <div class="col-12 col-md-2" style="font-size: 3rem; font-weight: 700; text-align: center;">~</div>
                    <div class="col-12 col-md-5">
                        <v-select
                            v-model="formula_y"
                            :items="headers"
                            label="Column Y"
                            dense
                            solo
                            hide-details
                            ></v-select>
                    </div>
                </div>
            </v-card-text>
            <v-card-actions>
                <v-btn block elevation="2" @click="doCompute" :disabled="!(headers && formula_x && formula_y !== null) || (formula_x === formula_y)">
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
const BASE_URL = 'http://localhost:50598/api';

export default {
    name: 'Compute',
    data: () => ({
        result: null,
        status: null,
        headers: null,
        formula_x: null,
        formula_y: null,
        actual_command: 'boxplot',
        commands: [
            {
                text:   'Boxplot',
                value:  'boxplot',
            },
            {
                text:   'ggplot2',
                value:  'ggplot2',
            },
        ],
    }),
    mounted: function() {
        this.headers = JSON.parse(localStorage.getItem('loaded_data_headers'));
        /* eslint no-console: ["error", { allow: ["warn", "error"] }] */
        console.warn(this.headers);
    },
    methods: {
        doCompute: function() {
            let url = BASE_URL + '/rlang/test1';
            let saved_data = localStorage.getItem('loaded_data');
            let saved_delimiter = localStorage.getItem('loaded_data_delimiter');
            let data = {
                delimiter:  saved_delimiter,
                command:    this.actual_command,
                formula_x:  this.formula_x,
                formula_y:  this.formula_y,
                dataset:    saved_data,
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
                this.$root.$refs.Alert.show('[API] Error: ' +  error, 'error');
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