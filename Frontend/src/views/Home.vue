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
const customerName = ref('');
const customerPhone = ref('');
const deliveryAddress = ref('');
const orderSuccess = ref(false);

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
    showOrderForm.value = false;
  } catch (error) {
    console.error("Order failed", error);
    alert("Ошибка при оформлении заказа");
  }
};
</script>

<template>
  <div class="container">
    <div class="hero">
      <h1>Цветы и Воздух</h1>
      <p class="subtitle">место, где мы создаём моменты радости, праздника и тонкой красоты</p>
      <div class="contact-info">
        <p>г. Пермь, ул. Маршала Рыбалко, 81</p>
        <p>8 (965) 555 65 69 | 8 (993) 195 65 69</p>
      </div>
    </div>

    <div v-if="orderSuccess" class="success-message card">
      <h2>Спасибо за заказ!</h2>
      <p>Мы свяжемся с вами в ближайшее время для подтверждения.</p>
      <button class="btn btn-primary" @click="orderSuccess = false">Продолжить покупки</button>
    </div>

    <div v-if="cart.length > 0 && !showOrderForm && !orderSuccess" class="cart-summary card">
      <h3>Корзина ({{ cart.reduce((acc, item) => acc + item.quantity, 0) }} шт)</h3>
      <p>Итого: <strong>{{ cartTotal }} ₽</strong></p>
      <button class="btn btn-accent" @click="showOrderForm = true">Оформить заказ</button>
    </div>

    <div v-if="showOrderForm" class="order-form card">
      <h2>Оформление заказа</h2>
      <form @submit.prevent="placeOrder">
        <div class="form-group">
          <label>Имя</label>
          <input type="text" v-model="customerName" required />
        </div>
        <div class="form-group">
          <label>Телефон</label>
          <input type="tel" v-model="customerPhone" required />
        </div>
        <div class="form-group">
          <label>Адрес доставки (необязательно)</label>
          <input type="text" v-model="deliveryAddress" />
        </div>
        <div class="form-actions">
          <button type="button" class="btn btn-text" @click="showOrderForm = false">Назад</button>
          <button type="submit" class="btn btn-accent">Подтвердить заказ</button>
        </div>
      </form>
    </div>

    <div v-if="loading" class="loading">Загрузка...</div>
    
    <div v-else-if="!showOrderForm && !orderSuccess" class="catalog">
      <div class="card flower-card" v-for="flower in flowers" :key="flower.id">
        <div class="flower-image">
          <img :src="flower.imageUrl || '/placeholder.webp'" :alt="flower.name" />
        </div>
        <div class="flower-info">
          <h3>{{ flower.name }}</h3>
          <p class="price">{{ flower.price }} ₽</p>
          <button class="btn btn-primary" @click="addToCart(flower)">В корзину</button>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.hero {
  text-align: center;
  margin-bottom: var(--spacing-xxl);
}

.hero h1 {
  font-size: 3rem;
  color: var(--color-text-main);
  margin-bottom: var(--spacing-sm);
  letter-spacing: -1px;
}

.hero p.subtitle {
  font-size: 1.25rem;
  color: var(--color-text-muted);
  margin-bottom: var(--spacing-lg);
}

.contact-info {
  font-size: 0.95rem;
  color: var(--color-text-muted);
  opacity: 0.8;
}

.contact-info p {
  margin-bottom: var(--spacing-xs);
}

.success-message {
  text-align: center;
  margin-bottom: var(--spacing-xl);
}

.cart-summary {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: var(--spacing-lg);
  background-color: var(--color-bg-base);
  border: 1px solid var(--color-secondary-beige);
}

.order-form {
  max-width: 600px;
  margin: 0 auto var(--spacing-xl);
}

.form-group {
  margin-bottom: var(--spacing-md);
}

.form-group label {
  display: block;
  margin-bottom: var(--spacing-xs);
  font-weight: 500;
}

.form-group input {
  width: 100%;
  padding: var(--spacing-sm);
  border: 1px solid var(--color-secondary-beige);
  border-radius: var(--radius-sm);
  font-family: inherit;
}

.form-actions {
  display: flex;
  justify-content: flex-end;
  gap: var(--spacing-sm);
  margin-top: var(--spacing-lg);
}

.btn-text {
  background: none;
  border: none;
  color: var(--color-text-muted);
  cursor: pointer;
  font-weight: 500;
}

.loading {
  text-align: center;
  color: var(--color-text-muted);
  font-size: 1.1rem;
}

.catalog {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(280px, 1fr));
  gap: var(--spacing-lg);
}

.flower-card {
  display: flex;
  flex-direction: column;
  padding: 0;
  overflow: hidden;
  transition: transform 0.2s ease, box-shadow 0.2s ease;
}

.flower-card:hover {
  transform: translateY(-4px);
  box-shadow: 0 12px 24px rgba(0, 0, 0, 0.06);
}

.flower-image {
  height: 280px;
  background-color: var(--color-bg-alt);
  display: flex;
  align-items: center;
  justify-content: center;
  overflow: hidden;
}

.flower-image img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.flower-info {
  padding: var(--spacing-lg);
  display: flex;
  flex-direction: column;
}

.flower-info h3 {
  margin-bottom: var(--spacing-xs);
  font-size: 1.25rem;
}

.price {
  font-size: 1.1rem;
  font-weight: 600;
  color: var(--color-text-muted);
  margin-bottom: var(--spacing-lg);
}
</style>
