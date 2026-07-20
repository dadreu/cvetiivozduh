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

const scrollTo = (id: string) => {
  const el = document.getElementById(id);
  if (el) {
    el.scrollIntoView({ behavior: 'smooth' });
  } else {
    // If not on home page, route to home then scroll (simple fallback)
    router.push('/').then(() => {
      setTimeout(() => {
        document.getElementById(id)?.scrollIntoView({ behavior: 'smooth' });
      }, 300);
    });
  }
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
          <a href="javascript:void(0)" @click="scrollTo('catalog-section')">Коллекция</a>
          <a href="javascript:void(0)" @click="scrollTo('vacancies-section')">Вакансии</a>
          <a href="javascript:void(0)" @click="scrollTo('contacts-section')">Контакты</a>
          
          <router-link v-if="authStore.isAuthenticated" to="/admin">Админка</router-link>
          <button class="nav-btn" v-if="authStore.isAuthenticated" @click="logout">Выйти</button>
        </nav>

        <div class="header-phone">
          <a href="tel:+79655556569" class="phone-link">+7 (965) 555-65-69</a>
        </div>
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
            <p>Пермь, ул. Маршала Рыбалко 81а<br/>Место, где мы создаём моменты радости, праздника и тонкой красоты.</p>
          </div>
          <div class="footer-links">
            <div class="link-column">
              <h4>Покупателям</h4>
              <a href="javascript:void(0)" @click="scrollTo('catalog-section')" class="footer-link">Наша Коллекция</a>
              <a href="#" class="footer-link">Оплата и доставка</a>
              <a href="#" class="footer-link">Условия возврата</a>
            </div>
            <div class="link-column">
              <h4>Компания</h4>
              <a href="javascript:void(0)" @click="scrollTo('contacts-section')" class="footer-link">Наши Контакты</a>
              <a href="javascript:void(0)" @click="scrollTo('vacancies-section')" class="footer-link">Вакансии флориста</a>
              <a href="#" class="footer-link">О нас</a>
            </div>
            <div class="link-column">
              <h4>Связь</h4>
              <p>Ежедневно 10:00 - 21:00</p>
              <a href="tel:+79655556569" class="footer-phone">+7 (965) 555-65-69</a>
              <a href="https://vk.com/market-43923180" target="_blank" class="footer-vk">Мы ВКонтакте</a>
            </div>
          </div>
        </div>
        <div class="footer-bottom">
          <p>&copy; 2024 Цветы и Воздух. Студия Цветов и шаров. Все права защищены. Разработано с душой.</p>
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
  padding: 25px 0;
  transition: all 0.5s cubic-bezier(0.16, 1, 0.3, 1);
  background: transparent;
}

.header-scrolled {
  padding: 15px 0;
  background: rgba(253, 251, 249, 0.95);
  backdrop-filter: blur(20px);
  -webkit-backdrop-filter: blur(20px);
  border-bottom: 1px solid rgba(0,0,0,0.05);
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
  font-size: 1.8rem;
  font-weight: 600;
  color: #fff;
  letter-spacing: -0.5px;
  text-shadow: 0 2px 4px rgba(0,0,0,0.5);
  transition: color 0.3s ease;
}

.header-scrolled .brand-text {
  color: var(--color-text-main);
  text-shadow: none;
}

.nav-links {
  display: flex;
  align-items: center;
  gap: 30px;
}

.nav-links a, .nav-btn {
  font-weight: 500;
  font-size: 0.8rem;
  text-transform: uppercase;
  letter-spacing: 1.5px;
  color: #fff;
  text-shadow: 0 1px 3px rgba(0,0,0,0.5);
  background: none;
  border: none;
  cursor: pointer;
  position: relative;
  padding: 5px 0;
  transition: color 0.3s ease;
}

.header-scrolled .nav-links a,
.header-scrolled .nav-btn {
  color: var(--color-text-main);
  text-shadow: none;
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

.header-phone {
  display: flex;
  align-items: center;
}

.phone-link {
  font-family: var(--font-family-base);
  font-weight: 600;
  font-size: 0.95rem;
  color: #fff;
  border: 1px solid rgba(255,255,255,0.5);
  background: rgba(0,0,0,0.2);
  padding: 8px 20px;
  border-radius: 30px;
  transition: all 0.3s ease;
  backdrop-filter: blur(5px);
}

.header-scrolled .phone-link {
  color: var(--color-text-main);
  border-color: var(--color-text-main);
  background: transparent;
}

.phone-link:hover {
  background: #fff;
  color: var(--color-text-main);
}

.header-scrolled .phone-link:hover {
  background: var(--color-text-main);
  color: #fff;
}

.main-content {
  flex: 1;
}

/* Footer Premium */
.footer {
  background-color: #1a1a1a;
  color: #fff;
  padding: 80px 0 30px;
  margin-top: 50px; /* Reduced from 100px */
}

.footer-top {
  display: flex;
  justify-content: space-between;
  flex-wrap: wrap;
  gap: 40px;
  margin-bottom: 60px;
}

.footer-brand {
  max-width: 350px;
}

.footer-logo {
  color: #fff;
  font-size: 2rem;
  margin-bottom: 15px;
}

.footer-brand p {
  color: rgba(255,255,255,0.5);
  font-size: 1rem;
  line-height: 1.6;
}

.footer-links {
  display: flex;
  gap: 80px;
  flex-wrap: wrap;
}

.link-column h4 {
  color: #fff;
  font-family: var(--font-family-base);
  font-size: 0.8rem;
  text-transform: uppercase;
  letter-spacing: 2px;
  margin-bottom: 25px;
}

.link-column p {
  color: rgba(255,255,255,0.6);
  margin-bottom: 12px;
  font-size: 0.9rem;
}

.footer-link {
  display: block;
  color: rgba(255,255,255,0.6);
  margin-bottom: 12px;
  font-size: 0.9rem;
  transition: transform 0.3s ease, color 0.3s ease;
}

.footer-link:hover {
  color: var(--color-accent-pink);
  transform: translateX(5px);
}

.footer-bottom {
  border-top: 1px solid rgba(255,255,255,0.05);
  padding-top: 30px;
  text-align: center;
  color: rgba(255,255,255,0.3);
  font-size: 0.8rem;
}

.footer-phone {
  display: block;
  color: var(--color-accent-pink);
  font-size: 1.1rem;
  font-weight: 600;
  margin: 15px 0;
  transition: color 0.3s ease;
}
.footer-phone:hover {
  color: #fff;
}

.footer-vk {
  display: inline-block;
  color: #0077FF;
  font-weight: 600;
  font-size: 1rem;
  background: rgba(0, 119, 255, 0.1);
  padding: 5px 12px;
  border-radius: 6px;
  transition: all 0.3s ease;
}
.footer-vk:hover {
  background: #0077FF;
  color: #fff;
}

@media (max-width: 900px) {
  .nav-links {
    display: none;
  }
  .header-phone {
    display: none;
  }
}
</style>
