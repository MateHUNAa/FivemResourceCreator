import React from "react";
import ReactDOM from "react-dom/client";
import App from "./App";
import "./index.css";
import { store } from "@/store";
import { Provider } from "react-redux";
import { MantineProvider } from "@mantine/core";
import { RpcProvider, MockTransport } from "@matehun/rpc";

const mock = new MockTransport();
mock.on("RetardRota", () => ({ value: 20 }));

const root = ReactDOM.createRoot(
  document.getElementById("root") as HTMLElement,
);

root.render(
  <React.StrictMode>
    <Provider store={store}>
      <MantineProvider defaultColorScheme="dark">
        <RpcProvider
          mock={import.meta.env.DEV ? mock : undefined}
          resourceName="ResourceName"
        >
          <App />
        </RpcProvider>
      </MantineProvider>
    </Provider>
  </React.StrictMode>,
);
