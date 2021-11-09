<template>
    <v-card class="mx-auto"
            max-width="640"
            outlined>
        <v-card-text>
            <div>Use input below to import dataset from you computer.</div>
            <div class="my-2">
                <v-file-input
                    v-model="file"
                    ref="myFiles"
                    accept=".txt, .csv, .json"
                    label="Dataset"
                    hint="Supported formats are .txt, .csv, .json"
                    persistent-hint
                    outlined
                    show-size
                    truncate-length="15"
                ></v-file-input>
            </div>
        </v-card-text>
        <v-card-actions>
            <v-btn block elevation="2" @click="loadData">
                Import
            </v-btn>
        </v-card-actions>
    </v-card>
</template>

<script>
export default {
    name: 'Import',
    data: () => ({
        file: null,
        data: null,
    }),
    methods: {
        loadData: function() {
            const accepted = ['csv', 'txt', 'json'];

            if(!this.file) {
                this.$root.$refs.Alert.show('You did not choose a file', 'error');
                return;
            }

            let ext = this.file.name.split('.').pop();
            if (accepted.includes(ext)) {
                const reader = new FileReader();
                reader.onerror = (err) => {
                    this.$root.$refs.Alert.show('Error while importing: ' + err, 'error');
                }

                reader.onload = (res) => {
                    let result = String(res.target.result);
                    //data:*/*;base64, removal
                    //Use Vuex later
                    localStorage.setItem('loaded_data', result.substring(result.indexOf(",") + 1));
                    //console.log(this.$loaded_data);
                };
                
                reader.readAsDataURL(this.file, 'UTF-8');
                this.$root.$refs.Alert.show('Dataset imported successfully.');
            } else {
                this.$root.$refs.Alert.show('Unsupported file format.', 'error');
            }
        },
    }
}
</script>