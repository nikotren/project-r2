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
            <div class="my-2">
                <v-select
                    v-model="delim"
                    :items="delimiters"
                    label="Delimiter"
                ></v-select>
                <v-text-field
                    v-if="delim == 'custom'"
                    v-model="delim_custom"
                    label="Custom Delimiter"
                ></v-text-field>
            </div>
            <div class="my-2">
                <v-checkbox
                    v-model="papa_headers"
                    label="Skip headers (first row)"
                    hide-details
                ></v-checkbox>
                <v-checkbox
                    v-model="papa_quotes"
                    label="Force fields quotes"
                    hide-details
                ></v-checkbox>
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
import Papa from 'papaparse';

export default {
    name: 'Import',
    data: () => ({
        file: null,
        data: null,
        delim: 'auto',
        delim_custom: null,
        delimiters: [
            {
                text:   'Autodetect',
                value:  'auto',
            },
            {
                text:   'Semicolon ;',
                value:  ';',
            },
            {
                text:   'Comma ,',
                value:  ',',
            },
            {
                text:   'Space',
                value:  ' ',
            },
            {
                text:   'Dot .',
                value:  '.',
            },
            {
                text:   'Pipe |',
                value:  '|',
            },
            {
                text:   'Tab \\t',
                value:  '\t',
            },
            {
                text:   'Custom',
                value:  'custom',
            }
        ],
        papa_headers: false,
        papa_quotes: false,
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
                    //!!! Use Vuex later !!!
                    let cleaned = result.substring(result.indexOf(",") + 1);

                    let papaConfig = {
                        header: this.papa_headers,
                        quotes: this.papa_quotes,
                        dymamicTyping: true,
                        delimiter: "",
                    };

                    if(this.delim !== 'auto') {
                        let d = this.delim;
                        if(this.delim_custom !== null) {
                            d = this.delim_custom;
                        }
                        papaConfig['delimiter'] = d;
                    }

                    let decoded = atob(cleaned);
                    //let jsoned = ConvertCsvToJson.fieldDelimiter(d).formatValueByType().csvStringToJson(decoded);
                    let papi = Papa.parse(decoded, papaConfig);
                    let jsoned = papi['data'];
                    let headers = Object.keys(jsoned[0]);
                    //console.log(jsoned);
                    localStorage.setItem('loaded_data_json', JSON.stringify(jsoned));
                    localStorage.setItem('loaded_data_headers', JSON.stringify(headers));
                    localStorage.setItem('loaded_data_delimiter', papi['meta']['delimiter']);
                    localStorage.setItem('loaded_data', cleaned);
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