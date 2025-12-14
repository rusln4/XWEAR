import { createRouter, createWebHistory } from 'vue-router';
import MainPage from './views/MainPage.vue';
import AccountPage from './views/Account.vue';
import ProductDetail from './views/ProductDetail.vue';
import CatalogPage from './views/Catalog.vue';
import CartPage from './views/Cart.vue';

const routes = [
    {
        path: '/',
        redirect: '/mainpage'
    },
    {
        path: '/mainpage',
        name: 'MainPage',
        component: MainPage
    },
    {
        path: '/account',
        name: 'Account',
        component: AccountPage
    },
    {
        path: '/product/:id',
        name: 'ProductDetail',
        component: ProductDetail
    },
    {
        path: '/catalog',
        name: 'Catalog',
        component: CatalogPage
    }
    ,
    {
        path: '/cart',
        name: 'Cart',
        component: CartPage
    }
]

const router = createRouter({
    history: createWebHistory(),
    routes
});
export default router;
