<template>
  <TopPanel />
  <router-view />
</template>

<script  lang="ts">
import { defineComponent } from "vue";
import TopPanel from "@/components/TopPanel.vue";

export default defineComponent({
  name: "App",
  components: {
    TopPanel,
  },
});
</script>

<style lang="scss">
body {
  margin: 0;
  padding: 0;
}

#app {
  font-family: Avenir, Helvetica, Arial, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  text-align: center;
  color: #2c3e50;
}

nav {
  a {
    font-weight: bold;
    color: #2c3e50;

    &.router-link-exact-active {
      color: #42b983;
    }
  }
}
</style>
