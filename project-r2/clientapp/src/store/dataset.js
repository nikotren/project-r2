import { defineStore } from 'pinia'

export const datasetStore = defineStore({
  id: 'DatasetStore',
  state: () =>({
    dataset: [],
    headers: [],
    delimiter: '',
  }),
  persist: true,
  getters: {},
  actions: {},
});