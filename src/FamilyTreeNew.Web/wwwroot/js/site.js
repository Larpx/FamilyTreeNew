/**
 * site.js - 全局页面功能脚本
 *
 * 提供站点级别的通用交互功能：
 * - 平滑滚动到页面顶部
 * - 回到顶部按钮的显示/隐藏
 * - 外部链接在新标签页打开
 * - 表单输入自动去除首尾空格
 */
(function () {
    'use strict';

    /** 回到顶部按钮元素 */
    var backToTopBtn = null;

    /** 滚动显示阈值（px），超过此值显示回到顶部按钮 */
    var SCROLL_THRESHOLD = 300;

    /**
     * 初始化全局功能
     */
    function init() {
        createBackToTopButton();
        bindScrollEvents();
        bindExternalLinks();
        bindFormTrim();
    }

    /**
     * 创建回到顶部按钮并添加到页面
     */
    function createBackToTopButton() {
        backToTopBtn = document.createElement('button');
        backToTopBtn.id = 'back-to-top';
        backToTopBtn.title = '回到顶部';
        backToTopBtn.setAttribute('aria-label', '回到顶部');
        backToTopBtn.innerHTML = '<svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><polyline points="18 15 12 9 6 15"></polyline></svg>';

        backToTopBtn.style.cssText = 'position:fixed;bottom:24px;right:24px;width:44px;height:44px;'
            + 'background:var(--color-primary);color:var(--text-inverse);border:none;'
            + 'border-radius:var(--radius-md);cursor:pointer;opacity:0;visibility:hidden;'
            + 'transition:opacity var(--transition-normal),visibility var(--transition-normal),transform var(--transition-normal);'
            + 'box-shadow:var(--shadow-md);z-index:1000;display:flex;align-items:center;justify-content:center;';

        backToTopBtn.addEventListener('click', function () {
            window.scrollTo({ top: 0, behavior: 'smooth' });
        });

        backToTopBtn.addEventListener('mouseenter', function () {
            this.style.transform = 'translateY(-3px)';
            this.style.background = 'var(--color-primary-dark)';
        });

        backToTopBtn.addEventListener('mouseleave', function () {
            this.style.transform = 'translateY(0)';
            this.style.background = 'var(--color-primary)';
        });

        document.body.appendChild(backToTopBtn);
    }

    /**
     * 绑定滚动相关事件
     *
     * 监听页面滚动，当滚动距离超过阈值时显示回到顶部按钮，
     * 否则隐藏按钮。
     */
    function bindScrollEvents() {
        var ticking = false;

        window.addEventListener('scroll', function () {
            if (!ticking) {
                window.requestAnimationFrame(function () {
                    if (window.scrollY > SCROLL_THRESHOLD) {
                        backToTopBtn.style.opacity = '1';
                        backToTopBtn.style.visibility = 'visible';
                    } else {
                        backToTopBtn.style.opacity = '0';
                        backToTopBtn.style.visibility = 'hidden';
                    }
                    ticking = false;
                });
                ticking = true;
            }
        }, { passive: true });
    }

    /**
     * 为外部链接添加新标签页打开行为
     *
     * 检测所有以 http 开头且不属于当前域名的链接，
     * 自动设置 target="_blank" 和 rel="noopener noreferrer"。
     */
    function bindExternalLinks() {
        var currentHost = window.location.hostname;

        document.querySelectorAll('a[href^="http"]').forEach(function (link) {
            if (link.hostname && link.hostname !== currentHost) {
                if (!link.getAttribute('target')) {
                    link.setAttribute('target', '_blank');
                }
                if (!link.getAttribute('rel')) {
                    link.setAttribute('rel', 'noopener noreferrer');
                }
            }
        });
    }

    /**
     * 为表单文本输入自动去除首尾空格
     *
     * 监听所有 text/search 输入框的 blur 事件，
     * 自动调用 trim() 去除首尾空白字符。
     */
    function bindFormTrim() {
        document.addEventListener('focusout', function (e) {
            if (e.target && (e.target.tagName === 'INPUT' || e.target.tagName === 'TEXTAREA')) {
                var type = e.target.getAttribute('type');
                if (type === 'text' || type === 'search' || e.target.tagName === 'TEXTAREA') {
                    if (typeof e.target.value === 'string') {
                        e.target.value = e.target.value.trim();
                    }
                }
            }
        }, { passive: true });
    }

    document.addEventListener('DOMContentLoaded', init);
})();
