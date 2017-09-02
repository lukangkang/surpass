// The Vue build version to load with the `import` command
// (runtime-only or standalone) has been set in webpack.base.conf with an alias.
import Vue from 'vue'
import App from './App'
import router from './router'
import components from './components';

Object.keys(components).forEach(e => Vue.component(e, components[e]));

Vue.config.productionTip = false
initVue();

function initVue() {
    return new Vue({
        router,
        render: h => h(App)
    }).$mount('#app');
}