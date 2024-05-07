<template>
  <h2>Your todos</h2>
  <div class="todo-list">
    <div class="todo-list-entry" v-for="todo in todos">
      {{ todo.title }}
    </div>
  </div>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import { TodoDto } from "@/apiClients/models/todoDto";
import TodoApiClient from "@/apiClients/todoApiClient";

export default defineComponent({
  name: "TodoList",
  props: {},
  data() {
    return {
      todos: [] as TodoDto[]
    };
  },
  async mounted() {
    const result = await TodoApiClient.getAll();
    this.todos = result.data as TodoDto[];
  }
});
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped lang="scss">
h3 {
  margin: 40px 0 0;
}
ul {
  list-style-type: none;
  padding: 0;
}
li {
  display: inline-block;
  margin: 0 10px;
}
a {
  color: #42b983;
}
</style>
