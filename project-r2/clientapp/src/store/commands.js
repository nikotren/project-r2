import { defineStore } from 'pinia'

export const commandsStore = defineStore({
    id: 'CommandsStore',
    state: () =>({
        commands: [],
    }),
    persist: true,
    getters: {},
    actions: {},
});