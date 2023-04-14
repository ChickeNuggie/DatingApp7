//make it easier to use inside services.
export interface Group {
    name: string;
    connections: Connection[];
}

export interface Connection {
    connectionId: string;
    username: string;
}