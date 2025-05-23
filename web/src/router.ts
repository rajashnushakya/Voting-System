import { createRouter, createWebHistory } from 'vue-router';

const routes = [
  {
    path: '/',
    name: 'default',
    redirect: '/login'
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
    meta: { requiresAuth: true }
  },
  {
    path: '/registration',
    name: 'Registration',
    component: () => import('./components/RegisttrationForm.vue'),
  },
  {
    path: '/log',
    name: 'log',
    component: () => import('./components/Logs.vue'),
    meta: { requiresAuth: true }
  },
  {
    path: '/result',
    name: 'result',
    component: () => import('./components/Result.vue'),
    meta: { requiresAuth: true }
  },
  {
    path: '/dashboard/voter',
    name: 'voter-dashboard',
    component: () => import('./components/VoterDashboard.vue'),
    props: true,
    meta: { requiresAuth: true }
  },
  {
    path: '/enroll/:electionId',
    name: 'voter-enroll',
    component: () => import('./components/ElectionCenterEnrollment.vue'),
    props: true,
    meta: { requiresAuth: true }
  },
  {
    path: '/vote/:electionId',
    name: 'vote',
    component: () => import('./components/VotingInterface.vue'),
    meta: { requiresAuth: true }
  },
  {
    path: '/vote/history',
    name: 'vote-history',
    component: () => import('./components/VotingHistory.vue'),
    meta: { requiresAuth: true }
  },
  {
    path: '/forgot-password',
    name: 'forgot-password',
    component: () => import('./components/ForgotPassword.vue'),
  },
  {
    path: '/election/centre/detail/:electionId',
    name: 'election-centre-detail',
    component: () => import('./components/childcomponents/ElectionDetails.vue'),
    meta: { requiresAuth: true }
  },
  {
    path: '/election/detail',
    name: 'election-detail',
    component: () => import('./components/childcomponents/ElectionTable.vue'),
    meta: { requiresAuth: true }
  },
  {
    path: '/voter/result',
    name: 'VoterResult',
    component: () => import('./components/VoterResult.vue'),
    meta: { requiresAuth: true }
  }
];

const router = createRouter({
  history: createWebHistory(),
  routes,
});

//  Global Navigation Guard
router.beforeEach((to, from, next) => {
  const isAuthenticated = !!localStorage.getItem('token'); // Check token (adjust if needed)

  if (to.meta.requiresAuth && !isAuthenticated) {
    next('/login'); // Redirect to login if not authenticated
  } else {
    next(); // Proceed to route
  }
});

export default router;
