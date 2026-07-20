<script setup lang="ts">
import { ref, onMounted, computed } from 'vue';
import apiClient from '@/api/axios';

interface Flower {
  id: number;
  name: string;
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
const showCart = ref(false); // Управление модальным окном корзины
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
          
          <div class="hero-contacts">
            <p>📍 г. Пермь, ул. Маршала Рыбалко 81а</p>
            <p>🕒 Ежедневно 10:00 - 21:00</p>
          </div>

          <div class="hero-actions">
            <a href="tel:+79655556569" class="btn btn-primary">Позвонить</a>
            <a href="https://vk.com/market-43923180" target="_blank" class="btn btn-outline">ВКонтакте</a>
            <button class="btn btn-text-only" @click="scrollToCatalog">Коллекция ↓</button>
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
        <button class="btn btn-primary" style="margin-top: 20px" @click="orderSuccess = false">Продолжить покупки</button>
      </div>

      <!-- Grid -->
      <div v-if="loading" class="loading">Создаем красоту...</div>
      
      <div v-else-if="!showOrderForm && !orderSuccess" class="bento-grid">
        <article class="bento-card" v-for="(flower, index) in flowers" :key="flower.id" 
                 :class="{'bento-large': index === 0 || index === 3}">
          <div class="card-img-wrap">
            <img :src="getImageUrl(flower)" :alt="flower.name" class="card-img" />
            <div class="card-overlay">
              <button class="add-btn" @click="addToCart(flower)" aria-label="Добавить">В корзину</button>
            </div>
          </div>
          <div class="card-content">
            <h3 class="card-title">{{ flower.name }}</h3>
            <p class="card-price">{{ flower.price }} ₽</p>
          </div>
        </article>
      </div>
    </section>

    <!-- Job Banner moved BELOW catalog -->
    <section class="highlight-section container animate-fade-in">
      <div class="job-banner glass">
        <div class="job-content">
          <span class="tag">Команда</span>
          <h2>Ищем таланты</h2>
          <p>Мы находимся в поиске флориста. Опыт не обязателен, главное — чувство прекрасного.</p>
        </div>
        <div class="job-action">
          <a href="https://vk.com/market-43923180" target="_blank" class="btn btn-outline">Присоединиться</a>
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
            <button type="submit" class="btn btn-primary">Подтвердить</button>
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
      <button class="btn btn-primary">Посмотреть</button>
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
          <button class="btn btn-primary" style="width: 100%; margin-top: 20px;" 
                  @click="showCart = false; showOrderForm = true">Перейти к оформлению</button>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.home-wrapper {
  padding-bottom: 50px;
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

@keyframes slowPan {
  from { transform: scale(1.05) translateY(0); }
  to { transform: scale(1.1) translateY(-2%); }
}

.hero-overlay {
  position: absolute;
  top: 0; left: 0; width: 100%; height: 100%;
  background: linear-gradient(to right, rgba(248, 247, 245, 0.9) 0%, rgba(248, 247, 245, 0.4) 100%);
}

.hero-content {
  width: 100%;
  padding-top: 80px; 
}

.hero-text-box {
  max-width: 600px;
  padding: 50px;
  border-radius: var(--radius-lg);
  background: rgba(255, 255, 255, 0.7);
}

.hero-logo-wrap {
  margin-bottom: 30px;
}

.hero-logo {
  width: 150px; /* Увеличенный логотип */
  height: 150px;
  border-radius: 50%;
  box-shadow: var(--shadow-ambient);
  object-fit: cover;
}

.hero h1 {
  font-size: 4.5rem;
  margin-bottom: 20px;
  color: var(--color-text-main);
}

.hero-contacts {
  margin: 30px 0;
  font-size: 1.1rem;
  line-height: 1.8;
  color: var(--color-text-main);
  font-weight: 500;
}

.hero-actions {
  display: flex;
  gap: 15px;
  flex-wrap: wrap;
  align-items: center;
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
  margin-left: 10px;
}
.btn-text-only:hover {
  color: var(--color-accent-pink);
}

/* Highlight Banner */
.highlight-section {
  position: relative;
  z-index: 10;
  margin-top: 100px;
  margin-bottom: 120px;
}

.job-banner {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 50px 60px;
  border-radius: var(--radius-lg);
  background: rgba(255, 255, 255, 0.85);
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

.job-content h2 {
  font-size: 2.5rem;
  margin-bottom: 15px;
}

.job-content p {
  color: var(--color-text-muted);
  font-size: 1.1rem;
  max-width: 500px;
}

/* Section Header */
.section-header {
  text-align: center;
  margin-bottom: 80px;
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
  bottom: 0; left: 0; width: 100%; height: 100%;
  background: linear-gradient(to top, rgba(0,0,0,0.4) 0%, transparent 40%);
  opacity: 0;
  transition: opacity 0.4s ease;
  display: flex;
  align-items: flex-end;
  justify-content: center;
  padding: 30px;
}

.bento-card:hover .card-img {
  transform: scale(1.08);
}

.bento-card:hover .card-overlay {
  opacity: 1;
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
  transform: translateY(20px);
  transition: all 0.4s cubic-bezier(0.16, 1, 0.3, 1);
}

.bento-card:hover .add-btn {
  transform: translateY(0);
}

.add-btn:hover {
  background: var(--color-text-main);
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
  bottom: 40px;
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
  .job-banner { flex-direction: column; text-align: center; gap: 30px; padding: 40px 30px; }
  .cart-sticky { bottom: 20px; right: 20px; left: 20px; justify-content: space-between; }
}
</style>
