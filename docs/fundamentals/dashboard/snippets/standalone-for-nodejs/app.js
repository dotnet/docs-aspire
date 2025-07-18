// This must be imported first!
require('./tracing');

const express = require('express');
const { trace, metrics } = require('@opentelemetry/api');

const app = express();
const port = process.env.PORT || 3000;

// Get a tracer
const tracer = trace.getTracer('nodejs-app');

// Get a meter for custom metrics
const meter = metrics.getMeter('nodejs-app');
const requestCounter = meter.createCounter('http_requests_total', {
  description: 'Total number of HTTP requests',
});

app.get('/', (req, res) => {
    const span = tracer.startSpan('handle-home-request');
    
    try {
        console.log('Received request for home page');
        requestCounter.add(1, { route: '/', method: 'GET' });
        
        span.setAttributes({
            'http.route': '/',
            'user.agent': req.get('User-Agent') || 'unknown'
        });
        
        res.json({
            message: 'Hello from Node.js!',
            timestamp: new Date().toISOString()
        });
    } finally {
        span.end();
    }
});

app.get('/api/weather', (req, res) => {
    const span = tracer.startSpan('handle-weather-request');
    
    try {
        const weather = [
            { city: 'Seattle', temperature: 72, condition: 'Cloudy' },
            { city: 'Portland', temperature: 68, condition: 'Rainy' },
            { city: 'San Francisco', temperature: 65, condition: 'Foggy' },
            { city: 'Los Angeles', temperature: 78, condition: 'Sunny' }
        ];
        
        console.log('Received request for weather data');
        requestCounter.add(1, { route: '/api/weather', method: 'GET' });
        
        span.setAttributes({
            'http.route': '/api/weather',
            'weather.cities_count': weather.length
        });
        
        res.json(weather);
    } finally {
        span.end();
    }
});

app.listen(port, () => {
    console.log(`Server running at http://localhost:${port}`);
});