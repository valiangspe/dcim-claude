<template>
  <div class="iso-compliance">
    <div class="card">
      <div class="card-header bg-warning text-dark">
        <h5 class="mb-0">ISO 22237 Compliance Checklist</h5>
      </div>
      <div class="card-body">
        <div class="row g-4">
          <div v-for="section in complianceSections" :key="section.id" class="col-lg-6">
            <div class="card border-light">
              <div class="card-header bg-light">
                <h6 class="mb-0">{{ section.name }}</h6>
                <small class="text-muted">{{ section.description }}</small>
              </div>
              <div class="card-body">
                <div v-for="item in section.items" :key="item.id" class="mb-3">
                  <div class="d-flex justify-content-between align-items-start">
                    <div class="flex-grow-1">
                      <small class="fw-semibold">{{ item.name }}</small>
                      <p class="text-muted small mb-0">{{ item.detail }}</p>
                    </div>
                    <span :class="getComplianceBadgeClass(item.status)">
                      {{ item.status }}
                    </span>
                  </div>
                </div>
              </div>
              <div class="card-footer bg-light">
                <small class="text-muted">
                  {{ getSectionStats(section) }}
                </small>
              </div>
            </div>
          </div>
        </div>
      </div>
      <div class="card-footer">
        <div class="row text-center">
          <div class="col-4">
            <small class="d-block text-muted">Total Checks</small>
            <strong>{{ getTotalChecks() }}</strong>
          </div>
          <div class="col-4">
            <small class="d-block text-muted">Compliant</small>
            <strong class="text-success">{{ getCompliantCount() }}</strong>
          </div>
          <div class="col-4">
            <small class="d-block text-muted">Non-Compliant</small>
            <strong class="text-danger">{{ getNonCompliantCount() }}</strong>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
interface ComplianceItem {
  id: string
  name: string
  detail: string
  status: string
}

interface ComplianceSection {
  id: string
  name: string
  description: string
  items: ComplianceItem[]
}

const complianceSections: ComplianceSection[] = [
  {
    id: '1',
    name: 'Site Infrastructure',
    description: 'Building and location requirements',
    items: [
      { id: '1-1', name: 'Facility Location', detail: 'Away from hazardous areas', status: 'Pass' },
      { id: '1-2', name: 'Access Control', detail: 'Physical access restrictions', status: 'Pass' },
      { id: '1-3', name: 'Environmental Controls', detail: 'Temperature and humidity control', status: 'Pass' },
      { id: '1-4', name: 'Fire Detection', detail: 'Fire alarm systems installed', status: 'Pass' }
    ]
  },
  {
    id: '2',
    name: 'Power Distribution',
    description: 'Electrical infrastructure',
    items: [
      { id: '2-1', name: 'UPS Systems', detail: 'Uninterruptible power supply', status: 'Pass' },
      { id: '2-2', name: 'Generator Backup', detail: '72-hour fuel capacity', status: 'Pass' },
      { id: '2-3', name: 'Redundancy', detail: 'N+1 power supply configuration', status: 'Fail' },
      { id: '2-4', name: 'Power Monitoring', detail: 'Continuous power monitoring', status: 'Pass' }
    ]
  },
  {
    id: '3',
    name: 'Cooling System',
    description: 'Thermal management',
    items: [
      { id: '3-1', name: 'Redundant Cooling', detail: 'N+1 cooling units', status: 'Pass' },
      { id: '3-2', name: 'Temperature Monitoring', detail: 'Real-time temperature sensors', status: 'Pass' },
      { id: '3-3', name: 'Preventive Maintenance', detail: 'Regular HVAC servicing', status: 'Pass' },
      { id: '3-4', name: 'Hot/Cold Aisle', detail: 'Proper airflow containment', status: 'Fail' }
    ]
  },
  {
    id: '4',
    name: 'Security',
    description: 'Information and physical security',
    items: [
      { id: '4-1', name: 'CCTV Surveillance', detail: 'Comprehensive video coverage', status: 'Pass' },
      { id: '4-2', name: 'Biometric Access', detail: 'Multi-factor authentication', status: 'Pass' },
      { id: '4-3', name: 'Intrusion Detection', detail: 'IDS/IPS systems deployed', status: 'Pass' },
      { id: '4-4', name: 'Encryption', detail: 'Data in transit and at rest encrypted', status: 'Pass' }
    ]
  },
  {
    id: '5',
    name: 'Management',
    description: 'Operational procedures',
    items: [
      { id: '5-1', name: 'Change Management', detail: 'Formal change control process', status: 'Pass' },
      { id: '5-2', name: 'Documentation', detail: 'Complete infrastructure documentation', status: 'Pass' },
      { id: '5-3', name: 'Disaster Recovery', detail: 'DR plan and regular testing', status: 'Fail' },
      { id: '5-4', name: 'Audit Logs', detail: 'Comprehensive audit trailing', status: 'Pass' }
    ]
  },
  {
    id: '6',
    name: 'Equipment',
    description: 'Hardware and assets',
    items: [
      { id: '6-1', name: 'Equipment Standards', detail: 'Industry-standard equipment', status: 'Pass' },
      { id: '6-2', name: 'Inventory Management', detail: 'Asset tracking system', status: 'Pass' },
      { id: '6-3', name: 'Maintenance Logs', detail: 'Equipment maintenance records', status: 'Pass' },
      { id: '6-4', name: 'Decommissioning', detail: 'Secure disposal procedures', status: 'Pass' }
    ]
  }
]

const getComplianceBadgeClass = (status: string): string => {
  const classes = 'badge'
  if (status === 'Pass') return `${classes} bg-success`
  if (status === 'Fail') return `${classes} bg-danger`
  return `${classes} bg-warning text-dark`
}

const getSectionStats = (section: ComplianceSection): string => {
  const pass = section.items.filter((item) => item.status === 'Pass').length
  const total = section.items.length
  return `${pass}/${total} compliant`
}

const getTotalChecks = (): number => {
  return complianceSections.reduce((sum, section) => sum + section.items.length, 0)
}

const getCompliantCount = (): number => {
  return complianceSections.reduce(
    (sum, section) => sum + section.items.filter((item) => item.status === 'Pass').length,
    0
  )
}

const getNonCompliantCount = (): number => {
  return complianceSections.reduce(
    (sum, section) => sum + section.items.filter((item) => item.status === 'Fail').length,
    0
  )
}
</script>

<style scoped>
.iso-compliance {
  padding: 1rem;
}

.card {
  transition: transform 0.2s;
}

.card:hover {
  transform: translateY(-2px);
}
</style>
