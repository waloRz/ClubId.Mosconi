const CACHE_NAME = 'clubid-cache-v1';
const urlsToCache = [
  '/',
  '/css/site.css',
  '/lib/bootstrap/dist/css/bootstrap.min.css',
  '/js/site.js',
  '/Categoria/Index' // Podés agregar las rutas que quieras que funcionen offline
];

// Instalación: Guardar archivos básicos
self.addEventListener('install', event => {
  event.waitUntil(
    caches.open(CACHE_NAME).then(cache => cache.addAll(urlsToCache))
  );
});

// Estrategia: Intentar red, si falla, usar caché
self.addEventListener('fetch', event => {
  event.respondWith(
    fetch(event.request).catch(() => caches.match(event.request))
  );
});