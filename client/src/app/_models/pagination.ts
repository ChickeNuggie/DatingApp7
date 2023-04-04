export interface Pagination {
    currentPage: number;
    itemsPerPage: number;
    totalItems: number;
    totalPages: number;
}

// create new paginationresult class and populate 
export class PaginatedResult<T> {
    result?: T;
    pagination?: Pagination;
}