<template>
    <v-container>
        <v-card class="mx-auto"
                max-width="640"
                outlined>
            <v-card-text>
                <div>Hello, type a command to run by Rlang engine: (for ie. <span style="font-style: italic;">exp(pi)</span>)</div>
                <div class="my-2">
                    <v-text-field v-model="cmd"
                                  label="Type your command.."
                                  required
                                  hide-details="auto"></v-text-field>
                </div>
            </v-card-text>
            <v-card-actions>
                <v-btn block elevation="2" @click="requestApi">
                    Send
                </v-btn>
            </v-card-actions>
        </v-card>
        <div v-if="result" class="my-2">Result is: {{ result }}</div>
        <div v-if="status" class="my-2">Latest status code: {{ status }}</div>
    </v-container>
</template>

<script>
const BASE_URL = 'http://localhost:50598/api';

export default {
    name: 'Home',

    data: () => ({
        cmd: "",
        result: null,
        status: null,
    }),
    methods: {
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
        }
    }
};
</script>