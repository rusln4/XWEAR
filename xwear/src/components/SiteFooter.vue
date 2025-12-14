<template>
    <footer class="footer">
        <div class="footer-content">
            <div class="catalog-footer">
                <h2 class="title-text">КАТАЛОГ</h2>
                <div class="footer-list">
                    <ul class="footer-list-ul">
                        <li><a href="#">Одежда</a></li>
                        <li><a href="#">Обувь</a></li>
                        <li><a href="#">Аксессуары</a></li>
                        <li><a href="#">Расчет стоимости</a></li>
                    </ul>
                </div>

                <div class="catalog-footer-title">
                    <h1 class="catalog-footer-title-text">XWEAR</h1>
                </div>
            </div>

            <div class="info-footer">
                <h2 class="title-text">ИНФОРМАЦИЯ</h2>
                <div class="footer-list">
                    <ul class="footer-list-ul">
                        <li><a href="#">Блог</a></li>
                        <li><a href="#">Контакты</a></li>
                        <li><a href="#">Доставка</a></li>
                        <li><a href="#">Оплата</a></li>
                        <li><a href="#">FAQ</a></li>
                    </ul>
                </div>
            </div>

            <div class="contact-footer">
                <h2 class="title-text">КОНТАКТЫ</h2>
                <div class="footer-list">
                    <ul class="footer-list-ul">
                        <li><a href="#">ruslanhu05@gmail.com</a></li>
                        <li><a href="#">+7 987 048 62 19</a></li>
                    </ul>
                </div>
                <h2 class="title-text">МЕССЕНДЖЕРЫ</h2>
                <div class="links-messangers">
                    <a href="#">
                        <img src="../assets/images/icons/tg_icon.svg" alt="">
                    </a>
                    <a href="#">
                        <img src="../assets/images/icons/wts_icon.svg" alt="">
                    </a>
                </div>
                <h2 class="title-text">НАШИ СОЦ.СЕТИ</h2>
                <div class="links-messangers">
                    <a href="#">
                        <img src="../assets/images/icons/vk_icon.svg" alt="">
                    </a>
                </div>
            </div>
            
            <div class="subscribe-news">
                <h2 class="title-text">ПОДПИСКА НА НОВОСТИ</h2>
                <p class="info-subscribe">Будьте в курсе скидок и новостей</p>
                <form class="subscribe-form" @submit.prevent="onSubscribe">
                    <input
                        class="subscribe-input"
                        type="email"
                        placeholder="Ваш email"
                        aria-label="Ваш email"
                        required
                    />
                    <button class="subscribe-send" type="submit" aria-label="Отправить">›</button>
                </form>
                <p class="subscribe-legal">Подписываясь на рассылку вы<br>соглашаетесь с обработкой персональных данных</p>
                <div class="subscribe-links">
                    <a href="#" class="subscribe-link">ПОЛИТИКА КОНФИДЕНЦИАЛЬНОСТИ</a>
                    <a href="#" class="subscribe-link">ПОЛЬЗОВАТЕЛЬСКОЕ СОГЛАШЕНИЕ</a>
                </div>
            </div>
        </div>
        
    </footer>
</template>

<script>
export default {
  name: 'SiteFooter'
}
</script>