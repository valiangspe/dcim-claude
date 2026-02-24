<template>
  <div class="container-fluid py-4">
    <h1 class="h3 mb-4">SSO / LDAP Configuration</h1>

    <div class="row">
      <div class="col-lg-6">
        <div class="card mb-4">
          <div class="card-header">
            <h5 class="mb-0">LDAP Provider</h5>
          </div>
          <div class="card-body">
            <div class="mb-3">
              <label class="form-label">Server</label>
              <p class="fw-medium font-monospace">{{ ldap.server }}</p>
            </div>
            <div class="mb-3">
              <label class="form-label">Base DN</label>
              <p class="fw-medium font-monospace small">{{ ldap.baseDn }}</p>
            </div>
            <div class="mb-3">
              <label class="form-label">User Filter</label>
              <p class="fw-medium font-monospace small">{{ ldap.userFilter }}</p>
            </div>
            <div class="mb-0">
              <label class="form-label">Status</label>
              <div>
                <span :class="['badge', ldap.enabled ? 'bg-success' : 'bg-secondary']">
                  {{ ldap.enabled ? 'Enabled' : 'Disabled' }}
                </span>
              </div>
            </div>
          </div>
        </div>
      </div>

      <div class="col-lg-6">
        <div class="card mb-4">
          <div class="card-header">
            <h5 class="mb-0">SSO Provider</h5>
          </div>
          <div class="card-body">
            <div class="mb-3">
              <label class="form-label">Provider Type</label>
              <p class="fw-medium">{{ sso.provider }}</p>
            </div>
            <div class="mb-3">
              <label class="form-label">Tenant ID</label>
              <p class="fw-medium font-monospace small">{{ sso.tenantId }}</p>
            </div>
            <div class="mb-3">
              <label class="form-label">Authorization Endpoint</label>
              <p class="fw-medium font-monospace small text-truncate">{{ sso.authEndpoint }}</p>
            </div>
            <div class="mb-0">
              <label class="form-label">Status</label>
              <div>
                <span :class="['badge', sso.enabled ? 'bg-success' : 'bg-secondary']">
                  {{ sso.enabled ? 'Configured' : 'Not Configured' }}
                </span>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <div class="card">
      <div class="card-header">
        <h5 class="mb-0">Authentication Statistics</h5>
      </div>
      <div class="card-body">
        <div class="row text-center">
          <div class="col-md-4">
            <div class="mb-2">
              <span class="badge bg-primary" style="font-size: 18px; padding: 8px 12px;">
                {{ stats.ldapUsers }}
              </span>
            </div>
            <p class="text-muted mb-0">LDAP Users</p>
          </div>
          <div class="col-md-4">
            <div class="mb-2">
              <span class="badge bg-info" style="font-size: 18px; padding: 8px 12px;">
                {{ stats.ssoUsers }}
              </span>
            </div>
            <p class="text-muted mb-0">SSO Users</p>
          </div>
          <div class="col-md-4">
            <div class="mb-2">
              <span class="badge bg-success" style="font-size: 18px; padding: 8px 12px;">
                {{ stats.lastSync }}
              </span>
            </div>
            <p class="text-muted mb-0">Last Sync</p>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
const ldap = {
  server: 'ldap.company.local:389',
  baseDn: 'cn=users,dc=company,dc=local',
  userFilter: '(&(objectClass=person)(sAMAccountName=*))',
  enabled: true
};

const sso = {
  provider: 'Azure AD',
  tenantId: '12345678-1234-1234-1234-123456789012',
  authEndpoint: 'https://login.microsoftonline.com/.../oauth2/v2.0/authorize',
  enabled: true
};

const stats = {
  ldapUsers: 87,
  ssoUsers: 23,
  lastSync: '2026-02-20 14:30'
};
</script>
