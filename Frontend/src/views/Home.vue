<script setup lang="ts">
import { ref, onMounted, computed } from 'vue';
import apiClient from '@/api/axios';

interface Flower {
  id: number;
  name: string;
  description: string;
  price: number;
  imageUrl: string;
}

interface CartItem extends Flower {
  quantity: number;
}

const flowers = ref<Flower[]>([]);
const cart = ref<CartItem[]>([]);
const loading = ref(true);

const showOrderForm = ref(false);
const showCart = ref(false); 
const customerName = ref('');
const customerPhone = ref('');
const deliveryAddress = ref('');
const orderSuccess = ref(false);

const fallbacks = [
  '/catalog/frenchRose.jpg',
  '/catalog/compose2.jpg',
  '/catalog/bee.jpg',
  '/catalog/gypsofils.jpg',
  '/catalog/i love u.jpg',
  '/catalog/Lylyi.jpg',
  '/catalog/bant.jpg',
  '/catalog/softy.jpg'
];

const getImageUrl = (flower: Flower) => {
  if (flower.imageUrl) return flower.imageUrl;
  return fallbacks[flower.id % fallbacks.length];
};

const getFallbackDesc = (flower: Flower) => {
  if (flower.description && flower.description !== 'Цветы и Воздух') return flower.description;
  const descs = [
    "Утонченная композиция для самых нежных чувств",
    "Яркий микс свежайших цветов, дарящий радость",
    "Классика, которая расскажет о вашей любви без слов",
    "Стильный монобукет в премиальной упаковке",
    "Воздушное облако из стойких цветов"
  ];
  return descs[flower.id % descs.length];
};

const cartTotal = computed(() => {
  return cart.value.reduce((total, item) => total + item.price * item.quantity, 0);
});

onMounted(async () => {
  try {
    const response = await apiClient.get('/catalog/flowers');
    flowers.value = response.data;
  } catch (error) {
    console.error("Failed to fetch catalog", error);
  } finally {
    loading.value = false;
  }
});

const addToCart = (flower: Flower) => {
  const existing = cart.value.find(item => item.id === flower.id);
  if (existing) {
    existing.quantity++;
  } else {
    cart.value.push({ ...flower, quantity: 1 });
  }
};

const removeFromCart = (flowerId: number) => {
  cart.value = cart.value.filter(item => item.id !== flowerId);
  if (cart.value.length === 0) {
    showCart.value = false;
    showOrderForm.value = false;
  }
};

const placeOrder = async () => {
  if (!customerName.value || !customerPhone.value) return;

  const orderData = {
    customerName: customerName.value,
    customerPhone: customerPhone.value,
    deliveryAddress: deliveryAddress.value,
    items: cart.value.map(item => ({
      flowerId: item.id,
      quantity: item.quantity
    }))
  };

  try {
    await apiClient.post('/catalog/order', orderData);
    orderSuccess.value = true;
    cart.value = [];
    showCart.value = false;
    showOrderForm.value = false;
  } catch (error) {
    console.error("Order failed", error);
    alert("Ошибка при оформлении заказа");
  }
};

const scrollToCatalog = () => {
  document.getElementById('catalog-section')?.scrollIntoView({ behavior: 'smooth' });
};
</script>

<template>
  <div class="home-wrapper">
    <!-- Immersive Hero Section -->
    <section class="hero animate-fade-in">
      <div class="hero-bg">
        <img src="/catalog/frenchRose.jpg" alt="Фон" class="hero-img" />
        <div class="hero-overlay"></div>
      </div>
      <div class="container hero-content">
        <div class="hero-text-box glass">
          <div class="hero-logo-wrap">
            <img src="@/images/logo.jpg" alt="Логотип Цветы и Воздух" class="hero-logo" />
          </div>
          <h1>Эмоции,<br/>воплощенные<br/>в цветах</h1>
          <p class="hero-desc">Студия Цветов и шаров в Перми. Мы создаём моменты истинной красоты и эстетики.</p>
          
          <div class="hero-actions">
            <button class="btn-premium" @click="scrollToCatalog">Коллекция ↓</button>
          </div>
        </div>
      </div>
    </section>

    <!-- Catalog Section -->
    <section id="catalog-section" class="container">
      <div class="section-header">
        <h2>Наша Коллекция</h2>
        <p>Искусство, которое можно подарить</p>
      </div>

      <div v-if="orderSuccess" class="success-message glass">
        <h2>Спасибо за ваш заказ!</h2>
        <p>Мы скоро свяжемся с вами для уточнения деталей доставки.</p>
        <button class="btn-premium" style="margin-top: 20px" @click="orderSuccess = false">Продолжить покупки</button>
      </div>

      <!-- Grid -->
      <div v-if="loading" class="loading">Создаем красоту...</div>
      
      <div v-else-if="!showOrderForm && !orderSuccess" class="bento-grid">
        <article class="bento-card" v-for="(flower, index) in flowers" :key="flower.id" 
                 :class="{'bento-large': index === 0 || index === 3}">
          <div class="card-img-wrap">
            <img :src="getImageUrl(flower)" :alt="flower.name" class="card-img" />
            <div class="card-overlay">
              <div class="overlay-content">
                <p class="flower-desc">{{ getFallbackDesc(flower) }}</p>
                <button class="add-btn" @click.stop="addToCart(flower)" aria-label="Добавить">В корзину</button>
              </div>
            </div>
          </div>
          <div class="card-content">
            <h3 class="card-title">{{ flower.name }}</h3>
            <p class="card-price">{{ flower.price }} ₽</p>
          </div>
        </article>
      </div>
    </section>

    <!-- Contacts & Socials -->
    <section id="contacts-section" class="container" style="margin-top: 100px;">
      <div class="section-split">
        <div class="info-block glass">
          <div class="section-header-small">
            <h2>Наши Контакты</h2>
            <p>Будем рады вас видеть</p>
          </div>
          <div class="info-content">
            <p style="margin-bottom: 5px;"><strong>Адрес:</strong> г. Пермь, ул. Маршала Рыбалко 81а</p>
            <a href="https://yandex.ru/maps/?text=Пермь,+ул.+Маршала+Рыбалко,+81а" target="_blank" class="map-link-text">
              <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M21 10c0 7-9 13-9 13s-9-6-9-13a9 9 0 0 1 18 0z"></path><circle cx="12" cy="10" r="3"></circle></svg>
              <span>Показать на Яндекс.Картах</span>
            </a>
            
            <p style="margin-top: 25px; margin-bottom: 5px;"><strong>Время работы:</strong> Ежедневно 10:00 - 21:00</p>
            
            <div style="margin-top: 15px;">
              <a href="tel:+79655556569" class="contact-phone-link">+7 (965) 555-65-69</a>
            </div>
          </div>
        </div>

        <div class="info-block glass">
          <div class="section-header-small">
            <h2>Мы в соцсетях</h2>
            <p>Эстетика в вашей ленте</p>
          </div>
          <div class="info-content">
            <p>Присоединяйтесь к нам, чтобы не пропустить свежие поставки и специальные предложения!</p>
            
            <div class="social-text-links">
              <a href="https://vk.com/market-43923180" target="_blank" class="social-text-link">ВКонтакте</a>
              <a href="#" target="_blank" class="social-text-link">Telegram</a>
              <a href="#" target="_blank" class="social-text-link">Instagram</a>
              <a href="#" target="_blank" class="social-text-link">WhatsApp</a>
            </div>
          </div>
        </div>
      </div>
    </section>

    <!-- Job Banner section (Redesigned) -->
    <section id="vacancies-section" class="container animate-fade-in" style="margin-top: 100px; margin-bottom: 80px;">
      <div class="section-header">
        <h2>Вакансии</h2>
        <p>Присоединяйтесь к нашей команде</p>
      </div>
      
      <div class="job-banner-luxury glass">
        <div class="job-img-side">
          <img src="/catalog/softy.jpg" alt="Работа флористом" />
        </div>
        <div class="job-content-side">
          <span class="tag">Флорист</span>
          <h2>Ищем таланты</h2>
          <p>Мы находимся в поиске человека, который любит цветы так же сильно, как мы. Опыт не обязателен — главное чувство прекрасного, а мы научим вас создавать настоящую эстетику.</p>
          <a href="https://vk.com/market-43923180" target="_blank" class="btn-premium">Откликнуться</a>
        </div>
      </div>
    </section>

    <!-- Checkout Form -->
    <div v-if="showOrderForm" class="container checkout-wrapper">
      <div class="checkout-card glass">
        <h3>Оформление заказа</h3>
        <form @submit.prevent="placeOrder">
          <div class="input-group">
            <input type="text" v-model="customerName" required placeholder="Ваше Имя" />
          </div>
          <div class="input-group">
            <input type="tel" v-model="customerPhone" required placeholder="Телефон" />
          </div>
          <div class="input-group">
            <input type="text" v-model="deliveryAddress" placeholder="Адрес доставки (необяз.)" />
          </div>
          <div class="checkout-actions">
            <button type="button" class="btn-text-only" @click="showOrderForm = false">Отмена</button>
            <button type="submit" class="btn-premium">Подтвердить</button>
          </div>
        </form>
      </div>
    </div>

    <!-- Sticky Cart Trigger -->
    <div v-if="cart.length > 0 && !showOrderForm && !orderSuccess" class="cart-sticky glass" @click="showCart = true">
      <div class="cart-info">
        <span class="cart-count">В корзине: {{ cart.reduce((acc, item) => acc + item.quantity, 0) }} шт.</span>
        <span class="cart-total">{{ cartTotal }} ₽</span>
      </div>
      <button class="btn-premium" style="padding: 12px 30px;">Посмотреть</button>
    </div>

    <!-- Cart Modal -->
    <div v-if="showCart" class="modal-overlay" @click.self="showCart = false">
      <div class="cart-modal glass">
        <div class="modal-header">
          <h3>Корзина</h3>
          <button @click="showCart = false" class="close-btn">×</button>
        </div>
        <div class="cart-items">
          <div v-for="item in cart" :key="item.id" class="cart-item">
            <img :src="getImageUrl(item)" class="cart-item-img" />
            <div class="cart-item-info">
              <h4>{{ item.name }}</h4>
              <p>{{ item.price }} ₽ x {{ item.quantity }}</p>
            </div>
            <button @click="removeFromCart(item.id)" class="remove-btn">Удалить</button>
          </div>
        </div>
        <div class="cart-modal-footer">
          <p class="cart-modal-total">Итого: <strong>{{ cartTotal }} ₽</strong></p>
          <button class="btn-premium" style="width: 100%; margin-top: 20px;" 
                  @click="showCart = false; showOrderForm = true">Перейти к оформлению</button>
        </div>
      </div>
    </div>
    
    <!-- Floating Action Button for Call -->
    <a href="tel:+79655556569" class="fab-call" aria-label="Позвонить нам">
      <div class="fab-wave"></div>
      <svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
        <path d="M22 16.92v3a2 2 0 0 1-2.18 2 19.79 19.79 0 0 1-8.63-3.07 19.5 19.5 0 0 1-6-6 19.79 19.79 0 0 1-3.07-8.67A2 2 0 0 1 4.11 2h3a2 2 0 0 1 2 1.72 12.84 12.84 0 0 0 .7 2.81 2 2 0 0 1-.45 2.11L8.09 9.91a16 16 0 0 0 6 6l1.27-1.27a2 2 0 0 1 2.11-.45 12.84 12.84 0 0 0 2.81.7A2 2 0 0 1 22 16.92z"></path>
      </svg>
    </a>
  </div>
</template>

<style scoped>
.home-wrapper {
  padding-bottom: 30px; 
}

/* Premium Button Unified */
.btn-premium {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  padding: 15px 40px;
  border-radius: 40px;
  border: 1px solid rgba(0,0,0,0.2);
  background: transparent;
  font-family: var(--font-family-base);
  text-transform: uppercase;
  letter-spacing: 2px;
  font-weight: 500;
  font-size: 0.9rem;
  cursor: pointer;
  color: var(--color-text-main);
  text-decoration: none;
  position: relative;
  overflow: hidden;
  z-index: 1;
  transition: all 0.4s ease;
}

.btn-premium::before {
  content: '';
  position: absolute;
  top: 0; left: 0; width: 100%; height: 100%;
  background: linear-gradient(135deg, #FFB6C1, #ff8cbf, #93a5cf, #e4efe9);
  background-size: 300% 300%;
  z-index: -1;
  transition: opacity 0.4s ease;
  opacity: 0;
  animation: gradientShift 5s ease infinite;
}

.btn-premium:hover {
  color: #fff;
  border-color: transparent;
  box-shadow: 0 15px 30px rgba(255, 182, 193, 0.4);
  transform: translateY(-3px);
}

.btn-premium:hover::before {
  opacity: 1;
}

@keyframes gradientShift {
  0% { background-position: 0% 50%; }
  50% { background-position: 100% 50%; }
  100% { background-position: 0% 50%; }
}

/* Immersive Hero */
.hero {
  position: relative;
  min-height: 100vh;
  display: flex;
  align-items: center;
  overflow: hidden;
  margin-bottom: 80px;
}

.hero-bg {
  position: absolute;
  top: 0; left: 0; width: 100%; height: 100%;
  z-index: -1;
}

.hero-img {
  width: 100%;
  height: 100%;
  object-fit: cover;
  object-position: center 30%;
  transform: scale(1.05);
  animation: slowPan 20s infinite alternate linear;
}

.hero-overlay {
  position: absolute;
  top: 0; left: 0; width: 100%; height: 100%;
  background: rgba(0,0,0,0.3); 
}

.hero-content {
  width: 100%;
  padding-top: 80px; 
  display: flex;
  justify-content: center; 
}

.hero-text-box {
  max-width: 650px;
  padding: 60px;
  border-radius: var(--radius-lg);
  background: rgba(255, 255, 255, 0.85); 
  text-align: center; 
  box-shadow: 0 30px 60px rgba(0,0,0,0.2);
}

.hero-logo-wrap {
  margin-bottom: 25px;
  display: flex;
  justify-content: center;
}

.hero-logo {
  width: 160px; 
  height: 160px;
  border-radius: 50%;
  box-shadow: var(--shadow-ambient);
  object-fit: cover;
  border: 4px solid #fff; 
}

.hero h1 {
  font-size: 4.5rem;
  margin-bottom: 20px;
  color: var(--color-text-main);
  line-height: 1.1;
}

.hero-desc {
  font-size: 1.1rem;
  color: var(--color-text-muted);
  margin-bottom: 30px;
  line-height: 1.6;
}

.hero-actions {
  display: flex;
  justify-content: center;
  margin-top: 20px;
}

.btn-text-only {
  background: none;
  border: none;
  font-size: 0.9rem;
  text-transform: uppercase;
  letter-spacing: 1px;
  cursor: pointer;
  color: var(--color-text-muted);
  transition: color 0.3s ease;
}
.btn-text-only:hover {
  color: var(--color-accent-pink);
}

/* Sections */
.section-header {
  text-align: center;
  margin-bottom: 60px;
}

.section-header h2 {
  font-size: 3.5rem;
  margin-bottom: 10px;
}

.section-header p {
  font-size: 1.1rem;
  color: var(--color-accent-blue);
  text-transform: uppercase;
  letter-spacing: 3px;
}

.section-header-small {
  margin-bottom: 30px;
}
.section-header-small h2 {
  font-size: 2.5rem;
  margin-bottom: 10px;
}
.section-header-small p {
  color: var(--color-accent-blue);
  text-transform: uppercase;
  letter-spacing: 2px;
  font-size: 0.9rem;
}

/* Contacts & Socials Split */
.section-split {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 40px;
}

.info-block {
  padding: 40px;
  border-radius: var(--radius-lg);
  background: rgba(255, 255, 255, 0.85);
  display: flex;
  flex-direction: column;
}

.info-content {
  flex: 1;
}

.info-content p {
  font-size: 1.1rem;
  margin-bottom: 15px;
  color: var(--color-text-main);
}

/* Map Text Link */
.map-link-text {
  display: inline-flex;
  align-items: center;
  gap: 8px;
  color: var(--color-text-muted);
  font-family: var(--font-family-base);
  font-size: 0.95rem;
  font-weight: 500;
  text-decoration: none;
  margin-top: 5px;
  transition: color 0.3s ease;
}
.map-link-text:hover {
  color: var(--color-accent-pink);
}

/* Big Contact Phone Link */
.contact-phone-link {
  font-size: 1.8rem;
  font-family: var(--font-family-base);
  color: var(--color-text-main);
  text-decoration: none;
  font-weight: 500;
  display: inline-block;
  margin-top: 15px;
  position: relative;
  z-index: 1;
  transition: color 0.3s ease, transform 0.3s ease;
}
.contact-phone-link::before {
  content: '';
  position: absolute;
  top: 50%; left: 50%;
  transform: translate(-50%, -50%);
  width: 120%; height: 160%;
  border-radius: 40px;
  background: radial-gradient(circle, rgba(220,163,183,0.15) 0%, rgba(220,163,183,0) 70%);
  z-index: -1;
  animation: soft-pulse 3s infinite cubic-bezier(0.4, 0, 0.2, 1);
  pointer-events: none;
}
.contact-phone-link:hover {
  color: var(--color-accent-pink);
  transform: translateY(-2px);
}

/* Minimalist Social Links */
.social-text-links {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 15px 10px;
  margin-top: 20px;
}

.social-text-link {
  font-family: var(--font-family-base);
  font-size: 1rem;
  color: var(--color-text-muted);
  text-decoration: none;
  position: relative;
  z-index: 1;
  width: fit-content;
  transition: color 0.3s ease;
  line-height: 1.2;
}

.social-text-link::before {
  content: '';
  position: absolute;
  top: 50%; left: 50%;
  transform: translate(-50%, -50%);
  width: 140%; height: 180%;
  border-radius: 20px;
  background: radial-gradient(circle, rgba(220,163,183,0.15) 0%, rgba(220,163,183,0) 70%);
  z-index: -1;
  animation: soft-pulse 3s infinite cubic-bezier(0.4, 0, 0.2, 1);
  animation-delay: 1.5s;
  pointer-events: none;
}

.social-text-link:hover {
  color: var(--color-accent-pink);
}


/* Luxury Job Banner */
.job-banner-luxury {
  display: flex;
  border-radius: var(--radius-lg);
  overflow: hidden;
  background: rgba(255, 255, 255, 0.85);
  box-shadow: var(--shadow-ambient);
}

.job-img-side {
  flex: 0 0 45%;
  position: relative;
}

.job-img-side img {
  position: absolute;
  top: 0; left: 0;
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.job-content-side {
  padding: 60px;
  flex: 1;
  display: flex;
  flex-direction: column;
  justify-content: center;
  align-items: flex-start;
}

.tag {
  display: inline-block;
  font-size: 0.7rem;
  text-transform: uppercase;
  letter-spacing: 2px;
  color: var(--color-accent-pink);
  font-weight: 600;
  margin-bottom: 15px;
}

.job-content-side h2 {
  font-size: 3rem;
  margin-bottom: 20px;
}

.job-content-side p {
  color: var(--color-text-muted);
  font-size: 1.1rem;
  line-height: 1.6;
  max-width: 90%;
  margin-bottom: 40px;
}

@media (max-width: 900px) {
  .job-banner-luxury {
    flex-direction: column;
  }
  .job-img-side {
    flex: none;
    height: 300px;
  }
  .job-content-side {
    padding: 40px 30px;
  }
}

/* Bento Grid */
.bento-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
  gap: 40px;
}

.bento-large {
  grid-column: span 2;
  grid-row: span 2;
}

@media (max-width: 900px) {
  .bento-large {
    grid-column: span 1;
    grid-row: span 1;
  }
  .section-split {
    grid-template-columns: 1fr;
  }
}

.bento-card {
  position: relative;
  border-radius: var(--radius-md);
  background: transparent;
  display: flex;
  flex-direction: column;
}

.card-img-wrap {
  position: relative;
  width: 100%;
  aspect-ratio: 4/5;
  border-radius: var(--radius-md);
  overflow: hidden;
  box-shadow: var(--shadow-ambient);
  margin-bottom: 20px;
  background: #fff;
}

.bento-large .card-img-wrap {
  aspect-ratio: 16/10;
}

.card-img {
  width: 100%;
  height: 100%;
  object-fit: cover;
  transition: transform 1.2s cubic-bezier(0.16, 1, 0.3, 1);
}

.card-overlay {
  position: absolute;
  top: 0; left: 0; width: 100%; height: 100%;
  background: rgba(0,0,0,0.6);
  opacity: 0;
  transition: opacity 0.4s ease;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 30px;
}

.overlay-content {
  text-align: center;
  transform: translateY(20px);
  transition: transform 0.4s cubic-bezier(0.16, 1, 0.3, 1);
}

.flower-desc {
  color: #fff;
  font-family: var(--font-family-base);
  font-size: 0.95rem;
  margin-bottom: 20px;
  line-height: 1.5;
  font-style: italic;
  padding: 0 20px;
}

.bento-card:hover .card-img {
  transform: scale(1.08);
}

.bento-card:hover .card-overlay {
  opacity: 1;
}

.bento-card:hover .overlay-content {
  transform: translateY(0);
}

.add-btn {
  background: rgba(255,255,255,0.9);
  backdrop-filter: blur(5px);
  border: none;
  padding: 12px 30px;
  border-radius: 30px;
  font-family: var(--font-family-base);
  text-transform: uppercase;
  letter-spacing: 1px;
  font-size: 0.8rem;
  font-weight: 500;
  cursor: pointer;
  color: var(--color-text-main);
  transition: all 0.4s cubic-bezier(0.16, 1, 0.3, 1);
}

.add-btn:hover {
  background: var(--color-accent-pink);
  color: #fff;
}

.card-content {
  padding: 0 10px;
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
}

.card-title {
  font-family: var(--font-family-heading);
  font-size: 1.4rem;
  margin: 0;
  max-width: 70%;
}

.card-price {
  font-family: var(--font-family-base);
  font-weight: 500;
  font-size: 1.1rem;
  color: var(--color-text-muted);
}

/* Cart Modal & Sticky */
.cart-sticky {
  position: fixed;
  bottom: 120px;
  right: 40px;
  padding: 20px 30px;
  border-radius: 40px;
  display: flex;
  align-items: center;
  gap: 30px;
  z-index: 90;
  background: rgba(255,255,255,0.85);
  box-shadow: var(--shadow-float);
  animation: fadeInUp 0.5s ease;
  cursor: pointer;
  transition: transform 0.3s ease;
}

.cart-sticky:hover {
  transform: translateY(-5px);
}

.cart-info {
  display: flex;
  flex-direction: column;
}

.cart-count {
  font-size: 0.8rem;
  text-transform: uppercase;
  letter-spacing: 1px;
  color: var(--color-text-muted);
}

.cart-total {
  font-size: 1.2rem;
  font-weight: 600;
}

/* Modal Layout */
.modal-overlay {
  position: fixed;
  top: 0; left: 0; width: 100%; height: 100%;
  background: rgba(0,0,0,0.4);
  backdrop-filter: blur(5px);
  z-index: 2000;
  display: flex;
  justify-content: flex-end;
}

.cart-modal {
  width: 100%;
  max-width: 450px;
  height: 100%;
  background: #fff;
  padding: 40px;
  display: flex;
  flex-direction: column;
  animation: slideInRight 0.4s cubic-bezier(0.16, 1, 0.3, 1);
}

@keyframes slideInRight {
  from { transform: translateX(100%); }
  to { transform: translateX(0); }
}

.modal-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 30px;
}

.modal-header h3 {
  font-size: 2rem;
  margin: 0;
}

.close-btn {
  background: none;
  border: none;
  font-size: 2rem;
  color: var(--color-text-muted);
  cursor: pointer;
  transition: color 0.3s ease;
}

.close-btn:hover {
  color: var(--color-text-main);
}

.cart-items {
  flex: 1;
  overflow-y: auto;
  display: flex;
  flex-direction: column;
  gap: 20px;
}

.cart-item {
  display: flex;
  align-items: center;
  gap: 15px;
  padding-bottom: 15px;
  border-bottom: 1px solid rgba(0,0,0,0.05);
}

.cart-item-img {
  width: 70px;
  height: 70px;
  object-fit: cover;
  border-radius: var(--radius-md);
}

.cart-item-info {
  flex: 1;
}

.cart-item-info h4 {
  font-size: 1.1rem;
  margin-bottom: 5px;
}

.cart-item-info p {
  color: var(--color-text-muted);
  font-size: 0.9rem;
}

.remove-btn {
  background: none;
  border: none;
  color: var(--color-accent-pink);
  font-size: 0.85rem;
  text-transform: uppercase;
  cursor: pointer;
}

.remove-btn:hover {
  text-decoration: underline;
}

.cart-modal-footer {
  padding-top: 30px;
  border-top: 1px solid rgba(0,0,0,0.1);
  margin-top: 20px;
}

.cart-modal-total {
  font-size: 1.3rem;
  display: flex;
  justify-content: space-between;
}

/* Checkout */
.checkout-wrapper {
  display: flex;
  justify-content: center;
  margin-top: 50px;
}

.checkout-card {
  width: 100%;
  max-width: 500px;
  padding: 50px;
  border-radius: var(--radius-lg);
}

.checkout-card h3 {
  font-size: 2rem;
  margin-bottom: 30px;
  text-align: center;
}

.input-group {
  margin-bottom: 20px;
}

.input-group input {
  width: 100%;
  padding: 16px 20px;
  background: rgba(255,255,255,0.5);
  border: 1px solid rgba(0,0,0,0.1);
  border-radius: var(--radius-md);
  font-family: var(--font-family-base);
  font-size: 1rem;
  transition: all 0.3s ease;
}

.input-group input:focus {
  outline: none;
  background: #fff;
  border-color: var(--color-accent-pink);
  box-shadow: 0 0 0 4px rgba(220, 163, 183, 0.1);
}

.checkout-actions {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-top: 40px;
}

.success-message {
  text-align: center;
  padding: 80px;
  border-radius: var(--radius-lg);
  margin-bottom: 80px;
}

.success-message h2 {
  font-size: 3rem;
}

.loading {
  text-align: center;
  padding: 100px 0;
  font-family: var(--font-family-heading);
  font-size: 2rem;
  font-style: italic;
  color: var(--color-text-muted);
}

@media (max-width: 768px) {
  .hero h1 { font-size: 3rem; }
  .hero-text-box { padding: 40px 30px; }
  .cart-sticky { bottom: 90px; right: 20px; left: 20px; justify-content: space-between; }
  .fab-call { bottom: 20px; right: 20px; width: 55px; height: 55px; }
}

@keyframes soft-pulse {
  0% { transform: translate(-50%, -50%) scale(0.8); opacity: 0; }
  50% { transform: translate(-50%, -50%) scale(1.1); opacity: 1; }
  100% { transform: translate(-50%, -50%) scale(1.3); opacity: 0; }
}

/* Floating Call Button */
.fab-call {
  position: fixed;
  bottom: 40px; 
  right: 40px;
  width: 65px;
  height: 65px;
  border-radius: 50%;
  background: var(--color-accent-pink);
  color: #fff;
  display: flex;
  align-items: center;
  justify-content: center;
  box-shadow: 0 10px 25px rgba(220, 163, 183, 0.5);
  z-index: 85;
  transition: transform 0.3s ease, background 0.3s ease;
}

.fab-call:hover {
  transform: scale(1.08) translateY(-5px);
  background: var(--color-accent-terracotta);
}

.fab-wave {
  position: absolute;
  top: 50%; left: 50%;
  transform: translate(-50%, -50%);
  width: 100%; height: 100%;
  border-radius: 50%;
  border: 2px solid var(--color-accent-pink);
  animation: pulse-ring 2s cubic-bezier(0.215, 0.61, 0.355, 1) infinite;
  z-index: -1;
}

@keyframes pulse-ring {
  0% { transform: translate(-50%, -50%) scale(1); opacity: 0.8; }
  100% { transform: translate(-50%, -50%) scale(2.2); opacity: 0; }
}
</style>
