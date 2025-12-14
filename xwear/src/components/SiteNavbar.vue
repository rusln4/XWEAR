<template>
    <nav>
        <div class="nameShop">
            <router-link to="/mainpage" class="brand-link">
                <h1 class="shopName">XWEAR</h1>
            </router-link>
        </div>

        <div class="menu">
            <ul>
                <li><a href="#">Одежда</a></li>
                <li><a href="#">Обувь</a></li>
                <li><a href="#">Аксессуары</a></li>
                <li><a href="#">Бренды</a></li>
                <li><a href="#">Расчет стоимости</a></li>
                <li><a href="#">Информация</a></li>
            </ul>
        </div>
        
        <div class="link-icon">
            <a href="#">
                <img src="../assets/images/icons/search_Icon.svg" alt="">
            </a>

            <a href="#">
                <img src="../assets/images/icons/star_icon.svg" alt="">
            </a>

            <router-link to="/account" class="user-profile-link">
                <img src="../assets/images/icons/user_icon.svg" alt="">
            </router-link>

            <router-link to="/cart" class="bascket-info-link">
                <img src="../assets/images/icons/bascket_icon.svg" alt="">
            </router-link>
        </div>
    </nav>
</template>

<script>
    export default {
        name: "SiteNavbar"
    }
</script>
