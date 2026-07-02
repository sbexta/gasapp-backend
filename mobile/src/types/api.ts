export interface LoginResponse {
  accessToken: string
  refreshToken: string
  user: {
    id: string
    email: string
    fullName: string
    role: string
  }
}

export interface AgendaItemDto {
  workOrderId: string
  orderNumber: string
  locationName: string
  locationAddress: string
  municipality: string
  scheduledDate: string
  status: string
  clientName: string
}

export interface WorkOrderDetailDto {
  id: string
  orderNumber: string
  status: string
  scheduledDate: string
  notes: string | null
  location: {
    name: string
    address: string
    municipality: string
    department: string
  }
  client: {
    businessName: string
    contactPhone: string | null
  }
  inspection: {
    id: string
    status: string
  } | null
}
