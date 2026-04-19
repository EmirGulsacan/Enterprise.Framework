export interface ApiResponse<T> {
    data: T;
    success: boolean;
    message: string;
    errors: string[];
    traceId: string;
}
export interface PagedResult<T> {
    items: T[];
    pageNumber: number;
    totalPages: number;
    totalCount: number;
    hasPreviousPage: boolean;
    hasNextPage: boolean;
}
