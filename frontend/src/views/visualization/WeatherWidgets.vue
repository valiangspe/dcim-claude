<template>
  <div class="container-fluid py-4">
    <h1 class="h3 mb-4">Weather & Location Data</h1>

    <div class="row">
      <div v-for="location in locations" :key="location.id" class="col-lg-6 mb-4">
        <div class="card h-100">
          <div class="card-header bg-light">
            <div class="d-flex justify-content-between align-items-center">
              <div>
                <h5 class="mb-0">{{ location.name }}</h5>
                <small class="text-muted">{{ location.region }}</small>
              </div>
              <div class="text-end">
                <div style="font-size: 32px;">{{ getWeatherIcon(location.condition) }}</div>
              </div>
            </div>
          </div>
          <div class="card-body">
            <div class="row mb-4">
              <div class="col-6">
                <div class="mb-3">
                  <div class="text-muted small">Current Temperature</div>
                  <div style="font-size: 28px; font-weight: bold;">{{ location.temp }}°C</div>
                </div>
                <div class="mb-3">
                  <div class="text-muted small">Feels Like</div>
                  <div style="font-size: 20px;">{{ location.feelsLike }}°C</div>
                </div>
                <div>
                  <div class="text-muted small">Wind Speed</div>
                  <div style="font-size: 20px;">{{ location.windSpeed }} km/h</div>
                </div>
              </div>
              <div class="col-6">
                <div class="mb-3">
                  <div class="text-muted small">Condition</div>
                  <div style="font-size: 18px; font-weight: 500;">{{ location.condition }}</div>
                </div>
                <div class="mb-3">
                  <div class="text-muted small">Humidity</div>
                  <div style="font-size: 20px;">{{ location.humidity }}%</div>
                </div>
                <div>
                  <div class="text-muted small">Pressure</div>
                  <div style="font-size: 20px;">{{ location.pressure }} mb</div>
                </div>
              </div>
            </div>

            <div class="mb-3">
              <div class="text-muted small mb-1">Air Quality Index</div>
              <div class="progress" style="height: 20px;">
                <div :class="getAqiColor(location.aqi)" style="width: 60%;">60 - Moderate</div>
              </div>
            </div>

            <div class="alert" :class="getAlertClass(location.impact)" role="alert" style="margin-bottom: 0;">
              <strong>DC Impact:</strong> {{ location.impact }}
            </div>
          </div>
          <div class="card-footer bg-light">
            <small class="text-muted">Last updated: {{ location.lastUpdated }}</small>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
const locations = [
  {
    id: 1,
    name: 'US-East-01 (Ashburn, VA)',
    region: 'Virginia, United States',
    temp: 8,
    feelsLike: 5,
    condition: 'Partly Cloudy',
    windSpeed: 12,
    humidity: 65,
    pressure: 1013,
    aqi: 45,
    impact: 'Low - Good cooling conditions, humidity within range',
    lastUpdated: '2026-02-20 14:35'
  },
  {
    id: 2,
    name: 'EU-Central-01 (Frankfurt)',
    region: 'Germany',
    temp: 4,
    feelsLike: 1,
    condition: 'Cloudy',
    windSpeed: 18,
    humidity: 72,
    pressure: 1008,
    aqi: 52,
    impact: 'Optimal - Cool ambient, excellent for cooling efficiency',
    lastUpdated: '2026-02-20 14:35'
  },
  {
    id: 3,
    name: 'APAC-SG-01 (Singapore)',
    region: 'Singapore',
    temp: 32,
    feelsLike: 38,
    condition: 'Thunderstorm',
    windSpeed: 28,
    humidity: 88,
    pressure: 1005,
    aqi: 85,
    impact: 'High - Hot & humid, cooling load at peak, humidity concern',
    lastUpdated: '2026-02-20 14:35'
  },
  {
    id: 4,
    name: 'US-West-01 (San Jose, CA)',
    region: 'California, United States',
    temp: 18,
    feelsLike: 17,
    condition: 'Clear',
    windSpeed: 8,
    humidity: 55,
    pressure: 1018,
    aqi: 35,
    impact: 'Good - Mild conditions, free cooling possible at night',
    lastUpdated: '2026-02-20 14:35'
  }
];

function getWeatherIcon(condition: string): string {
  const icons: Record<string, string> = {
    'Partly Cloudy': '⛅',
    'Cloudy': '☁️',
    'Thunderstorm': '⛈️',
    'Clear': '☀️',
    'Rainy': '🌧️',
    'Snowy': '🌨️'
  };
  return icons[condition] || '🌤️';
}

function getAqiColor(aqi: number): string {
  if (aqi < 50) return 'bg-success';
  if (aqi < 100) return 'bg-warning';
  return 'bg-danger';
}

function getAlertClass(impact: string): string {
  if (impact.startsWith('Low')) return 'alert-success';
  if (impact.startsWith('Good') || impact.startsWith('Optimal')) return 'alert-info';
  return 'alert-warning';
}
</script>
