export interface PagedResult<T> {
  items: T[]
  totalCount: number
  page: number
  pageSize: number
  totalPages: number
}

export interface LoginRequest {
  email: string
  password: string
  deviceInfo?: string
}

export interface LoginResult {
  accessToken: string
  refreshToken: string
  user: UserDto
}

export interface UserDto {
  id: string
  email: string
  firstName: string
  lastName: string
  fullName: string
  role: string
  isActive: boolean
}

export interface ClientDto {
  id: string
  businessName: string
  nit: string
  type: string
  contactName?: string
  contactPhone?: string
  contactEmail?: string
  isActive: boolean
  createdAt: string
}

export interface ClientDetailDto extends ClientDto {
  contracts: ContractSummaryDto[]
}

export interface ContractSummaryDto {
  id: string
  contractNumber: string
  startDate: string
  endDate: string
  status: string
}

export interface WorkOrderDto {
  id: string
  orderNumber: string
  locationId: string
  assignedTechnicianId?: string
  scheduledDate: string
  status: string
  notes?: string
}

export interface ContractDto {
  id: string
  contractNumber: string
  clientId: string
  clientName: string
  startDate: string
  endDate: string
  status: string
  notes?: string
}

export interface LocationDto {
  id: string
  name: string
  address: string
  municipality: string
  department: string
  clientName: string
}

export interface InspectionTypeDto {
  id: string
  name: string
  description?: string
  requiresCertificate: boolean
}

export type ClientType = 'Residential' | 'Commercial' | 'Industrial'
export type WorkOrderStatus = 'Draft' | 'Scheduled' | 'Assigned' | 'InProgress' | 'Completed' | 'Cancelled'
