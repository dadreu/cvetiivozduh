<script setup lang="ts">
import { useAuthStore } from '@/stores/auth';
import { useRouter } from 'vue-router';

const authStore = useAuthStore();
const router = useRouter();

const logout = async () => {
  // Call API to logout in real scenario
  authStore.clearAuth();
  router.push('/login');
};
</script>

<template>
  <header class="header">
    <div class="container nav-container">
      <router-link to="/" class="logo">FlowerShop</router-link>
      <nav class="nav-links">
        <router-link to="/">Каталог</router-link>
        <router-link v-if="authStore.isAuthenticated" to="/admin">Админка</router-link>
        <button class="btn btn-primary" v-if="authStore.isAuthenticated" @click="logout">Выйти</button>
        <router-link class="btn btn-primary" v-else to="/login">Вход</router-link>
      </nav>
    </div>
  </header>
  
  <main class="main-content">
    <router-view></router-view>
  </main>
</template>

<style scoped>
.header {
  background-color: #fff;
  border-bottom: 1px solid var(--color-bg-alt);
  padding: var(--spacing-md) 0;
  position: sticky;
  top: 0;
  z-index: 100;
}

.nav-container {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.logo {
  font-size: 1.5rem;
  font-weight: 600;
  color: var(--color-text-main);
  letter-spacing: -0.5px;
}

.nav-links {
  display: flex;
  align-items: center;
  gap: var(--spacing-lg);
}

.nav-links a {
  font-weight: 500;
  color: var(--color-text-muted);
}

.nav-links a:hover,
.nav-links a.router-link-active {
  color: var(--color-text-main);
}

.main-content {
  padding: var(--spacing-xxl) 0;
}
</style>
