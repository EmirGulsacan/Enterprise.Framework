export interface Employee {
  id: number;
  firstName: string;
  lastName: string;
  email: string;
  title: string;
  department: string;
  createdAtUtc?: string;
  createdBy?: string;
  lastModifiedAtUtc?: string;
  lastModifiedBy?: string;
}

export interface Asset {
  id: number;
  name: string;
  serialNumber: string;
  purchaseDate: string;
  status: string;
  assignedEmployeeId?: number;
  assignedEmployee?: Employee;
  createdAtUtc?: string;
  createdBy?: string;
  lastModifiedAtUtc?: string;
  lastModifiedBy?: string;
}

export interface Maintenance {
  id: number;
  assetId: number;
  asset?: Asset;
  scheduledDate: string;
  completedDate?: string;
  notes: string;
  isCompleted: boolean;
  createdAtUtc?: string;
  createdBy?: string;
  lastModifiedAtUtc?: string;
  lastModifiedBy?: string;
}

export interface Labor {
  id: number;
  maintenanceId: number;
  maintenance?: Maintenance;
  employeeId: number;
  employee?: Employee;
  hoursWorked: number;
  hourlyRate: number;
  createdAtUtc?: string;
  createdBy?: string;
  lastModifiedAtUtc?: string;
  lastModifiedBy?: string;
}

export interface Document {
  id: number;
  fileName: string;
  contentType: string;
  path: string;
  relatedEntityType: string;
  relatedEntityId: number;
  createdAtUtc?: string;
  createdBy?: string;
  lastModifiedAtUtc?: string;
  lastModifiedBy?: string;
}
