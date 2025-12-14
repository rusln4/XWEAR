<template>
    <section class="block-shoes">
    <header class="block-top-content-block">
    <h1 class="tag-catalog">{{ title }}</h1>
    <a class="link-more-products" href="#" @click.prevent="goToCatalog">
        <span>БОЛЬШЕ ТОВАРОВ</span>
        <img src="../assets/images/icons/arrow_right_black.svg" alt="">
    </a>
    </header>
  
    <ul class="block-shoes-buy">
        <li class="card-product" v-for="(p, i) in products" :key="i" @click="goTo(p.id)">
        <img class="fav-star" src="../assets/images/icons/star_icon.svg" alt="">
        <figure class="img-product">
            <img :src="p.image" alt="">
        </figure>
        <div class="block-name-price">
            <p class="name-product">{{ p.name }}</p>
            <p class="price-product"><span class="price-value">{{ p.price }}</span><span class="currency">₽</span></p>
        </div>
        </li>
    </ul>
    </section>
</template>
  
<script>
export default {
    name: 'ProductSection',
    props: {
        title: {
        type: String,
        required: true
    },
    products: {
        type: Array,
        default: () => []
    }},
    methods: {
        goTo(id) {
        if (!id) return
        if (this.$router) this.$router.push({ name: 'ProductDetail', params: { id } })
    },
        goToCatalog() {
        if (this.$router) this.$router.push({ name: 'Catalog' })
    }}
}
</script>
  