<template>
  <div>
    <input v-model="filter" placeholder="Filter by name" />
    <table>
      <thead>
        <tr>
          <th>ID</th>
          <th>Name</th>
          <th>Description</th>
        </tr>
      </thead>
      <tbody>
        <tr v-if="loading">
          <td colspan="3">Loading...</td>
        </tr>
        <tr v-else-if="error">
          <td colspan="3">{{ error }}</td>
        </tr>
        <tr v-else v-for="item in filteredData" :key="item.id">
          <td>{{ item.id }}</td>
          <td>{{ item.title.romaji }}</td>
          <td>{{ item.description }}</td>
        </tr>
      </tbody>
    </table>
  </div>
</template>

<script lang="ts">
import { defineComponent, ref, computed, onMounted } from 'vue';
import axios from 'axios';

interface DataItem {
  id: number;
  title: {
    romaji: string;
  };
  description: string;
}

export default defineComponent({
  name: 'DataGrid',
  setup() {
    const data = ref<DataItem[]>([]);
    const filter = ref('');
    const loading = ref(true);
    const error = ref<string | null>(null);

    const fetchData = async () => {
      try {
        console.log('Fetching data from API...');
        const response = await axios.get<DataItem[]>('http://localhost:5014/api/data', {
          withCredentials: true
        });
        console.log('Fetched data:', response.data);
        if (response.data.length === 0) {
          throw new Error('No data found for resource with given identifier');
        }
        data.value = response.data;
      } catch (err: any) {
        console.error('Error fetching data:', err);
        error.value = err.message || 'Failed to fetch data';
      } finally {
        loading.value = false;
        console.log('Fetch data process completed.');
      }
    };

    const filteredData = computed(() => {
      console.log('Filtering data with filter:', filter.value);
      return data.value.filter((item) =>
        item.title.romaji.toLowerCase().includes(filter.value.toLowerCase())
      );
    });

    onMounted(() => {
      console.log('Component mounted, fetching data...');
      fetchData();
    });

    return {
      filter,
      filteredData,
      loading,
      error
    };
  }
});
</script>

<style scoped>
table {
  width: 100%;
  border-collapse: collapse;
}

th, td {
  border: 1px solid #ddd;
  padding: 8px;
}

th {
  background-color: #f4f4f4;
}
</style>