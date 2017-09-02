import Vue from 'vue'
import Router from 'vue-router'
const _import = require('./_import_' + process.env.NODE_ENV)

Vue.use(Router)

/* layout */
import Layout from '../views/layout/Layout'

// export default new Router({
//     routes: [{
//         path: '/',
//         name: 'Hello',
//         component: Hello
//     }]
// })

export const constantRouterMap = [{
    path: '/login',
    component: _import('login/index'),
    hidden: true
}, {
    path: '/authredirect',
    component: _import('login/authredirect'),
    hidden: true
}, {
    path: '/404',
    component: _import('errorpage/404'),
    hidden: true
}, {
    path: '/401',
    component: _import('errorpage/401'),
    hidden: true
}, {
    path: '/',
    component: Layout,
    redirect: '/dashboard',
    name: '首页',
    hidden: true,
    children: [{
        path: 'dashboard',
        component: _import('dashboard/index')
    }]
}]

export default new Router({
    // mode: 'history', //后端支持可开
    scrollBehavior: () => ({
        y: 0
    }),
    routes: constantRouterMap
})