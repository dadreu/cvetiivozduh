<script setup lang="ts">
import { useAuthStore } from '@/stores/auth';
import { useRouter } from 'vue-router';
import { ref, onMounted, onUnmounted } from 'vue';

const authStore = useAuthStore();
const router = useRouter();
const scrolled = ref(false);

const handleScroll = () => {
  scrolled.value = window.scrollY > 50;
};

onMounted(() => {
  window.addEventListener('scroll', handleScroll);
});

onUnmounted(() => {
  window.removeEventListener('scroll', handleScroll);
});

const logout = async () => {
  authStore.clearAuth();
  router.push('/login');
};
</script>

<template>
  <div class="app-wrapper">
    <header class="header" :class="{ 'header-scrolled': scrolled }">
      <div class="container nav-container">
        <router-link to="/" class="brand">
          <span class="brand-text">Цветы и Воздух</span>
        </router-link>
        <nav class="nav-links">
          <router-link to="/">Коллекция</router-link>
          <router-link v-if="authStore.isAuthenticated" to="/admin">Админка</router-link>
          <button class="nav-btn" v-if="authStore.isAuthenticated" @click="logout">Выйти</button>
          <router-link class="nav-btn" v-else to="/login">Вход</router-link>
        </nav>
      </div>
    </header>
    
    <main class="main-content">
      <router-view></router-view>
    </main>
    
    <footer class="footer">
      <div class="container">
        <div class="footer-top">
          <div class="footer-brand">
            <h2 class="footer-logo">Цветы и Воздух</h2>
            <p>Место, где мы создаём моменты радости, праздника и тонкой красоты.</p>
          </div>
          <div class="footer-links">
            <div class="link-column">
              <h4>Контакты</h4>
              <p>ул. Маршала Рыбалко 81а, Пермь</p>
              <p>+7 (965) 555-65-69</p>
              <p>ежедневно 10:00 - 21:00</p>
            </div>
            <div class="link-column">
              <h4>Социальные сети</h4>
              <a href="https://vk.com/market-43923180" target="_blank" class="social-link">ВКонтакте (@cvetiivozduh)</a>
              <a href="#" class="social-link">Telegram</a>
            </div>
          </div>
        </div>
        <div class="footer-bottom">
          <p>&copy; 2024 Цветы и Воздух. Студия Цветов и шаров. Все права защищены.</p>
        </div>
      </div>
    </footer>
  </div>
</template>

<style scoped>
.app-wrapper {
  display: flex;
  flex-direction: column;
  min-height: 100vh;
}

.header {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  z-index: 1000;
  padding: 30px 0;
  transition: all 0.5s cubic-bezier(0.16, 1, 0.3, 1);
  background: transparent;
}

.header-scrolled {
  padding: 15px 0;
  background: rgba(248, 247, 245, 0.85);
  backdrop-filter: blur(20px);
  -webkit-backdrop-filter: blur(20px);
  border-bottom: 1px solid rgba(255,255,255,0.3);
  box-shadow: var(--shadow-ambient);
}

.nav-container {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.brand {
  display: flex;
  align-items: center;
  gap: 12px;
}

.brand-text {
  font-family: var(--font-family-heading);
  font-size: 2rem;
  font-weight: 600;
  color: var(--color-text-main);
  letter-spacing: -0.5px;
}

.nav-links {
  display: flex;
  align-items: center;
  gap: 40px;
}

.nav-links a, .nav-btn {
  font-weight: 500;
  font-size: 0.85rem;
  text-transform: uppercase;
  letter-spacing: 2px;
  color: var(--color-text-main);
  background: none;
  border: none;
  cursor: pointer;
  position: relative;
  padding: 5px 0;
}

.nav-links a::after, .nav-btn::after {
  content: '';
  position: absolute;
  bottom: 0;
  left: 0;
  width: 0;
  height: 1px;
  background-color: var(--color-accent-pink);
  transition: width 0.4s cubic-bezier(0.16, 1, 0.3, 1);
}

.nav-links a:hover::after,
.nav-links a.router-link-active::after,
.nav-btn:hover::after {
  width: 100%;
}

.main-content {
  flex: 1;
}

/* Footer Premium */
.footer {
  background-color: #1D1F23;
  color: #fff;
  padding: 100px 0 40px;
  margin-top: 100px;
  position: relative;
  overflow: hidden;
}

.footer::before {
  content: '';
  position: absolute;
  top: -50%;
  right: -20%;
  width: 600px;
  height: 600px;
  background: radial-gradient(circle, rgba(134,174,196,0.1) 0%, transparent 70%);
  border-radius: 50%;
  pointer-events: none;
}

.footer-top {
  display: flex;
  justify-content: space-between;
  flex-wrap: wrap;
  gap: 60px;
  margin-bottom: 80px;
}

.footer-brand {
  max-width: 400px;
}

.footer-logo {
  color: #fff;
  font-size: 2.5rem;
  margin-bottom: 20px;
}

.footer-brand p {
  color: rgba(255,255,255,0.6);
  font-size: 1.1rem;
  line-height: 1.8;
}

.footer-links {
  display: flex;
  gap: 80px;
  flex-wrap: wrap;
}

.link-column h4 {
  color: #fff;
  font-family: var(--font-family-base);
  font-size: 0.85rem;
  text-transform: uppercase;
  letter-spacing: 2px;
  margin-bottom: 30px;
}

.link-column p {
  color: rgba(255,255,255,0.6);
  margin-bottom: 12px;
  font-size: 0.95rem;
}

.social-link {
  display: block;
  color: rgba(255,255,255,0.6);
  margin-bottom: 12px;
  font-size: 0.95rem;
}

.social-link:hover {
  color: var(--color-accent-pink);
  transform: translateX(5px);
}

.footer-bottom {
  border-top: 1px solid rgba(255,255,255,0.1);
  padding-top: 40px;
  text-align: center;
  color: rgba(255,255,255,0.4);
  font-size: 0.85rem;
}

@media (max-width: 768px) {
  .nav-links {
    display: none; /* In real app, add mobile menu */
  }
  .footer-links {
    gap: 40px;
  }
}
</style>
