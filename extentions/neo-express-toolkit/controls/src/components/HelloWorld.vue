<script setup>
import { ref } from 'vue'

defineProps({
  msg: String,
})

const count = ref(0)

function sayHello() {
  vscode.postMessage({
    message: 'Hello World!!!',
  })
}
</script>

<template>
  <h1>{{ msg }}</h1>

  <div class="card">
    <button type="button" @click="count++">count is {{ count }}</button>
    <p>
      Edit
      <code>components/HelloWorld.vue</code> to test HMR
    </p>
  </div>

  <div class="card">
    <button type="button" @click="sayHello">Say Hello in vscode</button>
  </div>

  <p>
    Check out
    <a href="https://vuejs.org/guide/quick-start.html#local" target="_blank"
      >create-vue</a
    >, the official Vue + Vite starter
  </p>
  <p>
    Install
    <a href="https://github.com/vuejs/language-tools" target="_blank">Volar</a>
    in your IDE for a better DX
  </p>
  <p>Click on the Vite and Vue logos to learn more</p>
</template>

<style scoped>
</style>
