<template>
    <section class="account-layout">
      <ProfileHero />
      <div v-if="user">
        <div class="account-card">
          <div class="profile-header">
            <div style="display:flex;align-items:center; gap:14px;">
              <div class="profile-name-email">
                <div class="profile-name">{{ user.name || 'Без имени' }}</div>
                <div class="profile-email">{{ user.email }}</div>
              </div>
            </div>
            <div class="profile-actions">
              <span class="profile-id">Номер: {{ user.id }}</span>
            </div>
          </div>
          <div class="brand-divider"></div>
          <div class="form">
            <div class="profile-grid" v-if="editing">
              <div class="input-group">
                <label>Имя</label>
                <input type="text" v-model="form.name" placeholder="Ваше имя" />
              </div>
              <div class="input-group">
                <label>Телефон</label>
                <input type="tel" v-model="form.phone" placeholder="+7 999 123-45-67" disabled />
              </div>
            </div>
            <div v-else>
              <div class="form-row">
                <div class="form-label">Имя</div>
                <div>{{ user.name || '—' }}</div>
              </div>
              <div class="form-row">
                <div class="form-label">Email</div>
                <div>{{ user.email }}</div>
              </div>
              <div class="form-row">
                <div class="form-label">Телефон</div>
                <div>{{ user.phone || '—' }}</div>
              </div>
            </div>
            <div style="display:flex;gap:10px;margin-top:12px;">
              <button v-if="!editing" class="btn btn-primary" @click="startEdit">РЕДАКТИРОВАТЬ</button>
              <button v-if="editing" class="btn btn-primary" :disabled="saving" @click="saveProfile">СОХРАНИТЬ</button>
              <button v-if="editing" class="btn btn-secondary" :disabled="saving" @click="cancelEdit">ОТМЕНА</button>
              <button class="btn btn-outline" @click="logout">ВЫЙТИ</button>
            </div>
            <p v-if="error" class="error-text">{{ error }}</p>
            <p v-if="success" class="success-text">Данные обновлены</p>
          </div>
        </div>
      </div>
      <AccauntAuthTabs v-else @logged-in="onLoggedIn" />
    </section>
  </template>

<script setup>
    import {ref, onMounted} from 'vue'
    import ProfileHero from '@/components/ProfileHero.vue';
    import AccauntAuthTabs from '@/components/AccauntAuthTabs.vue';
    const avatarSrc = new URL('../assets/images/icons/аватарка.jpg', import.meta.url).href

    const user = ref(null)
    const editing = ref(null)
    const saving = ref(null)
    const error = ref('')
    const success = ref(false)
    const form = ref({name: '', phone: ''})

    function loadUser(){
        try{
            const raw = localStorage.getItem('xwear_user')
            user.value = raw ? JSON.parse(raw) : null
        }
        catch{
            user.value = null
        }
    }

    function onLoggedIn(u){
        user.value = u
    }

    function logout(){
        localStorage.removeItem('xwear_user')
        user.value = null
    }

    function startEdit(){           
        error.value = ''
        success.value = ''
        editing.value = true
        form.value = {name: user.value?.name || '', phone: user.value?.phone || ''}
    }

    function cancelEdit(){
        editing.value = false
        success.value = false
        error.value = ''
    }
    function onPhoneEditInput(e) {
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
        form.value.phone = r
    }
    async function saveProfile() {
        if (!user.value) return
        const id = Number(user.value?.id)
        if (!Number.isInteger(id) || id <= 0){
            error.value = "Неверный номер пользователя"; return
        }

        // Validation
        const name = form.value.name || ''
        if (name.length < 2 || /[^a-zA-Zа-яА-ЯёЁ]/.test(name)) {
             error.value = "Имя должно быть не короче 2 символов и содержать только буквы";
             return
        }

        saving.value = true
        error.value = ''
        try{
            const res = await fetch(`/api/Users/${id}`, {
                method: 'PUT', 
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ name: name })
            })
            if (res.ok){
                success.value = true
                user.value.name = name
                localStorage.setItem('xwear_user', JSON.stringify(user.value))
                setTimeout(() => {
                    editing.value = false
                    success.value = false
                }, 1500)
            }
            else {
                if (res.status === 400) {
                     const txt = await res.text()
                     error.value = txt || "Некорректные данные"
                } else {
                     error.value = "Ошибка при сохранении"
                }
            }
        }
        catch(e){
            error.value = "Ошибка сети"
        }
        finally{
            saving.value = false
        }
    }
    onMounted(loadUser)
    
</script>

<script>
    export default {name: "AccountView"}
</script>