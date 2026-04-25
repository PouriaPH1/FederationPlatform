/* ====================================
   Charts Configuration
   ==================================== */

/**
 * Advanced Charts Manager
 * Integrates Chart.js for dashboard visualization
 */

class ChartsManager {
    constructor() {
        this.charts = {};
    }

    /**
     * Create a bar chart
     * Used for: Activities by University
     */
    createBarChart(canvasId, labels, data, title) {
        const ctx = document.getElementById(canvasId);
        if (!ctx) return;

        if (this.charts[canvasId]) {
            this.charts[canvasId].destroy();
        }

        this.charts[canvasId] = new Chart(ctx, {
            type: 'bar',
            data: {
                labels: labels,
                datasets: [{
                    label: title,
                    data: data,
                    backgroundColor: [
                        '#1E40AF',
                        '#0EA5E9',
                        '#06B6D4',
                        '#10B981',
                        '#F59E0B',
                        '#EF4444'
                    ],
                    borderColor: '#ffffff',
                    borderWidth: 2,
                    borderRadius: 6
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: true,
                plugins: {
                    legend: {
                        display: true,
                        position: 'top',
                        labels: {
                            font: { family: 'Vazirmatn, sans-serif', size: 14 },
                            color: '#333',
                            padding: 15
                        }
                    },
                    tooltip: {
                        backgroundColor: 'rgba(0,0,0,0.8)',
                        titleFont: { family: 'Vazirmatn, sans-serif', size: 14 },
                        bodyFont: { family: 'Vazirmatn, sans-serif', size: 12 },
                        padding: 12,
                        displayColors: true
                    }
                },
                scales: {
                    y: {
                        beginAtZero: true,
                        ticks: {
                            font: { family: 'Vazirmatn, sans-serif' },
                            color: '#666'
                        },
                        grid: {
                            color: '#f0f0f0'
                        }
                    },
                    x: {
                        ticks: {
                            font: { family: 'Vazirmatn, sans-serif' },
                            color: '#666'
                        },
                        grid: {
                            display: false
                        }
                    }
                }
            }
        });
    }

    /**
     * Create a pie chart
     * Used for: Activity Types distribution
     */
    createPieChart(canvasId, labels, data, title) {
        const ctx = document.getElementById(canvasId);
        if (!ctx) return;

        if (this.charts[canvasId]) {
            this.charts[canvasId].destroy();
        }

        this.charts[canvasId] = new Chart(ctx, {
            type: 'pie',
            data: {
                labels: labels,
                datasets: [{
                    label: title,
                    data: data,
                    backgroundColor: [
                        '#1E40AF',
                        '#0EA5E9',
                        '#06B6D4',
                        '#10B981',
                        '#F59E0B',
                        '#EF4444'
                    ],
                    borderColor: '#ffffff',
                    borderWidth: 2
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: true,
                plugins: {
                    legend: {
                        position: 'right',
                        labels: {
                            font: { family: 'Vazirmatn, sans-serif', size: 12 },
                            color: '#333',
                            padding: 15,
                            boxWidth: 16
                        }
                    },
                    tooltip: {
                        backgroundColor: 'rgba(0,0,0,0.8)',
                        titleFont: { family: 'Vazirmatn, sans-serif', size: 14 },
                        bodyFont: { family: 'Vazirmatn, sans-serif', size: 12 },
                        padding: 12,
                        callbacks: {
                            label: function(context) {
                                const label = context.label || '';
                                const value = context.parsed || 0;
                                const total = context.dataset.data.reduce((a, b) => a + b, 0);
                                const percentage = ((value / total) * 100).toFixed(1);
                                return `${label}: ${value} (${percentage}%)`;
                            }
                        }
                    }
                }
            }
        });
    }

    /**
     * Create a doughnut chart
     * Used for: Activity Status distribution
     */
    createDoughnutChart(canvasId, labels, data, title) {
        const ctx = document.getElementById(canvasId);
        if (!ctx) return;

        if (this.charts[canvasId]) {
            this.charts[canvasId].destroy();
        }

        this.charts[canvasId] = new Chart(ctx, {
            type: 'doughnut',
            data: {
                labels: labels,
                datasets: [{
                    label: title,
                    data: data,
                    backgroundColor: [
                        '#10B981',  // Approved - Green
                        '#F59E0B',  // Pending - Amber
                        '#EF4444'   // Rejected - Red
                    ],
                    borderColor: '#ffffff',
                    borderWidth: 2
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: true,
                plugins: {
                    legend: {
                        position: 'bottom',
                        labels: {
                            font: { family: 'Vazirmatn, sans-serif', size: 12 },
                            color: '#333',
                            padding: 15,
                            boxWidth: 16
                        }
                    },
                    tooltip: {
                        backgroundColor: 'rgba(0,0,0,0.8)',
                        titleFont: { family: 'Vazirmatn, sans-serif', size: 14 },
                        bodyFont: { family: 'Vazirmatn, sans-serif', size: 12 },
                        padding: 12
                    }
                }
            }
        });
    }

    /**
     * Create a line chart
     * Used for: Activity trends over time
     */
    createLineChart(canvasId, labels, datasets, title) {
        const ctx = document.getElementById(canvasId);
        if (!ctx) return;

        if (this.charts[canvasId]) {
            this.charts[canvasId].destroy();
        }

        const colors = ['#1E40AF', '#0EA5E9', '#06B6D4', '#10B981', '#F59E0B'];

        const processedDatasets = datasets.map((dataset, index) => ({
            label: dataset.label,
            data: dataset.data,
            borderColor: colors[index % colors.length],
            backgroundColor: colors[index % colors.length] + '20',
            borderWidth: 2,
            fill: true,
            tension: 0.4,
            pointRadius: 4,
            pointBackgroundColor: colors[index % colors.length],
            pointBorderColor: '#ffffff',
            pointBorderWidth: 2
        }));

        this.charts[canvasId] = new Chart(ctx, {
            type: 'line',
            data: {
                labels: labels,
                datasets: processedDatasets
            },
            options: {
                responsive: true,
                maintainAspectRatio: true,
                interaction: {
                    mode: 'index',
                    intersect: false
                },
                plugins: {
                    legend: {
                        display: true,
                        position: 'top',
                        labels: {
                            font: { family: 'Vazirmatn, sans-serif', size: 12 },
                            color: '#333',
                            padding: 15
                        }
                    },
                    tooltip: {
                        backgroundColor: 'rgba(0,0,0,0.8)',
                        titleFont: { family: 'Vazirmatn, sans-serif', size: 14 },
                        bodyFont: { family: 'Vazirmatn, sans-serif', size: 12 },
                        padding: 12
                    }
                },
                scales: {
                    y: {
                        beginAtZero: true,
                        ticks: {
                            font: { family: 'Vazirmatn, sans-serif' },
                            color: '#666'
                        },
                        grid: {
                            color: '#f0f0f0'
                        }
                    },
                    x: {
                        ticks: {
                            font: { family: 'Vazirmatn, sans-serif' },
                            color: '#666'
                        },
                        grid: {
                            display: false
                        }
                    }
                }
            }
        });
    }

    /**
     * Destroy all charts
     */
    destroyAll() {
        Object.values(this.charts).forEach(chart => {
            if (chart) chart.destroy();
        });
        this.charts = {};
    }
}

// Create global instance
const chartsManager = new ChartsManager();

// Export
if (typeof window !== 'undefined') {
    window.chartsManager = chartsManager;
    window.ChartsManager = ChartsManager;
}

// Initialize on page load if data attributes are present
document.addEventListener('DOMContentLoaded', () => {
    // This will be extended by specific pages that need charts
});
