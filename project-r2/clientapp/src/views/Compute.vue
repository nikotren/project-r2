<template>
    <div>
        <v-card class="mx-auto my-2"
                max-width="640"
                outlined>
            <v-card-text>
                <div>Create a simple boxplot from imported Dataset (test).</div>
            </v-card-text>
            <v-card-actions>
                <v-btn block elevation="2" @click="doCompute">
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
    }),
    methods: {
        doCompute: function() {
            let url = BASE_URL + '/rlang/test1';
            let saved_data = localStorage.getItem('loaded_data');
            let data = {
                Dataset: saved_data,
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