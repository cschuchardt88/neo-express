<script setup lang="ts">
import { Ref, onMounted, ref } from 'vue';

type ExpressWebSocketResponseMessage = {
    version: string,
    requestid: string,
    eventid: number,
    result: any,
};

const client: Ref<WebSocket | undefined> = ref()
const blocks: Ref<any[]> = ref([])

onMounted((): void => {
    client.value = new WebSocket('ws://localhost:10340')

    client.value.onopen = () => {
        console.log('Connected to server!')
    }

    client.value.onmessage = (message): void => {
        const json: ExpressWebSocketResponseMessage = JSON.parse(message.data)
        if (json.eventid === 0x02) {
            blocks.value.unshift(json.result)
        }
    }
})

</script>

<template>
    <table>
        <thead>
            <tr>
                <th>Index</th>
                <th>Hash</th>
            </tr>
        </thead>
        <tbody>
            <tr v-for="block in blocks">
                <td>{{ block.index }}</td>
                <td>{{ block.hash }}</td>
            </tr>
        </tbody>
    </table>
</template>

<style scoped>
table {
    margin-top: 1rem;
    height: 100%;
    width: 100%;
    border-collapse: collapse;
    border: 1px solid var(--vscode-editor-lineHighlightBorder);
}

thead {
    background-color: var(--vscode-editor-selectionBackground);
    color: var(--vscode-editor-selectionForeground);
    text-transform: uppercase;
    font-size: 0.6rem;
}

tr:nth-child(even) {
    border-bottom: 1px solid var(--vscode-editor-lineHighlightBorder);
    background-color: var(--vscode-editor-background);
    color: var(--vscode-editor-foreground);
}

tr:nth-child(odd) {
    border-bottom: 1px solid var(--vscode-editor-lineHighlightBorder);
    background-color: var(--vscode-editor-inactiveSelectionBackground);
    color: var(--vscode-editor-foreground);
}

td {
    text-align: center;
    padding: 5;
}
</style>
