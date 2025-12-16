<template>
    <section class="product-page">
      <div class="product-card">
  
        <div class="brand-divider"></div>
        <div class="product-detail-grid">
          <div class="left">
            <figure class="detail-image">
              <img :src="mainImage" alt="product" @click="openLightbox(mainIndex)" />
            </figure>
            <div class="thumbs" v-if="images.length">
              <img v-for="(img, idx) in images" :key="img.id" :src="img.url" @click="setMain(idx)" />
            </div>
          </div>
          <div class="right">
            <div class="detail-title-row">
              <h1 class="detail-title">{{ product?.name || 'Товар' }}</h1>
              <img class="fav-star" src="../assets/images/icons/star_icon.svg" alt="">
            </div>
            <div class="detail-sizes" v-if="sizes.length">
              <div class="sizes-label">EU РАЗМЕРЫ:</div>
              <ul class="sizes-grid">
                <li v-for="s in sizes" :key="s.id" class="size-item" :class="{ active: s.id === selectedSizeId }" @click="selectSize(s)">
                  <span class="size-val">{{ s.size }}</span>
                  <span class="size-price">{{ fmtPrice(s.price) }} ₽</span>
                </li>
              </ul>
            </div>
            <div class="detail-buy-row">
              <div class="detail-price">{{ fmtPrice(displayPrice) }} ₽</div>
              <button class="btn btn-primary" @click="addToCart">ДОБАВИТЬ В КОРЗИНУ</button>
            </div>
          </div>
        </div>
        <p v-if="error" class="error-text">{{ error }}</p>
        <div v-if="lightboxOpen" class="lightbox" @click.self="closeLightbox">
          <button class="lightbox-close" @click="closeLightbox">✕</button>
          <button class="lightbox-nav prev" @click="prev">‹</button>
          <img class="lightbox-image" :src="images[activeIndex]?.url" alt="" />
          <button class="lightbox-nav next" @click="next">›</button>
          <div class="lightbox-thumbs" v-if="images.length">
            <img v-for="(img, i) in images" :key="img.id" :src="img.url" :class="{ active: i === activeIndex }" @click="activeIndex = i; setMain(i)" />
          </div>
        </div>
      </div>
    </section>
  </template>

<script setup>
    import { ref, onMounted } from 'vue';
    import { useRoute } from 'vue-router';


    const route = useRoute()
    const id = Number(route.params.id)
    const product = ref(null)
    const error = ref('')
    const images = ref([])
    const sizes = ref([])
    const selectedSizeId = ref(0)
    const displayPrice = ref(0)
    const mainImage = ref('')
    const mainIndex = ref(0)
    const lightboxOpen = ref(false)
    const activeIndex = ref(0)

    function fmtPrice(p){
        return new Intl.NumberFormat('ru-RU').format(p || 0)
    }

    onMounted(async() =>{
        if (!Number.isInteger(id) || id <= 0){
            error.value = "Некорректный номер товара";
            return
        }
        try{
            const res = await fetch(`/api/Products/${id}`)
            if(!res.ok){
                error.value = "Товар не найден";
                return
            }
            const ct = res.headers.get('content-type') || ''
            if (!ct.includes('application/json')){
                error.value = "Ошибка ответа сервера";
                return
            }
            const data = await res.json()
            product.value = data
            images.value = Array.isArray(data.images) ? data.images.map(i => ({id: i.id, url:`http://localhost:5037/api/Images/${i.id}/file`, isMain: i.isMain})) : []
            sizes.value = Array.isArray(data.sizes) ? data.sizes.map(s => ({id: s.id, size: s.size1, price: s.price})) : []
            const main = images.value.find(i => Number(i.isMain) === 1)?.url
            mainImage.value = main || images.value[0]?.url || ''
            mainIndex.value = images.value.findIndex(i => i.url === mainImage.value)
            displayPrice.value = sizes.value[0]?.price || 0
        }
        catch{
            error.value = "Сеть недоступна"
        }
    })

    function openLightbox(i){
        if (!images.value.length){
            return
        }
        activeIndex.value = typeof i === "number" && i >= 0? i : 0
        lightboxOpen.value = true
    }

    function closeLightbox(){
        lightboxOpen.value = false
    }

    function next(){
        if (!images.value.length){
            return;
        }
        activeIndex.value = (activeIndex.value + 1) % images.value.length;
        setMain(activeIndex.value)
    }

    function prev(){
        if (!images.value.length){
            return;
        }
        activeIndex.value = (activeIndex.value - 1 + images.value.length) % images.value.length;
        setMain(activeIndex.value)
    }

    function setMain(i){
        if (!images.value.length){
            return;
        }
        const idx = typeof i === 'number' && i >= 0 ? i : 0
        mainImage.value = images.value[idx]?.url || ''
        mainIndex.value = idx
        activeIndex.value = idx
    }

    function selectSize(s){
        if (!s){
            return
        }
        selectedSizeId.value = Number(s.id) || 0
        displayPrice.value = Number(s.price) || 0
    }

    async function addToCart(){
        error.value = ''
        try{
            const raw = localStorage.getItem('xwear_user')
            const u = raw ? JSON.parse(raw) : null
            const userId = Number(u?.id)
            if (!userId || userId <= 0){
                error.value = "Войдите в профиль, чтобы добавить в корзину";
                return
            }
            const res = await fetch('/api/Carts', {method: 'POST', headers: { 'Content-Type': 'application/json', 'Accept': 'application/json' }, body: JSON.stringify({ userId, productId: id, sizeId: selectedSizeId.value, count: 1})})
            if (!res.ok){
                if (res.status === 400){
                    error.value = "Неверные данные"
                }
                else if (res.status === 404){
                    error.value = "Товар или размер не найден"
                }
                else{
                    error.value = "Не удалось добавить товар в корзину"
                }
                return
            }
        }
        catch{
            error.value = "Сеть не доступна"
        }
    }
</script>

<style scoped>
    .product-page { width: 100%; margin: 0; padding: 30px 60px; }
    .product-card { padding: 0; border: none; background: transparent; }
    .product-detail-grid { display: grid; grid-template-columns: 1.6fr 1fr; gap: 40px; align-items: start; }
    .detail-image { height: 360px; }
    .detail-image img { width: 100%; height: 100%; display: block; object-fit: contain; background: #F9F9F9; border-radius: 8px; }
    .thumbs { display: flex; gap: 16px; margin-top: 16px; }
    .thumbs img { width: 80px; height: 80px; object-fit: contain; background: #F9F9F9; border-radius: 6px; }
    .detail-title-row { display: flex; align-items: center; justify-content: space-between; }
    .detail-title { font-family: rf-devi-extended-black; font-size: 40px; line-height: 1.1; text-transform: uppercase; }
    .detail-title-row .fav-star { width: 20px; height: 20px; filter: invert(1); }
    .sizes-label { margin: 24px 0 12px; font-size: 13px; color: #6A6A6A; letter-spacing: .5px; }
    .sizes-grid { display: grid; grid-template-columns: repeat(6, minmax(0, 1fr)); gap: 10px; list-style: none; padding: 0; }
    .size-item { border: 1px solid #E9EAEE; border-radius: 10px; padding: 12px 14px; display: flex; flex-direction: column; align-items: center; gap: 4px; background: #fff; }
    .size-item:hover { border-color: #D4D6DC; }
    .size-item.active { border-color: black; box-shadow: 0 0 0 2px rgba(0,0,0,.06); }
    .size-val { font-size: 14px; color: #171819; }
    .size-price { font-size: 12px; color: #707076; }
    .detail-buy-row { display: flex; align-items: center; gap: 20px; margin-top: 24px; }
    .detail-price { font-size: 22px; }
    @media (max-width: 900px) { .product-page { padding: 20px; } }
    @media (max-width: 768px) { .product-detail-grid { grid-template-columns: 1fr; } }
    
    .lightbox { position: fixed; inset: 0; background: rgba(0,0,0,.6); display: flex; align-items: center; justify-content: center; z-index: 1000; }
    .lightbox-image { max-width: 80vw; max-height: 70vh; border-radius: 8px; background: #F9F9F9; }
    .lightbox-close { position: absolute; top: 20px; right: 24px; background: transparent; border: none; color: white; font-size: 24px; cursor: pointer; }
    .lightbox-nav { position: absolute; top: 50%; transform: translateY(-50%); background: rgba(0,0,0,.3); color: white; border: none; width: 40px; height: 40px; border-radius: 50%; cursor: pointer; }
    .lightbox-nav.prev { left: 24px; }
    .lightbox-nav.next { right: 24px; }
    .lightbox-thumbs { position: absolute; bottom: 24px; left: 50%; transform: translateX(-50%); display: flex; gap: 8px; }
    .lightbox-thumbs img { width: 60px; height: 60px; object-fit: contain; background: #F9F9F9; border-radius: 6px; opacity: .8; cursor: pointer; }
    .lightbox-thumbs img.active { outline: 2px solid white; opacity: 1; }
    
</style>