import Vue from 'vue';
import VueRouter from 'vue-router';

import NotFound from '../views/NotFound.vue';
import Home from '../views/Home.vue';

Vue.use(VueRouter);

const routes = [
    {
        path: '/',
        name: 'Home',
        component: Home,
    },
    {
        path: '/import',
        name: 'Import',
        component: () => import(/* webpackChunkName: "import" */ '../views/Import.vue'),
    },
    {
        path: '/explorer',
        name: 'Dataset Explorer',
        component: () => import(/* webpackChunkName: "explorer" */ '../views/Explorer.vue'),
    },
    {
        path: '/compute',
        name: 'Compute',
        component: () => import(/* webpackChunkName: "compute" */ '../views/Compute.vue'),
    },
    {
        path: '/about',
        name: 'About',
        component: () => import(/* webpackChunkName: "about" */ '../views/About.vue'),
    },
    {
        path: '/:catchAll(.*)',
        component: NotFound,
        name: 'NotFound',
    },
];

const router = new VueRouter({
    mode: 'history',
    base: process.env.BASE_URL,
    routes,
});

router.beforeEach((to, from, next) => {
    document.title = 'Project R';
    next();
});

export default router;
