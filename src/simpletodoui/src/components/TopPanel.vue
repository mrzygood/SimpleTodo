<template>
  <div class="top-panel">
    <div class="logo">
      <strong><router-link to="/">SimpleTodo</router-link></strong>
    </div>
    <div>
      <nav>
        <router-link to="/">Home</router-link> |
        <router-link to="/about">About</router-link> |
        <router-link to="/about">Login</router-link>
      </nav>
    </div>
  </div>
</template>

<script lang="ts">
import {defineComponent} from "vue";

export default defineComponent({
  name: "TopPanel"
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
