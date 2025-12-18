<template>
    <div class="account-card">
      <div class="account-tabs">
        <button class="tab-btn" :class="{ active: activeTab === 'login' }" @click="activeTab = 'login'">Войти</button>
        <button class="tab-btn" :class="{ active: activeTab === 'register' }" @click="activeTab = 'register'">Регистрация</button>
      </div>
  
      <div class="tab-content" v-if="activeTab === 'login'">
        <form class="form" @submit.prevent="onLogin">
          <div class="input-group">
            <label>Email</label>
            <input type="email" placeholder="qwe@qwe.qwe" v-model="login.email" required />
          </div>
          <div class="input-group">
            <label>Пароль</label>
            <input :type="loginShowPassword ? 'text' : 'password'" placeholder="••••••••••" v-model="login.password" required />
            <a class="link" href="#" @click.prevent="loginShowPassword = !loginShowPassword">{{ loginShowPassword ? '🪬' : '🪬' }}</a>
          </div>
          <div class="form-meta">
            <label class="check">
              <input type="checkbox" />
              <span>Запомнить меня</span>
            </label>
            <a class="link" href="#">Забыли пароль?</a>
          </div>
          <button class="btn btn-primary" type="submit" :disabled="loginLoading">ВОЙТИ</button>
          <p v-if="loginError" class="error-text">{{ loginError }}</p>
          <div class="social-login">
            <span class="social-label">или через</span>
            <a href="#" class="social-btn"><img src="../assets/images/icons/vk_icon.svg" alt="VK" /></a>
          </div>
        </form>
      </div>
  
      <div class="tab-content" v-else>
        <form class="form" @submit.prevent="onRegister">
          <div class="input-row">
            <div class="input-group">
              <label>Email</label>
              <input type="email" placeholder="qwe@qwe.qwe" v-model="register.email" required />
            </div>
            <div class="input-group">
              <label>Пароль</label>
              <input :type="registerShowPassword ? 'text' : 'password'" placeholder="••••••••••" v-model="register.password" required />
              <a class="link" href="#" @click.prevent="registerShowPassword = !registerShowPassword">{{ registerShowPassword ? '🪬' : '🪬' }}</a>
            </div>
          </div>
          <div class="input-row">
            <div class="input-group">
              <label>Имя</label>
              <input type="text" placeholder="Иван" v-model="register.name" />
            </div>
            <div class="input-group">
              <label>Телефон</label>
              <input type="tel" placeholder="+7 999 123-45-67" v-model="register.phone" @input="onPhoneInput" maxlength="16" />
            </div>
          </div>
          <div class="input-group">
            <label>Повторите пароль</label>
            <input :type="registerShowPassword2 ? 'text' : 'password'" placeholder="••••••••••" v-model="register.password2" required />
            <a class="link" href="#" @click.prevent="registerShowPassword2 = !registerShowPassword2">{{ registerShowPassword2 ? '🪬' : '🪬' }}</a>
          </div>
          <button class="btn btn-primary" type="submit">СОЗДАТЬ АККАУНТ</button>
          <p v-if="registerError" class="error-text">{{ registerError }}</p>
          <p v-if="registerSuccess" class="success-text">{{ registerSuccess }}</p>
        </form>
      </div>
  
      <div class="brand-divider"></div>
      <p class="card-note">Создавая аккаунт, вы соглашаетесь с обработкой персональных данных</p>
    </div>
  </template>
  
  <script>
  export default {
    name: 'AccountAuthTabs',
    emits: ['logged-in'],
    data() {
      return {
        activeTab: 'login',
        loginLoading: false,
        loginError: '',
        registerError: '',
        registerSuccess: '',
        loginShowPassword: false,
        registerShowPassword: false,
        registerShowPassword2: false,
        login: { email: '', password: '' },
        register: { email: '', password: '', password2: '', name: '', phone: '' }
      }
    },
    methods: {
      onPhoneInput(e) {
        const v = e.target.value || ''
        let d = (v.replace(/\D/g, '') || '')
        if (d.startsWith('8')) d = '7' + d.slice(1)
        if (!d.startsWith('7')) d = '7' + d
        d = d.slice(0, 11)
        let r = '+7'
        if (d.length > 1) r += ' ' + d.slice(1,4)
        if (d.length > 4) r += ' ' + d.slice(4,7)
        if (d.length > 7) r += '-' + d.slice(7,9)
        if (d.length > 9) r += '-' + d.slice(9,11)
        this.register.phone = r
      },
      async onLogin() {
        if (!this.login.email || !this.login.password) {
          this.loginError = 'Заполните email и пароль';
          return; 
          }
        this.loginError = ''
        this.loginLoading = true
        try {
          const res = await fetch('/api/Auth/login', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ email: this.login.email, password: this.login.password })
          });
          if (!res.ok) {
            const status = res.status
            if (status === 401) this.loginError = 'Неверный email или пароль'
            else if (status >= 300 && status < 400) this.loginError = 'Редирект запроса, проверьте HTTPS'
            else this.loginError = 'Ошибка входа'
            return
          }
          const ct = res.headers.get('content-type') || ''
          const user = ct.includes('application/json') ? await res.json() : null
          if (!user) {
            this.loginError = 'Ошибка ответа сервера';
            return
          }
          localStorage.setItem('xwear_user', JSON.stringify(user));
          this.$emit('logged-in', user);
          if (this.$router) this.$router.push({ name: 'Account' })
        } 
        catch (e) {
          console.error(e)
          this.loginError = 'Сеть недоступна'
        } 
        finally {
          this.loginLoading = false
        }
      },
      async onRegister() {
        this.registerError = ''
        this.registerSuccess = ''
        if (!this.register.email || !this.register.password || !this.register.password2) {
            this.registerError = "Заполните обязательные поля";
            return
        }
        if (this.register.password !== this.register.password2) {
            this.registerError = "Пароли не совпадают";
            return
        }
        if (this.register.password.length < 6) {
            this.registerError = "Пароль слишком короткий (минимум 6 символов)";
            return
        }
        
        const name = this.register.name || ''
        if (name && (name.length < 2 || /[^a-zA-Zа-яА-ЯёЁ]/.test(name))) {
            this.registerError = "Некорректное имя (минимум 2 буквы, без цифр и символов)";
            return
        }

        try {
          const res = await fetch('/api/Auth/register', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ email: this.register.email, password: this.register.password, name: this.register.name || null, phone: this.register.phone || null })
          });
          
          if (!res.ok) {
            const txt = await res.text()
            this.registerError = txt || "Ошибка регистрации"
            return;
          }

          this.registerSuccess = "Регистрация прошла успешно!"
          this.register = { email: '', password: '', password2: '', name: '', phone: '' }
          setTimeout(() => {
              this.registerSuccess = ''
              this.activeTab = 'login'
          }, 2000)

        } 
        catch (e) { 
            console.error(e)
            this.registerError = "Ошибка сети"
        }
      }
    }
  }
  </script>
  
