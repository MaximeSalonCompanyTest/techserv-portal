import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import { BrowserRouter } from 'react-router-dom'
import './index.css'
import App from './App.jsx'

// Starting point of the frontend: React draws the whole app inside <div id="root"> from index.html.
// BrowserRouter lets React Router read the URL and switch pages without reloading.
// StrictMode only runs extra checks in development (that's why effects can run twice there).
createRoot(document.getElementById('root')).render(
  <StrictMode>
    <BrowserRouter>
      <App />
    </BrowserRouter>
  </StrictMode>,
)
