<script setup lang="ts">
import { Ref, onMounted, ref } from 'vue';

type ExpressWebSocketResponseMessage = {
    version: string,
    requestid: string,
    eventid: number,
    result: any,
};

const client: Ref<WebSocket | undefined> = ref()
const hashes: Ref<string[]> = ref([])

onMounted((): void => {
    client.value = new WebSocket('ws://localhost:10340')

    client.value.onopen = () => {
        console.log('Connected to server!')
    }

    client.value.onmessage = (message): void => {
        const json: ExpressWebSocketResponseMessage = JSON.parse(message.data)
        hashes.value.push(json.result.hash)
    }
})

</script>

<template>
    <ul>
        <li v-for="n in hashes">
            {{ n }}
        </li>
    </ul>
</template>
