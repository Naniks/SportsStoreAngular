﻿export class Filter {
    category?: string;
    search?: string;
    related: boolean = false;

    reset() {
        this.category = this.search = null;
        this.related = false;
    }
}

export class Pagination {

    productPerPage: number = 4;
    currentPage = 1;
}
