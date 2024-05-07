<template>
  <h2>Add new todo</h2>
  <div class="add-panel">
    <form @submit="addNewTodo">
      <input v-model="todo.title" placeholder="Title" />
      <input v-model="todo.description" placeholder="Description" />
      <button type="submit">Save</button>
    </form>
  </div>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import { CreateTodoRequest } from "@/apiClients/models/createTodoRequest";
import TodoApiClient from "@/apiClients/todoApiClient"

export default defineComponent({
  name: "AddTodo",
  props: {},
  data() {
    return {
      todo: {
        title: "test 1",
        description: "desc 1",
      }
    };
  },
  methods: {
    async addNewTodo(e: any) {
      e.preventDefault();
      const request = new CreateTodoRequest(this.todo.title, this.todo.description);
      await TodoApiClient.create(request);
    }
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
