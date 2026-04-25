/* ====================================
   Search & Filter Handler
   ==================================== */

/**
 * Advanced Search and Filter Manager
 * Handles real-time search, filtering, and sorting
 */

class SearchFilterManager {
    constructor(config) {
        this.config = {
            searchInputSelector: config.searchInputSelector || '[data-search-input]',
            filterSelectors: config.filterSelectors || {},
            tableSelector: config.tableSelector || null,
            minChars: config.minChars || 2,
            debounceDelay: config.debounceDelay || 300,
            ...config
        };

        this.state = {
            searchTerm: '',
            filters: {},
            results: []
        };

        this.debounceTimer = null;
        this.init();
    }

    init() {
        // Search input listener
        const searchInput = document.querySelector(this.config.searchInputSelector);
        if (searchInput) {
            searchInput.addEventListener('input', (e) => this.handleSearch(e));
        }

        // Filter listeners
        Object.entries(this.config.filterSelectors).forEach(([key, selector]) => {
            const element = document.querySelector(selector);
            if (element) {
                element.addEventListener('change', (e) => this.handleFilter(key, e));
            }
        });
    }

    handleSearch(e) {
        const searchTerm = e.target.value.trim();

        if (searchTerm.length >= this.config.minChars || searchTerm.length === 0) {
            clearTimeout(this.debounceTimer);
            this.debounceTimer = setTimeout(() => {
                this.state.searchTerm = searchTerm;
                this.performSearch();
            }, this.config.debounceDelay);
        }
    }

    handleFilter(key, e) {
        const value = e.target.value;
        if (value) {
            this.state.filters[key] = value;
        } else {
            delete this.state.filters[key];
        }
        this.performSearch();
    }

    performSearch() {
        const searchTerm = this.state.searchTerm.toLowerCase();
        const rows = document.querySelectorAll(this.config.tableSelector || 'tbody tr');

        let visibleCount = 0;

        rows.forEach(row => {
            let matchesSearch = true;
            let matchesFilters = true;

            // Search in row
            if (searchTerm) {
                const rowText = row.textContent.toLowerCase();
                matchesSearch = rowText.includes(searchTerm);
            }

            // Apply filters
            Object.entries(this.state.filters).forEach(([key, value]) => {
                const cell = row.querySelector(`[data-filter="${key}"]`);
                if (cell && cell.textContent.trim() !== value) {
                    matchesFilters = false;
                }
            });

            // Show/hide row
            if (matchesSearch && matchesFilters) {
                row.style.display = '';
                visibleCount++;
            } else {
                row.style.display = 'none';
            }
        });

        // Show empty state if no results
        if (visibleCount === 0 && rows.length > 0) {
            this.showEmptyState();
        } else {
            this.hideEmptyState();
        }

        // Trigger custom event
        document.dispatchEvent(new CustomEvent('search-complete', {
            detail: { visibleCount, totalCount: rows.length }
        }));
    }

    showEmptyState() {
        let emptyState = document.querySelector('[data-empty-state]');
        if (!emptyState) {
            const table = document.querySelector(this.config.tableSelector);
            if (table && table.parentElement) {
                emptyState = document.createElement('div');
                emptyState.setAttribute('data-empty-state', 'true');
                emptyState.className = 'alert alert-info text-center';
                emptyState.innerHTML = '<i class="fas fa-search"></i> نتیجه‌ای یافت نشد';
                table.parentElement.appendChild(emptyState);
            }
        }
        if (emptyState) emptyState.style.display = 'block';
    }

    hideEmptyState() {
        const emptyState = document.querySelector('[data-empty-state]');
        if (emptyState) emptyState.style.display = 'none';
    }

    reset() {
        this.state.searchTerm = '';
        this.state.filters = {};
        const searchInput = document.querySelector(this.config.searchInputSelector);
        if (searchInput) searchInput.value = '';
        Object.values(this.config.filterSelectors).forEach(selector => {
            const element = document.querySelector(selector);
            if (element) element.value = '';
        });
        this.performSearch();
    }

    getState() {
        return { ...this.state };
    }
}

// Initialize on DOM ready
document.addEventListener('DOMContentLoaded', () => {
    const searchable = document.querySelector('[data-searchable="true"]');
    if (searchable) {
        const config = {
            searchInputSelector: searchable.dataset.searchInput || '[data-search-input]',
            tableSelector: searchable.dataset.tableSelector || 'table tbody',
            minChars: parseInt(searchable.dataset.minChars) || 2
        };

        window.searchManager = new SearchFilterManager(config);
    }
});

// Export for use
if (typeof window !== 'undefined') {
    window.SearchFilterManager = SearchFilterManager;
}
