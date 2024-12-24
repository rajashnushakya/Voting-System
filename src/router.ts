import { createRouter, createWebHistory } from 'vue-router';
import DashboardComponent from './components/DashboardComponent.vue';
import LoginComponent from './components/LoginComponent.vue';

const routes = [
 
  {
    path: '/dashboard',
    name: 'Dashboard',
    component: DashboardComponent
  },
  {
    path: '/',
    name: 'Home',
    component: () => LoginComponent
  },
];

const router = createRouter({
  history: createWebHistory(),
  routes,
});

export default router;
