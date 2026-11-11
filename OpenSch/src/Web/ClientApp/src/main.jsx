import React from 'react';
import { createRoot } from 'react-dom/client';
import { BrowserRouter } from 'react-router-dom';
import App from './App';
import './index.css'

const baseUrl = document.querySelector('base')?.getAttribute('href') ?? '/';
const rootElement = document.getElementById('root');

if (!rootElement) {
  throw new Error('Required #root element was not found.');
}

const root = createRoot(rootElement);

root.render(
  <BrowserRouter basename={baseUrl}>
    <App />
  </BrowserRouter>
);
