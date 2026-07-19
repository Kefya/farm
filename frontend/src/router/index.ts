import { createRouter, createWebHistory } from 'vue-router';
import Login from '@/views/Login.vue';
import Register from '@/views/Register.vue';
import Farm from '@/views/Farm.vue';
import Shop from '@/views/Shop.vue';
import Inventory from '@/views/Inventory.vue';
import Leaderboard from '@/views/Leaderboard.vue';
import { useAuthStore } from '@/stores/auth';

const routes = [
  { path: '/login', component: Login },
  { path: '/register', component: Register },
  { path: '/', redirect: '/farm' },
  { path: '/farm', component: Farm, meta: { requiresAuth: true } },
  { path: '/shop', component: Shop, meta: { requiresAuth: true } },
  { path: '/inventory', component: Inventory, meta: { requiresAuth: true } },
  { path: '/leaderboard', component: Leaderboard },
];

const router = createRouter({ history: createWebHistory(), routes });

router.beforeEach(async (to, from) => {
  const auth = useAuthStore();
  if (to.meta.requiresAuth && !auth.isAuthenticated) {
    return { path: '/login', query: { next: to.fullPath } };
  }
});

export default router;