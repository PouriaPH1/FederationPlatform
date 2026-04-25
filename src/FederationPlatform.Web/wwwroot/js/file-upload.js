/* ====================================
   Advanced File Upload Handler
   ==================================== */

/**
 * File Upload Manager
 * Handles drag & drop, file validation, preview, and progress tracking
 */

class FileUploadManager {
    constructor(dropZoneSelector, fileInputSelector, previewSelector) {
        this.dropZone = document.querySelector(dropZoneSelector);
        this.fileInput = document.querySelector(fileInputSelector);
        this.previewContainer = document.querySelector(previewSelector);
        this.files = [];
        this.config = {
            maxSize: 10 * 1024 * 1024, // 10MB
            maxFiles: 5,
            allowedTypes: [
                'image/jpeg', 'image/png', 'image/gif',
                'application/pdf',
                'application/msword',
                'application/vnd.openxmlformats-officedocument.wordprocessingml.document'
            ]
        };
        this.init();
    }

    init() {
        if (!this.dropZone) return;

        // Drag and drop events
        this.dropZone.addEventListener('dragover', (e) => this.handleDragOver(e));
        this.dropZone.addEventListener('dragleave', (e) => this.handleDragLeave(e));
        this.dropZone.addEventListener('drop', (e) => this.handleDrop(e));

        // File input change
        if (this.fileInput) {
            this.fileInput.addEventListener('change', (e) => this.handleFileSelect(e));
        }

        // Click to select
        this.dropZone.addEventListener('click', () => {
            if (this.fileInput) this.fileInput.click();
        });
    }

    handleDragOver(e) {
        e.preventDefault();
        e.stopPropagation();
        this.dropZone.classList.add('dragover');
    }

    handleDragLeave(e) {
        e.preventDefault();
        e.stopPropagation();
        this.dropZone.classList.remove('dragover');
    }

    handleDrop(e) {
        e.preventDefault();
        e.stopPropagation();
        this.dropZone.classList.remove('dragover');

        const files = e.dataTransfer.files;
        this.handleFiles(files);
    }

    handleFileSelect(e) {
        const files = e.target.files;
        this.handleFiles(files);
    }

    handleFiles(fileList) {
        for (let file of fileList) {
            this.addFile(file);
        }
    }

    addFile(file) {
        // Check file count
        if (this.files.length >= this.config.maxFiles) {
            notify.error(`حداکثر ${this.config.maxFiles} فایل مجاز است`);
            return;
        }

        // Check file size
        if (file.size > this.config.maxSize) {
            notify.error(`حجم فایل باید کمتر از ${this.formatFileSize(this.config.maxSize)} باشد`);
            return;
        }

        // Check file type
        if (!this.config.allowedTypes.includes(file.type)) {
            notify.error(`نوع فایل ${file.type} مجاز نیست`);
            return;
        }

        // Add to files array
        const fileObj = {
            id: Date.now(),
            file: file,
            name: file.name,
            size: file.size,
            type: file.type,
            uploaded: false
        };

        this.files.push(fileObj);
        this.renderPreview(fileObj);
        notify.success(`${file.name} انتخاب شد`);
    }

    renderPreview(fileObj) {
        if (!this.previewContainer) return;

        const preview = document.createElement('div');
        preview.className = 'file-preview-item';
        preview.id = `file-${fileObj.id}`;
        preview.style.cssText = `
            padding: 12px;
            border: 1px solid #ddd;
            border-radius: 6px;
            margin-bottom: 10px;
            display: flex;
            align-items: center;
            gap: 12px;
        `;

        // Icon
        const icon = document.createElement('i');
        icon.className = this.getFileIcon(fileObj.type);
        icon.style.fontSize = '24px';
        icon.style.color = '#1E40AF';

        // Info
        const info = document.createElement('div');
        info.style.flex = '1';
        info.innerHTML = `
            <div style="font-weight: 600; margin-bottom: 4px;">${fileObj.name}</div>
            <div style="font-size: 12px; color: #666;">${this.formatFileSize(fileObj.size)}</div>
        `;

        // Remove button
        const removeBtn = document.createElement('button');
        removeBtn.type = 'button';
        removeBtn.className = 'btn btn-sm btn-danger';
        removeBtn.innerHTML = '<i class="fas fa-trash"></i>';
        removeBtn.onclick = () => this.removeFile(fileObj.id);

        preview.appendChild(icon);
        preview.appendChild(info);
        preview.appendChild(removeBtn);

        this.previewContainer.appendChild(preview);
    }

    removeFile(fileId) {
        this.files = this.files.filter(f => f.id !== fileId);
        const preview = document.getElementById(`file-${fileId}`);
        if (preview) preview.remove();
    }

    getFileIcon(mimeType) {
        const icons = {
            'image/jpeg': 'fas fa-image',
            'image/png': 'fas fa-image',
            'image/gif': 'fas fa-image',
            'application/pdf': 'fas fa-file-pdf',
            'application/msword': 'fas fa-file-word',
            'application/vnd.openxmlformats-officedocument.wordprocessingml.document': 'fas fa-file-word'
        };
        return icons[mimeType] || 'fas fa-file';
    }

    formatFileSize(bytes) {
        if (bytes === 0) return '0 Bytes';
        const k = 1024;
        const sizes = ['Bytes', 'KB', 'MB', 'GB'];
        const i = Math.floor(Math.log(bytes) / Math.log(k));
        return Math.round(bytes / Math.pow(k, i) * 100) / 100 + ' ' + sizes[i];
    }

    getFiles() {
        return this.files.map(f => f.file);
    }

    clear() {
        this.files = [];
        if (this.previewContainer) {
            this.previewContainer.innerHTML = '';
        }
    }
}

// Initialize on DOM ready
document.addEventListener('DOMContentLoaded', () => {
    const dropZones = document.querySelectorAll('[data-file-upload]');
    dropZones.forEach(zone => {
        const inputSelector = zone.dataset.fileUpload;
        const previewSelector = zone.dataset.preview || null;
        new FileUploadManager(`[data-file-upload="${inputSelector}"]`, inputSelector, previewSelector);
    });
});

// Export for use
if (typeof window !== 'undefined') {
    window.FileUploadManager = FileUploadManager;
}
