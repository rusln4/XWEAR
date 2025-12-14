<template>
    <section class="block-add-catalog">
      <figure class="block-image-catalog">
      <img src="../assets/images/add/add_main_top.svg" alt="">
      </figure>
      <div class="block-text-button-catalog">
      <h1 v-html="titleHtml"></h1>
      <p class="text-in-add" v-html="text"></p>
      <button class="btn-catalog-add" @click="goToCatalog">
        <span>{{ buttonText }}</span>
        <img src="../assets/images/icons/arrow_right.svg" alt="">
      </button>
        </div>
    </section>
</template>
  
<script>
export default {
    name: 'TopHeroBanner',
    props: {
      titleLines: {
        type: Array,
        default: () => []
      },
      text: {
        type: String,
        default: ''
      },
      buttonText: {
        type: String,
        default: ''
      }
    },
    computed: {
      titleHtml() {
        return this.titleLines.join('<br>')
      }
    },
    methods: {
      goToCatalog() {
        if (this.$router) this.$router.push({ name: 'Catalog' })
      }
    }
  }
</script>
  