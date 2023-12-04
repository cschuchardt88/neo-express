import ws from 'ws';
import { v4 as uuidv4 } from 'uuid';

type ExpressWebSocketRequestMessage = {
    version: string,
    requestid: string,
    method: string,
    params: any[],
};

type ExpressWebSocketResponseMessage = {
    version: string,
    requestid: string,
    eventid: number,
    result: any,
};

export function TestWs() {
    const client = new ws('ws://localhost:10340');
    
    client.on('open', (): void => {
        // Do something on connect.
        const getBlock: ExpressWebSocketRequestMessage = {
            version: '1.0',
            requestid: uuidv4(),
            method: 'getblock',
            params: [1000],
        };
        console.log(getBlock);
        client.send(JSON.stringify(getBlock));
    });
    
    client.on('message', (message: string): void => {
        const json: ExpressWebSocketResponseMessage = JSON.parse(message);
        // if (json.eventid === 2) {
        //     const result = json.result;
        //     console.log(result);
        // }
        console.log(json.result);
    });
}

