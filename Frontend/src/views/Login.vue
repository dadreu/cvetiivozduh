<script setup lang="ts">
import { ref } from 'vue';
import { useRouter } from 'vue-router';
import { useAuthStore } from '@/stores/auth';
import apiClient from '@/api/axios';

const username = ref('');
const password = ref('');
const errorMsg = ref('');
const loading = ref(false);

const router = useRouter();
const authStore = useAuthStore();

const handleLogin = async () => {
  errorMsg.value = '';
  loading.value = true;
  try {
    await apiClient.post('/auth/login', {
      username: username.value,
      password: password.value
    });
    authStore.setAuth(true);
    router.push('/admin');
  } catch (error: any) {
    if (error.response && error.response.status === 401) {
      errorMsg.value = 'Неверный логин или пароль';
    } else {
      errorMsg.value = 'Ошибка сервера';
    }
  } finally {
    loading.value = false;
  }
};
</script>

<template>
  <div class="container login-container">
    <div class="card login-card">
      <h2>Вход в панель управления</h2>
      <form @submit.prevent="handleLogin">
        <div class="form-group">
          <label>Логин</label>
          <input type="text" v-model="username" required />
        </div>
        <div class="form-group">
          <label>Пароль</label>
          <input type="password" v-model="password" required />
        </div>
        <div v-if="errorMsg" class="error">{{ errorMsg }}</div>
        <button class="btn btn-primary w-100" type="submit" :disabled="loading">
          {{ loading ? 'Загрузка...' : 'Войти' }}
        </button>
      </form>
    </div>
  </div>
</template>

<style scoped>
.login-container {
  display: flex;
  justify-content: center;
  align-items: center;
  min-height: 60vh;
}

.login-card {
  width: 100%;
  max-width: 400px;
}

.login-card h2 {
  text-align: center;
  margin-bottom: var(--spacing-lg);
}

.form-group {
  margin-bottom: var(--spacing-md);
}

.form-group label {
  display: block;
  margin-bottom: var(--spacing-xs);
  font-weight: 500;
  color: var(--color-text-main);
}

.form-group input {
  width: 100%;
  padding: var(--spacing-sm);
  border: 1px solid var(--color-secondary-beige);
  border-radius: var(--radius-sm);
  font-family: var(--font-family-base);
}

.form-group input:focus {
  outline: none;
  border-color: var(--color-secondary-dusty-blue);
}

.error {
  color: var(--color-accent-terracotta);
  margin-bottom: var(--spacing-md);
  text-align: center;
}

.w-100 {
  width: 100%;
}
</style>
