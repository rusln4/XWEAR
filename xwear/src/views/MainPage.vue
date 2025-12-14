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
</script>