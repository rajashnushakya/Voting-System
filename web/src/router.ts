import { createRouter, createWebHistory } from 'vue-router';

const routes = [
  {
    path: '/',
    name: 'default',
    redirect:'/login'
  },
  {
    path: '/home',
    name: 'Home',
    component: () => import('./components/HelloWorld.vue'),
  },
  {
    path: '/login',
    name: 'Login',
    component: () => import('./components/LoginComponent.vue'),
  },
  {
    path: '/dashboard',
    name: 'Dashboard',
    component: () => import('./components/DashboardComponent.vue'),
  },
];

const router = createRouter({
  history: createWebHistory(),
  routes,
});

export default router;
