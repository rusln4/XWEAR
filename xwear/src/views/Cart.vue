<template>
    <section class="cart-page">
      <div class="account-card" v-if="user">
        <div class="profile-header">
          <div class="profile-name-email">
            <div class="profile-name">Корзина</div>
            <div class="profile-email">{{ user.email }}</div>
          </div>
        </div>
        <div v-if="success" class="success-message" style="text-align: center; padding: 40px;">
          <h2 style="font-family: 'Unbounded', sans-serif; font-size: 24px; margin-bottom: 20px;">Спасибо за заказ!</h2>
          <p style="font-size: 18px; margin-bottom: 10px;">Заказ на сумму <strong>{{ fmt(lastOrderTotal) }} ₽</strong> успешно оформлен.</p>
          <p style="color: #888;">Количество товаров: {{ lastOrderCount }}</p>
          <button class="btn btn-primary" style="margin-top: 30px; max-width: 200px;" @click="success = false">ВЕРНУТЬСЯ В МАГАЗИН</button>
        </div>
        <div v-else-if="items.length">
          <div class="cart-grid">
            <div v-for="it in items" :key="it.id" class="cart-card">
              <figure class="cart-image"><img :src="it.image" alt="product" /></figure>
              <div class="cart-info">
                <div class="cart-title">{{ it.name }}</div>
                <div class="cart-meta">Размер: {{ it.size }} — {{ fmt(it.price) }} ₽</div>
                <div class="cart-actions">
                  <button class="qty-btn" @click="dec(it)">−</button>
                  <span class="qty">{{ it.count }}</span>
                  <button class="qty-btn" @click="inc(it)">+</button>
                  <button class="del-btn" @click="removeItem(it)">✕</button>
                </div>
                <div class="cart-line-total">{{ fmt(it.price * it.count) }} ₽</div>
              </div>
            </div>
          </div>
          <div class="brand-divider"></div>
          <div class="cart-summary">
            <div>Итого: <strong>{{ fmt(total) }} ₽</strong></div>
          </div>
          <div class="checkout-form">
            <button class="btn btn-primary" @click="checkout" :disabled="placing">ОФОРМИТЬ ЗАКАЗ</button>
          </div>
          <p v-if="error" class="error-text">{{ error }}</p>
        </div>
        <div v-else class="empty">Ваша корзина пуста</div>
      </div>
      <div v-else class="account-card">Войдите в профиль, чтобы увидеть корзину</div>
    </section>
  </template>
<script setup>
    import {ref, computed, onMounted} from 'vue'


    const user = ref(null)
    const items = ref([])
    const error = ref('')
    const success = ref(false)
    const placing = ref(false)
    const lastOrderTotal = ref(0)
    const lastOrderCount = ref(0)

    function fmt(n){
        return new Intl.NumberFormat('ru-RU').format(n || 0)
    }

    async function load() {
        error.value = ''
        try{
            const raw = localStorage.getItem('xwear_user')
            user.value = raw ? JSON.parse(raw) : null
            const uid = Number(user.value?.id)
            if (!uid || uid <= 0) return
            const res = await fetch(`/api/Carts/user/${uid}`)
            if (!res.ok){
                error.value = "Не удалось загрузить корзину"; return
            }
            const data = await res.json()
            const fallbackImg = new URL('../assets/images/clothes/no-image-large.jpg', import.meta.url).href
            items.value = Array.isArray(data) ? data.map(x => ({
                id: x.id, 
                productId: x.productId,
                name: x.name,
                image: x.imageId ? `/api/Images/${x.imageId}/file` : fallbackImg,
                size: x.size,
                price: x.price,
                count: x.count
            })) : []
        }
        catch{
            error.value = 'Сеть недоступна'
        }
    }

    const total = computed(() => items.value.reduce((sum, it) => sum + (Number(it.price) * Number(it.count)), 0))

    async function setCount(it, next) {
        if (!it || next <= 0){
            return
        }
        try{
            const res = await fetch(`/api/Carts/${it.id}/count`, { method: 'PUT', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify(next) })
            if (!res.ok){
                error.value = "Не удалось обновить количество";
                return
            }
            it.count = next
        }
        catch{
            error.value = 'Сеть недоступна'
        }
    }

    function inc(it){
        setCount(it, Number(it.count) + 1) 
    }
    function dec(it){
        if (Number(it.count) > 1) {
            setCount(it, Number(it.count) - 1)
        }
    }

    async function removeItem(it) {
        try{
            const res = await fetch(`/api/Carts/${it.id}`, { method: 'DELETE' })
            if (!res.ok){
                error.value = "Не удалось удалить";
                return
            }
            items.value = items.value.filter(x => x.id !== it.id)
        }
        catch{
            error.value = "Сеть недоступна"
        }
    }
    async function checkout() {
        success.value = false
        error.value = ''
        placing.value = true
        try{
            const uid = Number(user.value?.id)
            if (!uid){
                error.value = "Войдите в профиль";
                return
            }

            lastOrderTotal.value = total.value
            lastOrderCount.value = items.value.reduce((c, i) => c + i.count, 0)

            const res = await fetch('/api/Carts/checkout', {method: 'POST', headers: { 'Content-Type': 'application/json' },body: JSON.stringify({ userId: uid })})
            if (!res.ok){
                error.value = "Не удалось оформить заказ"
                return;
            }
            items.value = []
            success.value = true
            window.scrollTo(0,0)
        }   
        catch{
            error.value = "Сеть не доступна"
        }
        finally{
            placing.value = false
        }
    }

    onMounted(load)


</script>

<script>
    export default {name: 'CartPage'}
</script>

<style scoped>
.cart-grid { display: grid; grid-template-columns: repeat(3, minmax(0, 1fr)); gap: 24px; }
.cart-card { border: 1px solid #E9EAEE; border-radius: 10px; padding: 14px; background: #fff; }
.cart-image { width: 100%; aspect-ratio: 1 / 1; border-radius: 8px; background: #F9F9F9; overflow: hidden; }
.cart-image img { width: 100%; height: 100%; object-fit: contain; display: block; }
.cart-info { margin-top: 10px; }
.cart-title { font-size: 14px; }
.cart-meta { font-size: 14px; color: #6A6A6A; margin-top: 4px; }
.cart-actions { display: flex; align-items: center; gap: 10px; margin-top: 10px; }
.qty-btn { background: #F0F0F0; border: none; padding: 6px 10px; border-radius: 6px; cursor: pointer; }
.qty { min-width: 24px; text-align: center; }
.del-btn { margin-left: auto; background: transparent; border: none; cursor: pointer; font-size: 18px; }
.cart-line-total { margin-top: 8px; font-weight: 600; }
@media (max-width: 900px) { .cart-grid { grid-template-columns: repeat(2, minmax(0, 1fr)); } }
@media (max-width: 600px) { .cart-grid { grid-template-columns: 1fr; } }
</style>
