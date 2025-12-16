<template>
    <section class="catalog-page">
      <div class="catalog-layout">
        <aside class="filters">
          <div class="filter-group">
            <div class="filter-title">КАТЕГОРИИ</div>
            <ul class="filter-list">
              <li v-for="cat in categories" :key="cat" :class="{ active: cat === selectedCategory }" @click="selectCategory(cat)">{{ cat }}</li>
              <li v-if="categories.length" :class="{ active: !selectedCategory }" @click="selectCategory('')">Все</li>
            </ul>
          </div>
          <div class="filter-group">
            <div class="filter-title">ПОИСК ПО НАЗВАНИЮ</div>
            <input class="price-input" type="text" v-model.trim="search" placeholder="Например: Nike" />
          </div>
          <div class="filter-group">
            <div class="filter-title">ФИЛЬТР ПО ЦЕНЕ</div>
            <div class="price-inputs">
              <input class="price-input" type="number" :min="priceMinAll" :max="priceMaxAll" v-model.number="priceMin" />
              <span class="rub">₽</span>
              <input class="price-input" type="number" :min="priceMinAll" :max="priceMaxAll" v-model.number="priceMax" />
              <span class="rub">₽</span>
            </div>
            <input class="range" type="range" :min="priceMinAll" :max="priceMaxAll" v-model.number="priceMin" />
            <input class="range" type="range" :min="priceMinAll" :max="priceMaxAll" v-model.number="priceMax" />
          </div>
          <div class="filter-group">
            <div class="filter-title">РАЗМЕРЫ (EU)</div>
            <ul class="sizes-grid">
              <li v-for="sz in sizesAll" :key="sz" :class="{ active: sz === selectedSize }" @click="toggleSize(sz)">{{ sz }}</li>
              <li v-if="sizesAll.length" :class="{ active: !selectedSize }" @click="toggleSize('')">Все</li>
            </ul>
          </div>
        </aside>
  
        <main class="catalog-content">
          <header class="catalog-header">
            <h1 class="catalog-title">КАТАЛОГ</h1>
            <div class="catalog-count">{{ filtered.length }} товаров</div>
          </header>
          <ul class="block-shoes-buy">
            <li class="card-product" v-for="p in filtered" :key="p.id" @click="goTo(p.id)">
              <img class="fav-star" src="../assets/images/icons/star_icon.svg" alt="">
              <figure class="img-product">
                <img :src="p.image" alt="">
              </figure>
              <div class="block-name-price">
                <p class="name-product">{{ p.name }}</p>
                <p class="price-product"><span class="price-value">{{ p.priceText }}</span><span class="currency">₽</span></p>
              </div>
            </li>
          </ul>
        </main>
      </div>
    </section>
  </template>

<script setup>
    import {ref, computed, onMounted} from 'vue'
    import { useRouter } from 'vue-router';

    const items = ref([])
    const categories = ref([])
    const sizesAll = ref([])
    const selectedCategory = ref('')
    const selectedSize = ref('')
    const search = ref('')
    const priceMin = ref(0)
    const priceMax = ref(0)
    const priceMinAll = ref(0)
    const priceMaxAll = ref(0)

    function fmt(n){
        return new Intl.NumberFormat('ru-RU').format(n || 0)
    }

    onMounted(async() => {
        try{
            let res = await fetch('/api/Products/full')
            if (!res.ok){
                items.value = [];
                return
            }
            const ct = res.headers.get('content-type') || ''
            if (!ct.includes('application/json')){
                items.value = [];
                return
            }
            const data = await res.json()
            const fallbackImg = new URL('../assets/images/clothes/no-image-large.jpg', import.meta.url).href
            items.value = Array.isArray(data) ? data.map(p => {
                const image = p.imageId ? `/api/Images/${p.imageId}/file` : fallbackImg
                const sizes = Array.isArray(p.sizes) ? p.sizes : []
                const minPrice = typeof p.price === 'number' ? p.price : 0
                return { id: p.id, name: p.name, image: image, priceNum: minPrice, priceText: fmt(minPrice), category: p.category || "Другое", sizes}
            }) : []
            const cats = Array.from(new Set(items.value.map(i => i.category))).filter(Boolean)
            categories.value = cats
            const allSizes = Array.from(new Set(items.value.flatMap(i => i.sizes))).filter(Boolean).sort((a,b) => String(a).localeCompare(String(b), 'ru'))
            sizesAll.value = allSizes
            const pricesAll = items.value.map(i => i.priceNum).filter(n => typeof n === 'number')
            const min = pricesAll.length ? Math.min(...pricesAll) : 0
            const max = pricesAll.length ? Math.max(...pricesAll) : 0
            priceMinAll.value = min
            priceMaxAll.value = max
            priceMin.value = min
            priceMax.value = max
        }
        catch{
            items.value = []
        }
    })

    const filtered = computed(() => {
        return items.value.filter(i => {
            if (selectedCategory.value && i.category !== selectedCategory.value){
                return false
            }
            if (selectedSize.value && !i.sizes.includes(selectedSize.value)){
                return false
            }
            if (search.value && !String(i.name).toLowerCase().includes(String(search.value).toLowerCase())) return false
            if (typeof i.priceNum === 'number'){
                if (i.priceNum < priceMin.value){
                    return false
                }
                if (i.priceNum > priceMax.value){
                    return false
                }
            }
            return true
        })
    })

    function selectCategory(c){
        selectedCategory.value = c || ''
    }
    function toggleSize(s){
        selectedSize.value = s || ''
    }
    const router = useRouter()
    function goTo(id){
        if (!id){
            return;
        }
        if (router){
            router.push({name: 'ProductDetail', params: {id}})
        }
    }

</script>

<script>
    export default {name: 'CatalogPage'}
</script>
