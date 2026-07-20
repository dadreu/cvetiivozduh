<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useAuthStore } from '@/stores/auth';
import { useRouter } from 'vue-router';
import apiClient from '@/api/axios';

const authStore = useAuthStore();
const router = useRouter();

const currentTab = ref('flowers');
const flowers = ref<any[]>([]);
const categories = ref<any[]>([]);
const vacancies = ref<any[]>([]);
const loading = ref(true);

// Modals
const showFlowerModal = ref(false);
const showVacancyModal = ref(false);

const flowerForm = ref({ id: 0, name: '', description: '', price: 0, categoryId: 1, imageUrl: '', isWide: false });
const vacancyForm = ref({ id: 0, title: '', description: '', isActive: true });

onMounted(async () => {
  await fetchData();
});

const fetchData = async () => {
  loading.value = true;
  try {
    const [fRes, cRes, vRes] = await Promise.all([
      apiClient.get('/catalog/flowers'),
      apiClient.get('/catalog/categories'),
      apiClient.get('/admin/vacancies')
    ]);
    flowers.value = fRes.data;
    categories.value = cRes.data;
    vacancies.value = vRes.data;
  } catch (error) {
    console.error("Failed to load data", error);
  } finally {
    loading.value = false;
  }
};

const logout = () => {
  authStore.clearAuth();
  router.push('/login');
};

// --- Flowers CRUD ---
const openFlowerModal = (flower: any = null) => {
  if (flower) {
    flowerForm.value = { ...flower };
  } else {
    flowerForm.value = { id: 0, name: '', description: '', price: 0, categoryId: categories.value[0]?.id || 1, imageUrl: '', isWide: false };
  }
  showFlowerModal.value = true;
};

const saveFlower = async () => {
  try {
    if (flowerForm.value.id) {
      await apiClient.put(`/admin/flowers/${flowerForm.value.id}`, flowerForm.value);
    } else {
      await apiClient.post(`/admin/flowers`, flowerForm.value);
    }
    showFlowerModal.value = false;
    await fetchData();
  } catch (e) {
    alert('Ошибка при сохранении товара');
  }
};

const deleteFlower = async (id: number) => {
  if (!confirm('Удалить товар?')) return;
  try {
    await apiClient.delete(`/admin/flowers/${id}`);
    await fetchData();
  } catch (e) {
    alert('Ошибка удаления');
  }
};

const uploadImage = async (event: any) => {
  const file = event.target.files[0];
  if (!file) return;
  const formData = new FormData();
  formData.append('file', file);
  try {
    const res = await apiClient.post('/admin/upload', formData);
    flowerForm.value.imageUrl = res.data.imageUrl;
  } catch (e) {
    alert('Ошибка загрузки фото');
  }
};

// --- Vacancies CRUD ---
const openVacancyModal = (vacancy: any = null) => {
  if (vacancy) {
    vacancyForm.value = { ...vacancy };
  } else {
    vacancyForm.value = { id: 0, title: '', description: '', isActive: true };
  }
  showVacancyModal.value = true;
};

const saveVacancy = async () => {
  try {
    if (vacancyForm.value.id) {
      await apiClient.put(`/admin/vacancies/${vacancyForm.value.id}`, vacancyForm.value);
    } else {
      await apiClient.post(`/admin/vacancies`, vacancyForm.value);
    }
    showVacancyModal.value = false;
    await fetchData();
  } catch (e) {
    alert('Ошибка при сохранении вакансии');
  }
};

const deleteVacancy = async (id: number) => {
  if (!confirm('Удалить вакансию?')) return;
  try {
    await apiClient.delete(`/admin/vacancies/${id}`);
    await fetchData();
  } catch (e) {
    alert('Ошибка удаления');
  }
};
</script>

<template>
  <div class="admin-layout">
    <aside class="admin-sidebar">
      <div class="admin-brand">Цветы и Воздух<br><span>Панель управления</span></div>
      <nav class="admin-nav">
        <a href="javascript:void(0)" :class="{ active: currentTab === 'flowers' }" @click="currentTab = 'flowers'">Товары</a>
        <a href="javascript:void(0)" :class="{ active: currentTab === 'vacancies' }" @click="currentTab = 'vacancies'">Вакансии</a>
        <a href="/" target="_blank">На сайт</a>
      </nav>
      <button class="btn-logout" @click="logout">Выйти</button>
    </aside>
    
    <main class="admin-main">
      <div v-if="loading" class="loading">Загрузка данных...</div>
      
      <div v-else class="admin-content">
        <!-- Flowers Tab -->
        <div v-if="currentTab === 'flowers'">
          <div class="content-header">
            <h2>Управление товарами</h2>
            <button class="btn-premium btn-small" @click="openFlowerModal()">+ Добавить товар</button>
          </div>
          <div class="table-card">
            <table class="admin-table">
              <thead>
                <tr>
                  <th>Фото</th>
                  <th>Название</th>
                  <th>Цена</th>
                  <th>Действия</th>
                </tr>
              </thead>
              <tbody>
                <tr v-for="item in flowers" :key="item.id" @click="openFlowerModal(item)" class="clickable-row">
                  <td>
                    <img v-if="item.imageUrl" :src="item.imageUrl" class="td-img" />
                    <span v-else>Нет фото</span>
                  </td>
                  <td>{{ item.name }}</td>
                  <td>{{ item.price }} ₽</td>
                  <td>
                    <button class="btn-text" @click.stop="openFlowerModal(item)">Ред.</button>
                    <button class="btn-text text-danger" @click.stop="deleteFlower(item.id)">Удалить</button>
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>

        <!-- Vacancies Tab -->
        <div v-if="currentTab === 'vacancies'">
          <div class="content-header">
            <h2>Управление вакансиями</h2>
            <button class="btn-premium btn-small" @click="openVacancyModal()">+ Добавить вакансию</button>
          </div>
          <div class="table-card">
            <table class="admin-table">
              <thead>
                <tr>
                  <th>Название</th>
                  <th>Статус</th>
                  <th>Действия</th>
                </tr>
              </thead>
              <tbody>
                <tr v-for="item in vacancies" :key="item.id" @click="openVacancyModal(item)" class="clickable-row">
                  <td>{{ item.title }}</td>
                  <td>
                    <span :class="item.isActive ? 'badge-active' : 'badge-inactive'">
                      {{ item.isActive ? 'Активна' : 'Скрыта' }}
                    </span>
                  </td>
                  <td>
                    <button class="btn-text" @click.stop="openVacancyModal(item)">Ред.</button>
                    <button class="btn-text text-danger" @click.stop="deleteVacancy(item.id)">Удалить</button>
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
      </div>
    </main>

    <!-- Flower Modal -->
    <div v-if="showFlowerModal" class="modal-overlay" @click.self="showFlowerModal = false">
      <div class="modal-content glass">
        <h3>{{ flowerForm.id ? 'Редактировать товар' : 'Добавить товар' }}</h3>
        <form @submit.prevent="saveFlower" class="admin-form">
          <div class="form-group">
            <label>Название</label>
            <input type="text" v-model="flowerForm.name" required />
          </div>
          <div class="form-group">
            <label>Категория</label>
            <select v-model="flowerForm.categoryId">
              <option v-for="c in categories" :key="c.id" :value="c.id">{{ c.name }}</option>
            </select>
          </div>
          <div class="form-group">
            <label>Описание</label>
            <textarea v-model="flowerForm.description" rows="3"></textarea>
          </div>
          <div class="form-group">
            <label>Цена (₽)</label>
            <input type="number" v-model="flowerForm.price" required />
          </div>
          <div class="form-group">
            <label>Фотография (загрузка)</label>
            <input type="file" @change="uploadImage" accept="image/*" />
            <div v-if="flowerForm.imageUrl" class="img-preview">
              <img :src="flowerForm.imageUrl" />
            </div>
          </div>
          <div class="form-group checkbox-group">
            <input type="checkbox" id="isWideFlower" v-model="flowerForm.isWide" />
            <label for="isWideFlower">Широкая карточка (на 2 колонки)</label>
          </div>
          <div class="modal-actions">
            <button type="button" class="btn-text" @click="showFlowerModal = false">Отмена</button>
            <button type="submit" class="btn-premium btn-small">Сохранить</button>
          </div>
        </form>
      </div>
    </div>

    <!-- Vacancy Modal -->
    <div v-if="showVacancyModal" class="modal-overlay" @click.self="showVacancyModal = false">
      <div class="modal-content glass">
        <h3>{{ vacancyForm.id ? 'Редактировать вакансию' : 'Добавить вакансию' }}</h3>
        <form @submit.prevent="saveVacancy" class="admin-form">
          <div class="form-group">
            <label>Должность</label>
            <input type="text" v-model="vacancyForm.title" required />
          </div>
          <div class="form-group">
            <label>Описание</label>
            <textarea v-model="vacancyForm.description" rows="4" required></textarea>
          </div>
          <div class="form-group checkbox-group">
            <input type="checkbox" id="isActive" v-model="vacancyForm.isActive" />
            <label for="isActive">Показывать на сайте (Активна)</label>
          </div>
          <div class="modal-actions">
            <button type="button" class="btn-text" @click="showVacancyModal = false">Отмена</button>
            <button type="submit" class="btn-premium btn-small">Сохранить</button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<style scoped>
.admin-layout {
  display: flex;
  min-height: 100vh;
  background: #f4f5f7;
}

.admin-sidebar {
  width: 250px;
  background: #fff;
  border-right: 1px solid rgba(0,0,0,0.05);
  display: flex;
  flex-direction: column;
  padding: 30px 20px;
}

.admin-brand {
  font-family: var(--font-family-heading);
  font-size: 1.6rem;
  font-weight: 700;
  color: var(--color-accent-pink);
  margin-bottom: 40px;
  line-height: 1.2;
}

.admin-brand span {
  font-size: 0.8rem;
  font-family: var(--font-family-base);
  color: var(--color-text-muted);
  font-weight: 400;
}

.admin-nav {
  display: flex;
  flex-direction: column;
  gap: 10px;
  flex: 1;
}

.admin-nav a {
  text-decoration: none;
  color: var(--color-text-main);
  padding: 12px 15px;
  border-radius: 8px;
  transition: all 0.2s ease;
  font-weight: 500;
}

.admin-nav a:hover {
  background: rgba(0,0,0,0.03);
}

.admin-nav a.active {
  background: var(--color-accent-pink);
  color: #fff;
}

.btn-logout {
  background: none;
  border: 1px solid var(--color-accent-terracotta);
  color: var(--color-accent-terracotta);
  padding: 10px;
  border-radius: 8px;
  cursor: pointer;
  font-weight: 500;
  transition: all 0.2s;
}

.btn-logout:hover {
  background: var(--color-accent-terracotta);
  color: #fff;
}

.admin-main {
  flex: 1;
  padding: 40px;
  overflow-y: auto;
}

.content-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 30px;
}

.content-header h2 {
  font-family: var(--font-family-heading);
  font-size: 2rem;
  color: var(--color-text-main);
}

.btn-small {
  padding: 10px 20px;
  font-size: 0.9rem;
}

.table-card {
  background: #fff;
  border-radius: 12px;
  box-shadow: 0 4px 20px rgba(0,0,0,0.02);
  padding: 20px;
}

.admin-table {
  width: 100%;
  border-collapse: collapse;
}

.admin-table th, .admin-table td {
  padding: 15px;
  text-align: left;
  border-bottom: 1px solid #eee;
  color: var(--color-text-main);
}

.admin-table th {
  font-weight: 600;
  color: var(--color-text-muted);
}

.td-img {
  width: 50px;
  height: 50px;
  object-fit: cover;
  border-radius: 6px;
}

.badge-active {
  background: #d4edda;
  color: #155724;
  padding: 4px 8px;
  border-radius: 4px;
  font-size: 0.8rem;
}

.badge-inactive {
  background: #f8d7da;
  color: #721c24;
  padding: 4px 8px;
  border-radius: 4px;
  font-size: 0.8rem;
}

.btn-text {
  background: none;
  border: none;
  color: var(--color-secondary-dusty-blue);
  cursor: pointer;
  font-weight: 500;
  margin-right: 15px;
}

.btn-text.text-danger {
  color: var(--color-accent-terracotta);
}

.modal-overlay {
  position: fixed;
  top: 0; left: 0; right: 0; bottom: 0;
  background: rgba(0,0,0,0.4);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1100;
}

.modal-content {
  background: #fff;
  padding: 30px;
  border-radius: 16px;
  width: 500px;
  max-width: 90%;
  box-shadow: 0 10px 40px rgba(0,0,0,0.1);
}

.modal-content h3 {
  margin-bottom: 20px;
  font-family: var(--font-family-heading);
}

.admin-form .form-group {
  margin-bottom: 15px;
}

.admin-form label {
  display: block;
  margin-bottom: 5px;
  font-weight: 500;
  color: var(--color-text-main);
  font-size: 0.9rem;
}

.admin-form input[type="text"],
.admin-form input[type="number"],
.admin-form textarea,
.admin-form select {
  width: 100%;
  padding: 12px 15px;
  border: 1px solid rgba(0,0,0,0.1);
  border-radius: 10px;
  background: #fafafa;
  font-family: var(--font-family-base);
  font-size: 0.95rem;
  transition: all 0.3s ease;
  outline: none;
}

.admin-form input:focus,
.admin-form textarea:focus,
.admin-form select:focus {
  border-color: var(--color-accent-pink);
  background: #fff;
  box-shadow: 0 0 0 4px rgba(220,163,183,0.15);
}

.admin-form input[type="file"] {
  font-family: var(--font-family-base);
  font-size: 0.9rem;
  color: var(--color-text-muted);
  padding: 0;
  border: none;
  background: transparent;
}

.admin-form input[type="file"]::file-selector-button {
  padding: 10px 20px;
  border-radius: 8px;
  border: 1px solid rgba(0,0,0,0.1);
  background: #fff;
  color: var(--color-text-main);
  font-family: var(--font-family-base);
  font-weight: 500;
  cursor: pointer;
  transition: all 0.3s ease;
  margin-right: 15px;
}

.admin-form input[type="file"]::file-selector-button:hover {
  background: #fafafa;
  border-color: var(--color-accent-pink);
}

.clickable-row {
  cursor: pointer;
  transition: background 0.2s ease;
}

.clickable-row:hover {
  background: #fafafa;
}

.checkbox-group {
  display: flex;
  align-items: center;
  gap: 10px;
}

.checkbox-group label {
  margin: 0;
}

.img-preview {
  margin-top: 10px;
}

.img-preview img {
  width: 100px;
  border-radius: 8px;
}

.modal-actions {
  display: flex;
  justify-content: flex-end;
  margin-top: 30px;
  gap: 15px;
}
</style>
