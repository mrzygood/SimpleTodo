<template>
  <div class="home">
    <AddTodo />
    <TodoList />
  </div>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import AddTodo from "@/components/AddTodo.vue";
import TodoList from "@/components/TodoList.vue";

export default defineComponent({
  name: "HomeView",
  components: {
    AddTodo,
    TodoList,
  },
});
</script>
