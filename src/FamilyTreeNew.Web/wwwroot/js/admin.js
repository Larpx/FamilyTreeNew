(function() {
    'use strict';
    
    const sidebar = document.getElementById('sidebar');
    const sidebarToggle = document.getElementById('sidebarToggle');
    
    if (sidebarToggle && sidebar) {
        sidebarToggle.addEventListener('click', function() {
            sidebar.classList.toggle('collapsed');
            sidebar.classList.toggle('show');
        });
    }
    
    document.addEventListener('click', function(e) {
        if (window.innerWidth <= 991) {
            if (sidebar && !sidebar.contains(e.target) && 
                sidebarToggle && !sidebarToggle.contains(e.target)) {
                sidebar.classList.remove('show');
            }
        }
    });
    
    const currentPath = window.location.pathname;
    const navLinks = document.querySelectorAll('.sidebar-nav .nav-link');
    
    navLinks.forEach(function(link) {
        const href = link.getAttribute('href');
        if (href && currentPath.startsWith(href) && href !== '/') {
            link.classList.add('active');
        }
    });
    
    const alerts = document.querySelectorAll('.alert-dismissible');
    alerts.forEach(function(alert) {
        setTimeout(function() {
            const bsAlert = new bootstrap.Alert(alert);
            bsAlert.close();
        }, 5000);
    });
    
    const forms = document.querySelectorAll('form');
    forms.forEach(function(form) {
        form.addEventListener('submit', function() {
            const submitBtn = form.querySelector('button[type="submit"]');
            if (submitBtn) {
                submitBtn.disabled = true;
                const originalText = submitBtn.innerHTML;
                submitBtn.innerHTML = '<span class="spinner-border spinner-border-sm me-2"></span>处理中...';
                submitBtn.dataset.originalText = originalText;
            }
        });
    });
    
    const tooltips = document.querySelectorAll('[title]');
    tooltips.forEach(function(el) {
        if (el.title) {
            el.setAttribute('data-bs-toggle', 'tooltip');
        }
    });
    
    const tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    tooltipTriggerList.map(function(tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl);
    });
    
    window.showToast = function(message, type) {
        type = type || 'info';
        const toastContainer = document.getElementById('toastContainer');
        if (!toastContainer) {
            const container = document.createElement('div');
            container.id = 'toastContainer';
            container.className = 'toast-container position-fixed top-0 end-0 p-3';
            container.style.zIndex = '9999';
            document.body.appendChild(container);
        }
        
        const toastEl = document.createElement('div');
        toastEl.className = 'toast show';
        toastEl.setAttribute('role', 'alert');
        toastEl.innerHTML = 
            '<div class="toast-header">' +
            '<i class="bi bi-' + (type === 'success' ? 'check-circle text-success' : type === 'error' ? 'x-circle text-danger' : 'info-circle text-primary') + ' me-2"></i>' +
            '<strong class="me-auto">' + (type === 'success' ? '成功' : type === 'error' ? '错误' : '提示') + '</strong>' +
            '<button type="button" class="btn-close" data-bs-dismiss="toast"></button>' +
            '</div>' +
            '<div class="toast-body">' + message + '</div>';
        
        document.getElementById('toastContainer').appendChild(toastEl);
        
        setTimeout(function() {
            toastEl.remove();
        }, 5000);
    };
    
    const tables = document.querySelectorAll('.table-responsive');
    tables.forEach(function(table) {
        if (table.scrollWidth > table.clientWidth) {
            table.classList.add('has-scroll');
        }
    });
})();
