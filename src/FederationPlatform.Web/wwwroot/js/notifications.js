/* ====================================
   Toast & Notification System
   ==================================== */

/**
 * Toast Notification Manager
 * Display success, error, warning, and info messages
 */

class NotificationManager {
    constructor() {
        this.container = null;
        this.notifications = [];
        this.init();
    }

    init() {
        // Create notification container if it doesn't exist
        this.container = document.getElementById('notifications-container');
        if (!this.container) {
            this.container = document.createElement('div');
            this.container.id = 'notifications-container';
            this.container.style.cssText = `
                position: fixed;
                top: 20px;
                right: 20px;
                z-index: 9999;
                display: flex;
                flex-direction: column;
                gap: 10px;
            `;
            document.body.appendChild(this.container);
        }
    }

    show(message, type = 'info', duration = 5000) {
        const notification = document.createElement('div');
        const id = Date.now();
        
        // Set base styles
        notification.style.cssText = `
            padding: 16px;
            border-radius: 8px;
            box-shadow: 0 4px 12px rgba(0,0,0,0.15);
            display: flex;
            align-items: center;
            gap: 12px;
            min-width: 300px;
            animation: slideIn 0.3s ease-out;
        `;

        // Set color based on type
        const colors = {
            success: {
                bg: '#D1FAE5',
                border: '#10B981',
                text: '#065F46',
                icon: '✓'
            },
            error: {
                bg: '#FEE2E2',
                border: '#EF4444',
                text: '#991B1B',
                icon: '✕'
            },
            warning: {
                bg: '#FEF3C7',
                border: '#F59E0B',
                text: '#92400E',
                icon: '⚠'
            },
            info: {
                bg: '#DBEAFE',
                border: '#3B82F6',
                text: '#1E3A8A',
                icon: 'ⓘ'
            }
        };

        const color = colors[type] || colors.info;
        notification.style.backgroundColor = color.bg;
        notification.style.borderRight = `4px solid ${color.border}`;
        notification.style.color = color.text;

        // Create content
        const icon = document.createElement('span');
        icon.textContent = color.icon;
        icon.style.fontSize = '18px';
        icon.style.fontWeight = 'bold';

        const text = document.createElement('span');
        text.textContent = message;
        text.style.flex = '1';

        const closeBtn = document.createElement('button');
        closeBtn.textContent = '×';
        closeBtn.style.cssText = `
            background: none;
            border: none;
            font-size: 24px;
            cursor: pointer;
            color: inherit;
            padding: 0;
            opacity: 0.7;
        `;
        closeBtn.onclick = () => this.remove(notification, id);

        notification.appendChild(icon);
        notification.appendChild(text);
        notification.appendChild(closeBtn);

        this.container.appendChild(notification);
        this.notifications.push({ id, element: notification });

        // Auto remove after duration
        if (duration > 0) {
            setTimeout(() => this.remove(notification, id), duration);
        }
    }

    remove(element, id) {
        element.style.animation = 'fadeOut 0.3s ease-out';
        setTimeout(() => {
            element.remove();
            this.notifications = this.notifications.filter(n => n.id !== id);
        }, 300);
    }

    success(message, duration) {
        this.show(message, 'success', duration);
    }

    error(message, duration) {
        this.show(message, 'error', duration);
    }

    warning(message, duration) {
        this.show(message, 'warning', duration);
    }

    info(message, duration) {
        this.show(message, 'info', duration);
    }

    clear() {
        this.notifications.forEach(n => n.element.remove());
        this.notifications = [];
    }
}

// Create global instance
const notify = new NotificationManager();

// Add to window for global access
if (typeof window !== 'undefined') {
    window.notify = notify;
    window.NotificationManager = NotificationManager;
}

// Add fade out animation
const style = document.createElement('style');
style.textContent = `
    @keyframes fadeOut {
        from {
            opacity: 1;
            transform: translateX(0);
        }
        to {
            opacity: 0;
            transform: translateX(20px);
        }
    }
`;
document.head.appendChild(style);
