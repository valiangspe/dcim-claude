const BASE = `${import.meta.env.VITE_APP_BASE_URL || ''}/api`

async function fetchApi<T>(url: string, options?: RequestInit): Promise<T> {
  const res = await fetch(`${BASE}${url}`, {
    headers: { 'Content-Type': 'application/json' },
    ...options,
  })
  if (!res.ok) {
    throw new Error(`API ${res.status}: ${res.statusText}`)
  }
  if (res.status === 204) return undefined as T
  return res.json()
}

function crudApi<T>(resource: string) {
  return {
    getAll: () => fetchApi<T[]>(`/${resource}`),
    getById: (id: number) => fetchApi<T>(`/${resource}/${id}`),
    create: (data: Partial<T>) => fetchApi<T>(`/${resource}`, { method: 'POST', body: JSON.stringify(data) }),
    update: (id: number, data: Partial<T>) => fetchApi<T>(`/${resource}/${id}`, { method: 'PUT', body: JSON.stringify(data) }),
    remove: (id: number) => fetchApi<void>(`/${resource}/${id}`, { method: 'DELETE' }),
  }
}

export interface PowerFeed { id: number; name: string; capacity: number; load: number; status: string }
export interface CoolingUnit { id: number; name: string; type: string; capacity: number; load: number; supplyTemp: number; returnTemp: number; fanSpeed: number; status: string }
export interface Hall { id: number; name: string; sqft: number; racks: number; occupied: number; totalU: number; usedU: number; power: number }
export interface NetworkLink { id: number; name: string; capacity: number; usage: number; latency: number; loss: number; status: string }
export interface Asset { id: number; name: string; class: string; installed: number; planned: number; archived: number }
export interface Alarm { id: number; source: string; severity: string; message: string; time: string; acknowledged: boolean }
export interface ChangeRequest { id: number; requestId: string; requester: string; type: string; priority: string; status: string; requestedDate: string }
export interface User { id: number; name: string; email: string; role: string; lastLogin: string; status: string }
export interface Report { id: number; name: string; description: string; color: string; lastRun: string }
export interface SecurityIncident { id: number; type: string; timestamp: string; location: string; severity: string; status: string }
export interface EnvironmentalMetric { id: number; category: string; name: string; value: number; unit: string; month: number; year: number }
export interface Sensor { id: number; sensorId: string; name: string; type: string; location: string; status: string; minThreshold: string; maxThreshold: string; pollingInterval: string }

export const powerFeedsApi = crudApi<PowerFeed>('power-feeds')
export const coolingUnitsApi = crudApi<CoolingUnit>('cooling-units')
export const hallsApi = crudApi<Hall>('halls')
export const networkLinksApi = crudApi<NetworkLink>('network-links')
export const assetsApi = crudApi<Asset>('assets')
export const alarmsApi = crudApi<Alarm>('alarms')
export const changeRequestsApi = crudApi<ChangeRequest>('change-requests')
export const usersApi = crudApi<User>('users')
export const reportsApi = crudApi<Report>('reports')
export const securityIncidentsApi = crudApi<SecurityIncident>('security-incidents')
export const environmentalMetricsApi = crudApi<EnvironmentalMetric>('environmental-metrics')
export const sensorsApi = crudApi<Sensor>('sensors')

export async function seedDatabase() {
  return fetchApi<{ seeded: boolean; counts: Record<string, number> }>('/seed', { method: 'POST' })
}
