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
  {
    path:'/registration',
    name:'Registration',
    component:()=>import ('./components/RegisttrationForm.vue'),
  },
  {
    path:'/electionDetails',
    name:'ElectionDetails',
    component:()=>import ('./components/ElectionDetail.vue'),
  },
  {
    path:'/addElection',
    name:'addElection',
    component:()=>import ('./components/childcomponents/AddElection.vue'),
  },
  {
    path:'/Election',
    name:'Election',
    component:()=>import ('./components/Election.vue'),
  },
  {
    path:'/log',
    name:'log',
    component:()=>import ('./components/Logs.vue'),
  },
  {
    path:'/result',
    name:'result',
    component:()=>import ('./components/Result.vue'),
  },



];

const router = createRouter({
  history: createWebHistory(),
  routes,
});

export default router;
