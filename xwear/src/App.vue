<template>
<div  id="app">
  <SiteNavbar/>
  <router-view />
  <SiteFooter/>
</div>

</template>

<script setup>
    import SiteNavbar from "./components/SiteNavbar.vue"
    import SiteFooter from "./components/SiteFooter.vue";
    import "./style/style.css"
</script>

