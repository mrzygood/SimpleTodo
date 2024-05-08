<template>
  <div class="top-panel">
    <div class="logo">
      <strong><router-link to="/">SimpleTodo</router-link></strong>
    </div>
    <div>
      <nav>
        <router-link to="/">Home</router-link> |
        <router-link to="/about">About</router-link> |
        <a @click="login">Login</a>
      </nav>
    </div>
  </div>
</template>

<script lang="ts">
import {defineComponent} from "vue";
import KeyCloakService from "@/auth/KeycloakService";

export default defineComponent({
  name: "TopPanel",
  methods: {
    async login() {
      await KeyCloakService.CallLogin();
    },
    async logout() {
      await KeyCloakService.CallLogout();
    },
    async getInfo() {
      await KeyCloakService.GetUser();
      await KeyCloakService.GetBearerToken();
    }
  },
  async mounted() {
    KeyCloakService.Init();
  }
});
</script>

<style scoped lang="scss">
.top-panel {
  display: flex;
  justify-content: space-between;
  background: lightgrey;
  padding: 0.5rem;
  
  .logo {
    a {
      text-decoration: none;
      color: black;
    }
  }
}
</style>
