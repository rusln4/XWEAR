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
              <div class="detail-price">{{ fmtPrice(currentPrice) }} ₽</div>
              
              <div v-if="cartQuantity > 0" class="qty-control">
                  <button class="qty-btn" @click="decreaseCart">−</button>
                  <span class="qty-val">{{ cartQuantity }}</span>
                  <button class="qty-btn" @click="increaseCart">+</button>
              </div>
              <button v-else class="btn btn-primary" @click="addToCart">ДОБАВИТЬ В КОРЗИНУ</button>
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
    import { ref, onMounted, computed, watch } from 'vue';
    import { useRoute } from 'vue-router';


    const route = useRoute()
    const product = ref(null)
    const error = ref('')
    const images = ref([])
    const sizes = ref([])
    const selectedSizeId = ref(0)
    const mainImage = ref('')
    const mainIndex = ref(0)
    const lightboxOpen = ref(false)
    const activeIndex = ref(0)

    const cartQuantity = ref(0)
    const cartItem = ref(null)

    function fmtPrice(p){
        return new Intl.NumberFormat('ru-RU').format(p || 0)
    }

    const currentPrice = computed(() => {
        const s = sizes.value.find(x => x.id === selectedSizeId.value)
        if (s) {
            return s.price * (cartQuantity.value > 0 ? cartQuantity.value : 1)
        }
        return sizes.value[0]?.price || 0
    })

    watch(selectedSizeId, async (newVal) => {
        if (newVal > 0) {
            await checkCart()
        } else {
            cartQuantity.value = 0
            cartItem.value = null
        }
    })

    onMounted(async() =>{
        const id = Number(route.params.id)
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
            const fallbackImg = new URL('../assets/images/clothes/no-image-large.jpg', import.meta.url).href
            
            images.value = Array.isArray(data.images) && data.images.length > 0 ? data.images.map(i => ({id: i.id, url:`/api/Images/${i.id}/file`, isMain: i.isMain})) : []
            if (images.value.length === 0) {
                 images.value = [{ id: 0, url: fallbackImg, isMain: 1 }]
            }

            sizes.value = Array.isArray(data.sizes) ? data.sizes.map(s => ({id: s.id, size: s.size1, price: s.price})) : []
            const main = images.value.find(i => Number(i.isMain) === 1)?.url
            mainImage.value = main || images.value[0]?.url || fallbackImg
            mainIndex.value = images.value.findIndex(i => i.url === mainImage.value)
            
            
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
        error.value = ''
    }

    async function checkCart() {
        if (selectedSizeId.value === 0) return
        const raw = localStorage.getItem('xwear_user')
        const u = raw ? JSON.parse(raw) : null
        const userId = Number(u?.id)
        if (!userId) return

        try {
            const res = await fetch(`/api/Carts/user/${userId}`)
            if (res.ok) {
                const items = await res.json()
                const productId = Number(route.params.id)
                const item = items.find(i => i.productId === productId && i.sizeId === selectedSizeId.value)
                if (item) {
                    cartQuantity.value = item.count
                    cartItem.value = item
                } else {
                    cartQuantity.value = 0
                    cartItem.value = null
                }
            }
        } catch (e) {
            console.error(e)
        }
    }

    async function addToCart(){
        error.value = ''
        if (selectedSizeId.value === 0) {
            error.value = "Выберите размер"
            return
        }
        try{
            const raw = localStorage.getItem('xwear_user')
            const u = raw ? JSON.parse(raw) : null
            const userId = Number(u?.id)
            if (!userId || userId <= 0){
                error.value = "Войдите в профиль, чтобы добавить в корзину";
                return
            }
            const productId = Number(route.params.id)
            const res = await fetch('/api/Carts', {method: 'POST', headers: { 'Content-Type': 'application/json', 'Accept': 'application/json' }, body: JSON.stringify({ userId, productId, sizeId: selectedSizeId.value, count: 1})})
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
            await checkCart()
        }
        catch{
            error.value = "Сеть не доступна"
        }
    }

    async function increaseCart() {
        if (!cartItem.value) return
        try {
            const newCount = cartQuantity.value + 1
            const res = await fetch(`/api/Carts/${cartItem.value.id}/count`, {
                method: 'PUT',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(newCount)
            })
            if (res.ok) {
                cartQuantity.value = newCount
                // update local cartItem count too
                cartItem.value.count = newCount
            }
        } catch (e) {
            console.error(e)
            error.value = "Ошибка обновления корзины"
        }
    }

    async function decreaseCart() {
        if (!cartItem.value) return
        try {
            if (cartQuantity.value > 1) {
                const newCount = cartQuantity.value - 1
                const res = await fetch(`/api/Carts/${cartItem.value.id}/count`, {
                    method: 'PUT',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify(newCount)
                })
                if (res.ok) {
                    cartQuantity.value = newCount
                    cartItem.value.count = newCount
                }
            } else {
                const res = await fetch(`/api/Carts/${cartItem.value.id}`, { method: 'DELETE' })
                if (res.ok) {
                    cartQuantity.value = 0
                    cartItem.value = null
                }
            }
        } catch (e) {
            console.error(e)
            error.value = "Ошибка обновления корзины"
        }
    }
</script>
