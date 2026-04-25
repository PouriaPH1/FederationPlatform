// Notification Manager - Real-time notification system
class NotificationManager {
    constructor() {
        this.notificationContainer = null;
        this.unreadBadge = null;
        this.refreshInterval = 30000; // Check every 30 seconds
        this.isInitialized = false;
    }

    init() {
        this.notificationContainer = document.getElementById('notification-container') || this.createNotificationContainer();
        this.unreadBadge = document.getElementById('notification-badge');
        this.isInitialized = true;

        // Start polling for new notifications
        this.startPolling();

        // Listen for notification events
        this.attachEventListeners();
    }

    createNotificationContainer() {
        const container = document.createElement('div');
        container.id = 'notification-container';
        container.className = 'notification-container';
        document.body.appendChild(container);
        return container;
    }

    attachEventListeners() {
        const notificationBell = document.getElementById('notification-bell');
        if (notificationBell) {
            notificationBell.addEventListener('click', (e) => {
                e.preventDefault();
                this.toggleNotificationPanel();
            });
        }
    }

    async startPolling() {
        // Initial load
        await this.updateUnreadCount();
        
        // Poll every 30 seconds
        setInterval(() => this.updateUnreadCount(), this.refreshInterval);
    }

    async updateUnreadCount() {
        try {
            const response = await fetch('/api/notification/unread-count');
            if (!response.ok) return;

            const data = await response.json();
            if (data.unreadCount > 0) {
                if (this.unreadBadge) {
                    this.unreadBadge.textContent = data.unreadCount;
                    this.unreadBadge.style.display = 'inline-block';
                }
            } else {
                if (this.unreadBadge) {
                    this.unreadBadge.style.display = 'none';
                }
            }
        } catch (error) {
            console.error('Error updating unread count:', error);
        }
    }

    async toggleNotificationPanel() {
        const panel = document.getElementById('notification-panel');
        if (!panel) {
            await this.showNotificationPanel();
            return;
        }

        if (panel.style.display === 'none' || !panel.style.display) {
            await this.showNotificationPanel();
        } else {
            this.hideNotificationPanel();
        }
    }

    async showNotificationPanel() {
        let panel = document.getElementById('notification-panel');
        if (!panel) {
            panel = document.createElement('div');
            panel.id = 'notification-panel';
            panel.className = 'notification-panel';
            document.body.appendChild(panel);
        }

        panel.innerHTML = '<div class="spinner"></div>';
        panel.style.display = 'block';

        try {
            const notifications = await this.getNotifications();
            this.renderNotifications(notifications);
        } catch (error) {
            console.error('Error loading notifications:', error);
            panel.innerHTML = '<p class="error">خطا در بارگیری اعلان‌ها</p>';
        }
    }

    hideNotificationPanel() {
        const panel = document.getElementById('notification-panel');
        if (panel) {
            panel.style.display = 'none';
        }
    }

    async getNotifications() {
        const response = await fetch('/api/notification/unread');
        if (!response.ok) throw new Error('Failed to fetch notifications');
        return await response.json();
    }

    renderNotifications(notifications) {
        const panel = document.getElementById('notification-panel');
        if (!panel) return;

        if (notifications.length === 0) {
            panel.innerHTML = '<div class="empty-state"><p>اعلانی وجود ندارد</p></div>';
            return;
        }

        let html = '<div class="notification-header">';
        html += '<h4>اعلان‌ها</h4>';
        html += `<button class="mark-all-btn" onclick="notificationManager.markAllAsRead()">تمام را خوانده شده علامت‌گذاری کن</button>`;
        html += '</div>';
        html += '<div class="notification-list">';

        notifications.forEach(notif => {
            const icon = this.getNotificationIcon(notif.type);
            const date = new Date(notif.createdAt).toLocaleDateString('fa-IR');
            html += `
                <div class="notification-item ${notif.isRead ? 'read' : 'unread'}">
                    <div class="notification-content">
                        <div class="notification-header-item">
                            <span class="notification-icon">${icon}</span>
                            <h5>${notif.title}</h5>
                        </div>
                        <p class="notification-message">${notif.message}</p>
                        <small class="notification-date">${date}</small>
                    </div>
                    <button class="notification-close" onclick="notificationManager.deleteNotification(${notif.id})">×</button>
                </div>
            `;
        });

        html += '</div>';
        panel.innerHTML = html;
    }

    getNotificationIcon(type) {
        const icons = {
            'Activity': '📝',
            'News': '📰',
            'System': '⚙️',
            'Approval': '✅',
            'Rejection': '❌'
        };
        return icons[type] || '🔔';
    }

    async markAsRead(id) {
        try {
            const response = await fetch(`/api/notification/mark-as-read/${id}`, {
                method: 'POST'
            });

            if (response.ok) {
                await this.updateUnreadCount();
                await this.showNotificationPanel();
            }
        } catch (error) {
            console.error('Error marking notification as read:', error);
        }
    }

    async markAllAsRead() {
        try {
            const response = await fetch('/api/notification/mark-all-as-read', {
                method: 'POST'
            });

            if (response.ok) {
                await this.updateUnreadCount();
                await this.showNotificationPanel();
            }
        } catch (error) {
            console.error('Error marking all as read:', error);
        }
    }

    async deleteNotification(id) {
        try {
            const response = await fetch(`/api/notification/delete/${id}`, {
                method: 'DELETE'
            });

            if (response.ok) {
                await this.updateUnreadCount();
                await this.showNotificationPanel();
            }
        } catch (error) {
            console.error('Error deleting notification:', error);
        }
    }

    showToast(title, message, type = 'info') {
        const icon = this.getNotificationIcon(type);
        const toast = document.createElement('div');
        toast.className = `toast toast-${type}`;
        toast.innerHTML = `
            <div class="toast-icon">${icon}</div>
            <div class="toast-content">
                <strong>${title}</strong>
                <p>${message}</p>
            </div>
            <button class="toast-close" onclick="this.parentElement.remove()">×</button>
        `;

        if (!this.notificationContainer) {
            this.notificationContainer = this.createNotificationContainer();
        }

        this.notificationContainer.appendChild(toast);

        // Auto-dismiss after 5 seconds
        setTimeout(() => {
            toast.remove();
        }, 5000);
    }
}

// Initialize on page load
document.addEventListener('DOMContentLoaded', () => {
    window.notificationManager = new NotificationManager();
    notificationManager.init();
});
