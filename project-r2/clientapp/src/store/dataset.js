import { defineStore } from 'pinia'

export const datasetStore = defineStore({
  id: 'DatasetStore',
  state: () =>({
    dataset: [],
    headers: [],
    raw_csv: '',
  }),
  persist: true,
  getters: {},
  actions: {},
});