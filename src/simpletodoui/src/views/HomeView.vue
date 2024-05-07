<template>
  <div class="home">
    <AddTodo msg="Welcome to Your Vue.js + TypeScript App" />
  </div>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import AddTodo from "@/components/AddTodo.vue"; // @ is an alias to /src

export default defineComponent({
  name: "HomeView",
  components: {
    AddTodo,
  },
});
</script>
