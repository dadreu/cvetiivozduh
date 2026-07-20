<script setup lang="ts">
import { ref, onMounted } from 'vue';
import apiClient from '@/api/axios';

interface Flower {
  id: number;
  name: string;
  price: number;
  imageUrl: string;
}

const flowers = ref<Flower[]>([]);
const loading = ref(true);

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
</script>

<template>
  <div class="container">
    <div class="hero">
      <h1>Искусство в каждом лепестке</h1>
      <p>Откройте для себя свежесть и красоту скандинавской флористики.</p>
    </div>

    <div v-if="loading" class="loading">Загрузка...</div>
    
    <div v-else class="catalog">
      <div class="card flower-card" v-for="flower in flowers" :key="flower.id">
        <div class="flower-image">
          <img :src="flower.imageUrl || '/placeholder.webp'" :alt="flower.name" />
        </div>
        <div class="flower-info">
          <h3>{{ flower.name }}</h3>
          <p class="price">{{ flower.price }} ₽</p>
          <button class="btn btn-primary">В корзину</button>
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

.hero p {
  font-size: 1.25rem;
  color: var(--color-text-muted);
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
