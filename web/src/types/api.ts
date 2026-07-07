export interface PagedResult<T> {
  items: T[]
  total: number
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

export interface InspectionListDto {
  id: string
  workOrderId: string
  orderNumber: string
  status: string
  startedAt: string | null
  completedAt: string | null
  scheduledDate: string
}

export interface SignatureDto {
  signerName: string
  signerDocument: string | null
  signedAt: string
  signatureData: string
}

export interface InspectionDetailDto {
  id: string
  workOrderId: string
  orderNumber: string
  scheduledDate: string
  status: string
  startedAt: string | null
  completedAt: string | null
  technicianNotes: string | null
  responses: InspectionResponseDto[]
  findings: InspectionFindingDto[]
  hasSignature: boolean
  signature: SignatureDto | null
  locationLat: number | null
  locationLng: number | null
  locationCapturedAt: string | null
}

export interface InspectionResponseDto {
  id: string
  checklistItemId: string
  textValue: string | null
  boolValue: boolean | null
  numericValue: number | null
  complies: boolean
  notes: string | null
}

export interface InspectionFindingDto {
  id: string
  description: string
  severity: string
  requiresCorrection: boolean
  isResolved: boolean
  correctiveAction: string | null
  checklistItemId: string | null
}

export type ClientType = 'Residential' | 'Commercial' | 'Industrial'
export type WorkOrderStatus = 'Draft' | 'Scheduled' | 'Assigned' | 'InProgress' | 'Completed' | 'Cancelled'
export type InspectionStatus = 'Pending' | 'PreCheck' | 'InProgress' | 'TechnicalReview' | 'GeneratingDocs' | 'Completed' | 'RequiresFollowup' | 'Rejected'
