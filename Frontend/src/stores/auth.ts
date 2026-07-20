import { defineStore } from 'pinia';
import { ref } from 'vue';

export const useAuthStore = defineStore('auth', () => {
  const isAuthenticated = ref(localStorage.getItem('isAuthenticated') === 'true');

  function setAuth(status: boolean) {
    isAuthenticated.value = status;
    if (status) {
      localStorage.setItem('isAuthenticated', 'true');
    } else {
      localStorage.removeItem('isAuthenticated');
    }
  }

  function clearAuth() {
    isAuthenticated.value = false;
    localStorage.removeItem('isAuthenticated');
  }

  return { isAuthenticated, setAuth, clearAuth };
});
