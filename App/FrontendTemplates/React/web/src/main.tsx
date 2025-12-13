import React from 'react'
import ReactDOM from 'react-dom/client'
import App from './App'
import './index.css'
import { store } from '@/store'
import { Provider } from 'react-redux'
import { MantineProvider } from "@mantine/core";

ReactDOM.createRoot(document.getElementById('root') as HTMLElement).render(
  <React.StrictMode>
    <Provider store={store}>
       <MantineProvider defaultColorScheme="dark">
        <App />
      </MantineProvider>
    </Provider>
  </React.StrictMode>
)
