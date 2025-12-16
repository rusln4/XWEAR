<template>
    <TopHeroBanner
      :title-lines="['ШИРОКИЙ','АССОРТИМЕНТ','ОДЕЖДЫ']"
      text="Одежда от известные брендов у нас в каталоге.<br>Только качественные вещи."
      button-text="ПЕРЕЙТИ В КАТАЛОГ"
    />
  
    <ProductSection title="КАТАЛОГ" :products="products.slice(0, 4)" />
</template>

<script setup>
    import { ref, onMounted } from 'vue'
    import TopHeroBanner from '../components/TopHeroBanner.vue'
    import ProductSection from '../components/ProductSection.vue'

    const products = ref([])
    const fallbackImg = new URL('../assets/images/clothes/no-image-large.jpg', import.meta.url).href

    onMounted(async() => {
      try{
        const res = await fetch('api/Products/catalog')
        if (!res.ok){
          products.value = []
          return
        }
        const ct = res.headers.get('content-type') || ''
        if (!ct.includes('application/json')){
          products.value = []
          return
        }
        const data = await res.json()
        products.value = Array.isArray(data) ? data.map(item => ({id: item.id, name: item.name, price: new Intl.NumberFormat('ru-RU').format(item.price), image: item.imageId ? `http://localhost:5037/api/Images/${item.imageId}/file` : fallbackImg})) : []

      }
      catch{
        products.value = []
      }
    })


</script>