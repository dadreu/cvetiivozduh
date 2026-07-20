<script setup lang="ts">
import { ref, onMounted } from 'vue';
import apiClient from '@/api/axios';

const flowers = ref<any[]>([]);
const loading = ref(true);

onMounted(async () => {
  try {
    const response = await apiClient.get('/catalog/flowers');
    flowers.value = response.data;
  } finally {
    loading.value = false;
  }
});
</script>

<template>
  <div class="container">
    <h2>Управление товарами</h2>
    <div class="admin-actions">
      <button class="btn btn-accent">Добавить товар</button>
    </div>

    <div v-if="loading">Загрузка...</div>
    <div class="card" v-else>
      <table class="admin-table">
        <thead>
          <tr>
            <th>ID</th>
            <th>Название</th>
            <th>Цена</th>
            <th>Действия</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="flower in flowers" :key="flower.id">
            <td>{{ flower.id }}</td>
            <td>{{ flower.name }}</td>
            <td>{{ flower.price }} ₽</td>
            <td>
              <button class="btn-text">Редактировать</button>
              <button class="btn-text text-danger">Удалить</button>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>

<style scoped>
.admin-actions {
  margin-bottom: var(--spacing-lg);
  display: flex;
  justify-content: flex-end;
}

.admin-table {
  width: 100%;
  border-collapse: collapse;
}

.admin-table th, .admin-table td {
  padding: var(--spacing-sm);
  text-align: left;
  border-bottom: 1px solid var(--color-secondary-beige);
}

.btn-text {
  background: none;
  border: none;
  cursor: pointer;
  color: var(--color-secondary-dusty-blue);
  font-weight: 500;
  margin-right: var(--spacing-sm);
}

.btn-text:hover {
  text-decoration: underline;
}

.text-danger {
  color: var(--color-accent-terracotta);
}
</style>
