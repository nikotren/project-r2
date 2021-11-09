<template>
    <v-alert v-model="alert" class="alert" :type="type" transition="slide-y-transition" dismissible>
        {{ message }}
    </v-alert>
</template>
<script>
export default {
    name: 'Alert',
    data: () => ({
        alert: false,
        type: 'info',
        message: '',
        timeout: null,
    }),
    created() {
        this.$root.$refs.Alert = this;
    },
    methods: {
        show: function (msg, type = 'info') {
            if (msg.length > 0) {
                this.message = msg;
                this.type = type;
                this.alert = true;
                clearTimeout(this.timeout);
                this.timeout = setTimeout(() => {
                    this.alert = false;
                }, 5000);
            }
        },
    },
};
</script>

<style scoped>
.alert {
    position: fixed;
    left: 50%;
    bottom: 2rem;
    transform: translate(-50%, -50%);
    margin: 0 auto;
    z-index: 1000;
}
</style>
