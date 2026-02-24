<template>
  <div class="container-fluid py-4">
    <div class="row mb-4">
      <div class="col">
        <h1 class="h3 mb-0">Site Management</h1>
      </div>
      <div class="col-auto">
        <button class="btn btn-primary btn-sm">+ Add Site</button>
      </div>
    </div>

    <div class="card">
      <div class="card-body">
        <table class="table table-hover mb-0">
          <thead class="table-light">
            <tr>
              <th>Site Name</th>
              <th>Location</th>
              <th>Address</th>
              <th>Contact</th>
              <th>Capacity</th>
              <th>Status</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="site in sites" :key="site.id">
              <td class="fw-medium">{{ site.name }}</td>
              <td>{{ site.location }}</td>
              <td class="text-muted text-truncate" :title="site.address">{{ site.address }}</td>
              <td>
                <div class="small">{{ site.contact.name }}</div>
                <div class="text-muted small">{{ site.contact.phone }}</div>
              </td>
              <td>
                <div class="small">{{ site.capacity }} kW</div>
              </td>
              <td>
                <span :class="['badge', site.status === 'Active' ? 'bg-success' : site.status === 'Maintenance' ? 'bg-warning' : 'bg-danger']">
                  {{ site.status }}
                </span>
              </td>
              <td>
                <button class="btn btn-sm btn-outline-secondary">Edit</button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
const sites = [
  {
    id: 1,
    name: 'US-East-01',
    location: 'Virginia, USA',
    address: '1234 Data Center Drive, Ashburn, VA 20147, USA',
    contact: { name: 'James Wilson', phone: '+1-571-555-0101' },
    capacity: 5000,
    status: 'Active'
  },
  {
    id: 2,
    name: 'EU-Central-01',
    location: 'Frankfurt, Germany',
    address: '567 Cloud Street, 60311 Frankfurt am Main, Germany',
    contact: { name: 'Klaus Mueller', phone: '+49-69-555-0202' },
    capacity: 3500,
    status: 'Active'
  },
  {
    id: 3,
    name: 'APAC-SG-01',
    location: 'Singapore',
    address: '890 Tech Park, Singapore 138632',
    contact: { name: 'David Tan', phone: '+65-6555-0303' },
    capacity: 2500,
    status: 'Active'
  },
  {
    id: 4,
    name: 'US-West-01',
    location: 'California, USA',
    address: '4321 Silicon Valley Blvd, San Jose, CA 95110, USA',
    contact: { name: 'Maria Garcia', phone: '+1-408-555-0404' },
    capacity: 4200,
    status: 'Maintenance'
  },
  {
    id: 5,
    name: 'LEGACY-DC-01',
    location: 'New York, USA',
    address: '789 Legacy Way, New York, NY 10001, USA',
    contact: { name: 'Robert Chen', phone: '+1-212-555-0505' },
    capacity: 1200,
    status: 'Decommissioned'
  }
];
</script>
