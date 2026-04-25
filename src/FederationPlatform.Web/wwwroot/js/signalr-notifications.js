// SignalR Real-time Notifications
class SignalRNotificationManager {
    constructor() {
        this.connection = null;
        this.isConnected = false;
    }

    async init() {
        // Create SignalR connection
        this.connection = new signalR.HubConnectionBuilder()
            .withUrl("/notificationHub")
            .withAutomaticReconnect()
            .build();

        // Handle incoming notifications
        this.connection.on("ReceiveNotification", (title, message) => {
            this.showNotification(title, message);
            this.updateNotificationBadge();
        });

        // Handle reconnection
        this.connection.onreconnecting(() => {
            console.log("SignalR reconnecting...");
        });

        this.connection.onreconnected(() => {
            console.log("SignalR reconnected");
            this.isConnected = true;
        });

        this.connection.onclose(() => {
            console.log("SignalR connection closed");
            this.isConnected = false;
        });

        // Start connection
        try {
            await this.connection.start();
            this.isConnected = true;
            console.log("SignalR connected successfully");
        } catch (err) {
            console.error("SignalR connection error:", err);
            setTimeout(() => this.init(), 5000); // Retry after 5 seconds
        }
    }

    showNotification(title, message) {
        // Show browser notification if permitted
        if ("Notification" in window && Notification.permission === "granted") {
            new Notification(title, {
                body: message,
                icon: "/images/logo.png",
                badge: "/images/badge.png"
            });
        }

        // Show in-app notification
        if (window.notificationManager) {
            notificationManager.showToast(title, message, 'info');
        }

        // Play notification sound
        this.playNotificationSound();
    }

    async updateNotificationBadge() {
        try {
            const response = await fetch('/api/notification/unread-count');
            if (!response.ok) return;

            const data = await response.json();
            const badge = document.getElementById('notification-badge');
            
            if (badge) {
                if (data.unreadCount > 0) {
                    badge.textContent = data.unreadCount;
                    badge.style.display = 'inline-block';
                } else {
                    badge.style.display = 'none';
                }
            }
        } catch (error) {
            console.error('Error updating notification badge:', error);
        }
    }

    playNotificationSound() {
        const audio = new Audio('/sounds/notification.mp3');
        audio.volume = 0.3;
        audio.play().catch(err => console.log('Could not play sound:', err));
    }

    async requestNotificationPermission() {
        if ("Notification" in window && Notification.permission === "default") {
            const permission = await Notification.requestPermission();
            return permission === "granted";
        }
        return Notification.permission === "granted";
    }
}

// Initialize on page load
document.addEventListener('DOMContentLoaded', async () => {
    window.signalRNotificationManager = new SignalRNotificationManager();
    await signalRNotificationManager.init();
    
    // Request notification permission
    await signalRNotificationManager.requestNotificationPermission();
});
