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
    path:'/log',
    name:'log',
    component:()=>import ('./components/Logs.vue'),
  },
  {
    path:'/result',
    name:'result',
    component:()=>import ('./components/Result.vue'),
  },
  {
    path:'/dashboard/voter',
    name:'voter-dashboard',
    component:()=>import ('./components/VoterDashboard.vue'),
    props: true,
  },
  {
    path:'/enroll/:electionId',
    name:'voter-enroll',
    component:()=>import ('./components/ElectionCenterEnrollment.vue'),
    props: true,
  },

    {
      path: '/vote/:electionId',
      name: 'vote',
      component:()=> import('./components/VotingInterface.vue'),
    },
    
    {
      path: '/vote/history',
      name: 'vote-history',
      component:()=> import('./components/VotingHistory.vue'),
    },
        
    {
      path: '/forgot-password',
      name: 'forgot-password',
      component:()=> import('./components/ForgotPassword.vue'),
    },

    {
      path: '/election/centre/detail/:electionId',
      name: 'election-centre-detail',
      component:()=> import('./components/childcomponents/ElectionDetails.vue'),
    },
    {
      path: '/election/detail',
      name: 'election-detail',
      component:()=> import('./components/childcomponents/ElectionTable.vue'),
    },
        {
      path: '/voter/result',
      name: 'VoterResult',
      component:()=> import('./components/VoterResult.vue'),
    }

  



];

const router = createRouter({
  history: createWebHistory(),
  routes,
});

export default router;
