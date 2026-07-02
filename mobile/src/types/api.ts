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
}

export interface WorkOrderChecklistDto {
  inspectionId: string
  sections: ChecklistSectionDto[]
}

export interface ChecklistSectionDto {
  id: string
  name: string
  order: number
  items: ChecklistItemDto[]
}

export interface ChecklistItemDto {
  id: string
  question: string
  itemType: 'YesNo' | 'Text' | 'Numeric' | 'Photo' | 'Signature'
  isRequired: boolean
  order: number
  helpText: string | null
  response: ChecklistItemResponseDto | null
}

export interface ChecklistItemResponseDto {
  responseId: string
  textValue: string | null
  boolValue: boolean | null
  numericValue: number | null
  complies: boolean
  notes: string | null
}

export interface WorkOrderSummaryDto {
  id: string
  orderNumber: string
  locationId: string
  assignedTechnicianId?: string
  scheduledDate: string
  status: string
  notes?: string
}

export interface FindingDto {
  id: string
  description: string
  severity: 'Low' | 'Medium' | 'High' | 'Critical'
  requiresCorrection: boolean
  isResolved: boolean
  correctiveAction: string | null
}
